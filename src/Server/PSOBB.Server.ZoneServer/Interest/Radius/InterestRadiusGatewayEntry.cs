using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public sealed class InterestRadiusGatewayEntry : InterestMonitorComponent
	{
		[Tooltip("Enables some additional debug logging.")]
		[SerializeField]
		private bool EnableDebug = false;

		/// <inheritdoc />
		public override void OnTriggerEnter(Collider other)
		{
			//Don't check debug log enabled. It's been force requested.
			if(EnableDebug)
				Logger.Debug($"Interest Collider encountered. Name: {other.transform.gameObject.name}");

			if(other == null) throw new ArgumentNullException(nameof(other));

			GameObject rootObject = other.GetRootGameObject();

			//TODO: This WON'T work if you parent the object. We NEED a better way to handle looking up the guid from collision
			//TODO: Should entites be interested in themselves?
			//Right now the way this is set up we don't check if this is the entity itself.
			//Which means an entity is always interested in itself, unless manually removed
			//because when it enters the world it will enter its own interest radius
			//This behavior MAY change.
			NetworkEntityGuid me = ObjectToEntityMapper.ObjectToEntityMap[transform.GetRootGameObject()];

			if(!ObjectToEntityMapper.ObjectToEntityMap.ContainsKey(rootObject))
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Tried to enter Entity: {rootObject.name} to Entity interest ID: {me} but does not exist. Is not owned.");

				return;
			}

			//TODO: Handle non-bool result. Ex (Entered, AlreadyExists, Failed) etc.
			bool result = RadiusManager.TryEntityEnter(me, ObjectToEntityMapper.ObjectToEntityMap[rootObject]);

			if(!result)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to enter Entity: {ObjectToEntityMapper.ObjectToEntityMap[rootObject]} to Entity Interest ID: {me}");
			}
			else if(Logger.IsInfoEnabled)
				Logger.Info($"Entity: {ObjectToEntityMapper.ObjectToEntityMap[rootObject]} entered interest radius of Entity: {me}");
				
		}

		/// <inheritdoc />
		public override void OnTriggerExit(Collider other)
		{
			//We don't do anything here, it is exit only.
		}
	}
}
