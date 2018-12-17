using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Nito.AsyncEx;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unitysync.Async;

namespace Guardians
{
	[ExternalBehaviour]
	public sealed class AvatarModelChangerListenerExternal : BaseExternalComponent<GameObject>
	{
		private ILoadableContentResourceManager ContentResourceManager { get; }

		/// <summary>
		/// Mutable resource handle that represents the current avatar model
		/// resource.
		/// </summary>
		private IPrefabContentResourceHandle CurrentPrefabHandle { get; set; }

		/// <summary>
		/// Subscribable event for when the model changes.
		/// </summary>
		public event Action OnAvatarModelChangedEvent;

		public GameObject CurrentRootAvatarGameObject { get; private set; }

		/// <inheritdoc />
		public AvatarModelChangerListenerExternal(
			NetworkEntityGuid currentEntityGuid, 
			IEntityDataChangeCallbackRegisterable callbackRegister, 
			ILog logger,
			[NotNull] ILoadableContentResourceManager contentResourceManager)
			: base(logger)
		{
			ContentResourceManager = contentResourceManager ?? throw new ArgumentNullException(nameof(contentResourceManager));
			//Register a listener callback for the current Entity when its model id changes.
			callbackRegister.RegisterCallback<int>(currentEntityGuid, EntityDataFieldType.ModelId, OnModelIdeChanged);
		}

		protected override void OnInitialization([NotNull] GameObject currentAvatarRoot)
		{
			CurrentRootAvatarGameObject = currentAvatarRoot ?? throw new ArgumentNullException(nameof(currentAvatarRoot));
		}

		private void OnModelIdeChanged(NetworkEntityGuid entityGuid, EntityDataChangedArgs<int> changeData)
		{
			//Don't check the actual CurrentRootAvatarGameObject as it could be null due to engine race condition
			ThrowIfNotInitialized();

			//TODO: There are some race conditions here, it's possible that the entity has been removed from the client but the avatar download is still ongoing
			//which could cause the avatar to spawn
			ProjectVersionStage.AssertAlpha();

			if(CurrentPrefabHandle != null)
			{
				CurrentPrefabHandle.Release();
				CurrentPrefabHandle = null;
			}

			//TODO: Refactor this
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Encountered Model Change for Entity: {entityGuid} Changed to Id: {changeData.NewValue}");

			//TODO: Refactor this, will be a common loading pattern I assume
			//TODO: We can check if it's already loaded
			var avatarPrefabAsync = ContentResourceManager.LoadAvatarPrefabAsync(changeData.NewValue);

			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				//We need to await the resource but capture the context, because we'll need to be on the main thread.
				IPrefabContentResourceHandle handle = await avatarPrefabAsync
					.ConfigureAwait(true);

				GameObject gameObject = await handle.LoadPrefabAsync()
					.ConfigureAwait(true);

				//This SHOULD be the main thread
				OnPrefabResourceAvailable(gameObject);
			});
		}

		private void OnPrefabResourceAvailable([NotNull] GameObject prefab)
		{
			if(prefab == null) throw new ArgumentNullException(nameof(prefab));

			//Since this could happen many frames after initial request it's possible that
			//the avatar or entity isn't even in the world anymore, so we must check that.
			if(!CurrentRootAvatarGameObject.IsGameObjectValid())
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Encountered case despawned while loading new avatar.");
				return;
			}

			GameObject newAvatarRoot = InstantiateNewFromPrefab(prefab);

			//Now we can delete the existing avatar
			//And set the new one
			GameObject.DestroyImmediate(CurrentRootAvatarGameObject, false);
			CurrentRootAvatarGameObject = newAvatarRoot;

			OnAvatarModelChangedEvent?.Invoke();
		}

		private GameObject InstantiateNewFromPrefab(GameObject prefab)
		{
			//This should be the avatar prefab
			//and we should be on the main thread here so we can now do the spawning and such
			//Replace the current avatar root gameobject with the new one.
			GameObject newAvatarRoot = GameObject.Instantiate(prefab, CurrentRootAvatarGameObject.transform.parent);
			newAvatarRoot.transform.localScale = CurrentRootAvatarGameObject.transform.localScale;
			newAvatarRoot.transform.localPosition = Vector3.zero;
			newAvatarRoot.transform.localRotation = Quaternion.identity;
			return newAvatarRoot;
		}

		public void OnDestroy()
		{
			CurrentPrefabHandle?.Release();
		}
	}

	[Injectee]
	public sealed class AvatarModelChangerListener : ExternalizedDependencyMonoBehaviour<AvatarModelChangerListenerExternal>
	{
		public UnityEvent OnAvatarModelChangedEvent;

		[UsedImplicitly]
		[SerializeField]
		private GameObject InitialAvatarRoot;

		void Start()
		{
			//Just forward the event.
			this.ExternalDependency.OnAvatarModelChangedEvent += () => OnAvatarModelChangedEvent?.Invoke();
			ExternalDependency.Initialize(InitialAvatarRoot);
		}

		void OnDestroy()
		{
			//Forward destroy event.
			ExternalDependency.OnDestroy();
		}
	}
}
