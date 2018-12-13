using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class AvatarModelChangerListener : MonoBehaviour
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
			Logger.Debug($"Encountered Model Change for Entity: {entityGuid} Changed to Id: {changeData.NewValue}");
		}
	}
}
