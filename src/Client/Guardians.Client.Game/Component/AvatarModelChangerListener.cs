using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unitysync.Async;

namespace Guardians
{
	[Injectee]
	public sealed class AvatarModelChangerListener : SerializedMonoBehaviour
	{
		/// <summary>
		/// The current entity GUID.
		/// </summary>
		[Inject]
		private NetworkEntityGuid CurrentEntityGuid { get; set; }

		[Inject]
		private IEntityDataChangeCallbackRegisterable CallbackRegister { get; set; }

		[Inject]
		private ILog Logger { get; set; }

		[Inject]
		private IContentServerServiceClient ContentClient { get; set; }

		[Inject]
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; set; }

		public GameObject CurrentRootAvatarGameObject;

		public UnityEvent OnAvatarModelChangedEvent;

		public GameObject DemoPrefabTest;

		void Start()
		{
			//TODO: There is a leak here, because we can never unregister
			ProjectVersionStage.AssertBeta();
			//To check when avatars change we need to
			//register a callback for model field change
			CallbackRegister.RegisterCallback<int>(CurrentEntityGuid, EntityDataFieldType.ModelId, OnModelIdeChanged);
		}

		private void OnModelIdeChanged(NetworkEntityGuid entityGuid, EntityDataChangedArgs<int> changeData)
		{
			//TODO: Refactor this
			if(Logger != null && Logger.IsDebugEnabled)
				Logger.Debug($"Encountered Model Change for Entity: {entityGuid} Changed to Id: {changeData.NewValue}");

			ContentClient.RequestAvatarDownloadUrl((long)changeData.NewValue, AuthTokenRepo.RetrieveWithType())
				.UnityAsyncContinueWith(this, OnAvatarDownloadUrlRecieved)
				.ConfigureAwait(false);
		}

		private async Task OnAvatarDownloadUrlRecieved(ContentDownloadURLResponse response)
		{
			if(!response.isSuccessful)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to get DownloadUrl: {response.ResultCode}");
				return;
			}

			//At this point, we need to download the asset bundle.

			//Can't do web request not on the main thread, sadly.
			await new UnityYieldAwaitable();

			//TODO: Do we need to be on the main unity3d thread
			UnityWebRequestAsyncOperation asyncOperation = UnityWebRequestAssetBundle.GetAssetBundle(response.DownloadURL, 0).SendWebRequest();

			//TODO: We should render these operations to the loading screen UI.
			asyncOperation.completed += operation =>
			{
				AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(asyncOperation.webRequest);

				//TODO: This needs to be refactored.
				string[] paths = bundle.GetAllAssetNames();

				foreach(string p in paths)
					Debug.Log($"Found Asset in Bundle: {p}");

				GameObject asset = bundle.LoadAsset<GameObject>(paths.First());

				//This should be the avatar prefab
				//and we should be on the main thread here so we can now do the spawning and such
				//Replace the current avatar root gameobject with the new one.
				GameObject newAvatarRoot = GameObject.Instantiate(asset, CurrentRootAvatarGameObject.transform.parent);
				newAvatarRoot.transform.localScale = CurrentRootAvatarGameObject.transform.localScale;
				newAvatarRoot.transform.localPosition = Vector3.zero;
				newAvatarRoot.transform.localRotation = Quaternion.identity;

				//Now we can delete the existing avatar
				//And set the new one
				GameObject.DestroyImmediate(CurrentRootAvatarGameObject, false);
				CurrentRootAvatarGameObject = newAvatarRoot;

				OnAvatarModelChangedEvent?.Invoke();

				//TODO: Really have to look into leaking, caching and wasted memory.
				bundle.Unload(false);
			};
		}

		[Button]
		public void TestOnModelChanged()
		{
			OnModelIdeChanged(CurrentEntityGuid, new EntityDataChangedArgs<int>(1, 1));
		}
	}
}
