using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;

namespace GladMMO
{
	[SceneTypeCreateGladMMO(GameSceneType.DefaultLobby)]
	public sealed class OnGroupJoinUIUnitFrameControllerEventListener : EventQueueBasedTickable<IPlayerGroupJoinedEventSubscribable, PlayerJoinedGroupEventArgs>
	{
		private IEntityDataChangeCallbackRegisterable EntityDataCallbackRegister { get; }

		private IUIUnitFrame[] GroupUnitFrames { get; }

		IReadonlyEntityGuidMappable<IEntityDataFieldContainer> EntityDataMappable { get; }

		/// <inheritdoc />
		public OnGroupJoinUIUnitFrameControllerEventListener(IPlayerGroupJoinedEventSubscribable subscriptionService,
			[NotNull] IEntityDataChangeCallbackRegisterable entityDataCallbackRegister, 
			[KeyFilter(UnityUIRegisterationKey.GroupUnitFrames)] IReadOnlyCollection<IUIUnitFrame> groupUnitFrames,
			[NotNull] IReadonlyEntityGuidMappable<IEntityDataFieldContainer> entityDataMappable,
			[NotNull] ILog logger)
			: base(subscriptionService, true, logger)
		{
			EntityDataCallbackRegister = entityDataCallbackRegister ?? throw new ArgumentNullException(nameof(entityDataCallbackRegister));
			EntityDataMappable = entityDataMappable ?? throw new ArgumentNullException(nameof(entityDataMappable));
			GroupUnitFrames = groupUnitFrames.ToArrayTryAvoidCopy();
		}

		/// <inheritdoc />
		protected override void HandleEvent(PlayerJoinedGroupEventArgs args)
		{
			//Even if we don't know them, we should register an event for it.
			EntityDataCallbackRegister.RegisterCallback<int>(args.PlayerGuid, (int)FreecraftCore.EUnitFields.UNIT_FIELD_HEALTH, OnCurrentHealthChangedValue);

			//TODO: If we come to know them after group join, we'll need to register.
			if(!EntityDataMappable.ContainsKey(args.PlayerGuid))
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Encountered GroupJoin from far-away Entity: {args.PlayerGuid.CurrentObjectGuid}");

				return;
			}

			//Very possible we don't know them
			//But if we do we should calculate their initial unitframe resources
			RecalulateHealthUI(args.PlayerGuid, EntityDataMappable[args.PlayerGuid].GetFieldValue<int>((int)FreecraftCore.EUnitFields.UNIT_FIELD_HEALTH));
		}

		private void RecalulateHealthUI(ObjectGuid player, int currentHealth)
		{
			float healthPercentage = (float)currentHealth / EntityDataMappable[player].GetFieldValue<int>((int)FreecraftCore.EUnitFields.UNIT_FIELD_MAXHEALTH);

			GroupUnitFrames[0].HealthBar.BarFillable.FillAmount = healthPercentage;

			//Also we want to see the percentage text
			GroupUnitFrames[0].HealthBar.BarText.Text = $"{currentHealth} / {EntityDataMappable[player].GetFieldValue<int>((int)FreecraftCore.EUnitFields.UNIT_FIELD_MAXHEALTH)}";
		}

		private void OnCurrentHealthChangedValue(ObjectGuid source, EntityDataChangedArgs<int> changeArgs)
		{
			RecalulateHealthUI(source, changeArgs.NewValue);
		}

		/// <inheritdoc />
		public override async Task OnGameInitialized()
		{
			await base.OnGameInitialized();
			
			//This just sets all unitframes as disabled. We'll disable them as they are needed.
			foreach(var u in GroupUnitFrames)
				u.SetElementActive(false);
		}
	}
}
