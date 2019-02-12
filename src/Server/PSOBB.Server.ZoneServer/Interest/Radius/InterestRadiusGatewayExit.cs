using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public sealed class InterestRadiusGatewayExit : InterestMonitorComponent
	{
		/// <inheritdoc />
		public override void OnTriggerEnter(Collider other)
		{
			//We don't do anything on entry since we need staggered exit/enter.
		}

		/// <inheritdoc />
		public override void OnTriggerExit(Collider other)
		{
			if(other == null) throw new ArgumentNullException(nameof(other));

			GameObject rootObject = other.GetRootGameObject();

			NetworkEntityGuid me = ObjectToEntityMapper.ObjectToEntityMap[transform.GetRootGameObject()];

			if(!ObjectToEntityMapper.ObjectToEntityMap.ContainsKey(rootObject))
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Tried to remove Entity: {rootObject.name} from Entity interest ID: {me} but does not exist. Is not owned.");

				return;
			}

			bool result = RadiusManager.TryEntityLeave(me, ObjectToEntityMapper.ObjectToEntityMap[rootObject]);

			if(!result)
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to exit Entity: {ObjectToEntityMapper.ObjectToEntityMap[rootObject]} to from Entity Interest ID: {me}");
		}
	}
}
