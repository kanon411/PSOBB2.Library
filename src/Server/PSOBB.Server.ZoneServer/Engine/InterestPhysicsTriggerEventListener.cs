using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using UnityEngine;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class InterestPhysicsTriggerEventListener : IGameInitializable
	{
		private IPhysicsTriggerEventSubscribable TriggerEventSubscribable { get; }

		public IReadonlyGameObjectToEntityMappable ObjectToEntityMapper { get; }

		public ILog Logger { get; }

		public IInterestRadiusManager RadiusManager { get; }

		/// <inheritdoc />
		public InterestPhysicsTriggerEventListener([NotNull] IPhysicsTriggerEventSubscribable triggerEventSubscribable, [NotNull] IReadonlyGameObjectToEntityMappable objectToEntityMapper, [NotNull] ILog logger, [NotNull] IInterestRadiusManager radiusManager)
		{
			TriggerEventSubscribable = triggerEventSubscribable ?? throw new ArgumentNullException(nameof(triggerEventSubscribable));
			ObjectToEntityMapper = objectToEntityMapper ?? throw new ArgumentNullException(nameof(objectToEntityMapper));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			RadiusManager = radiusManager ?? throw new ArgumentNullException(nameof(radiusManager));
		}

		private void InterestTriggerExit([NotNull] object sender, [NotNull] PhysicsTriggerEventArgs args)
		{
			if(sender == null) throw new ArgumentNullException(nameof(sender));
			if(args == null) throw new ArgumentNullException(nameof(args));

			GameObject rootObject = args.ColliderThatTriggered.GetRootGameObject();

			NetworkEntityGuid me = ObjectToEntityMapper.ObjectToEntityMap[args.GameObjectTriggered.transform.GetRootGameObject()];

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

		private void InterestTriggerEnter([NotNull] object sender, [NotNull] PhysicsTriggerEventArgs args)
		{
			if(sender == null) throw new ArgumentNullException(nameof(sender));
			if(args == null) throw new ArgumentNullException(nameof(args));

			GameObject rootObject = args.ColliderThatTriggered.GetRootGameObject();

			//TODO: This WON'T work if you parent the object. We NEED a better way to handle looking up the guid from collision
			//TODO: Should entites be interested in themselves?
			//Right now the way this is set up we don't check if this is the entity itself.
			//Which means an entity is always interested in itself, unless manually removed
			//because when it enters the world it will enter its own interest radius
			//This behavior MAY change.
			NetworkEntityGuid me = ObjectToEntityMapper.ObjectToEntityMap[args.GameObjectTriggered.transform.GetRootGameObject()];

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
		public Task OnGameInitialized()
		{
			TriggerEventSubscribable.RegisterTriggerEnterEventSubscription(PhysicsTriggerEventType.InterestEnter, InterestTriggerEnter);
			TriggerEventSubscribable.RegisterTriggerEnterEventSubscription(PhysicsTriggerEventType.InterestExit, InterestTriggerExit);

			return Task.CompletedTask;
		}
	}
}
