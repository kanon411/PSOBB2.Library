using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using SceneJect.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

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
			if(Logger.IsErrorEnabled)
				Logger.Debug($"Encountered Model Change for Entity: {entityGuid} Changed to Id: {changeData.NewValue}");

			//Replace the current avatar root gameobject with the new one.
			GameObject newAvatarRoot = GameObject.Instantiate(DemoPrefabTest, CurrentRootAvatarGameObject.transform.position, Quaternion.Euler(Vector3.zero), CurrentRootAvatarGameObject.transform.parent);
			newAvatarRoot.transform.localScale = CurrentRootAvatarGameObject.transform.localScale;
			newAvatarRoot.transform.localPosition = Vector3.zero;

			//Now we can delete the existing avatar
			//And set the new one
			GameObject.DestroyImmediate(CurrentRootAvatarGameObject, false);
			CurrentRootAvatarGameObject = newAvatarRoot;

			OnAvatarModelChangedEvent?.Invoke();
		}
	}
}
