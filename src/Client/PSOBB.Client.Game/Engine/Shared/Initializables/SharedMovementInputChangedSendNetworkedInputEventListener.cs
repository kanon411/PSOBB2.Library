using System;
using System.Collections.Generic;
using System.Text;
using GladNet;

namespace PSOBB
{
	public sealed class SharedMovementInputChangedSendNetworkedInputEventListener : BaseSingleEventListenerInitializable<IMovementInputChangedEventSubscribable, MovementInputChangedEventArgs>
	{
		private IPeerPayloadSendService<GameClientPacketPayload> SendService { get; }

		/// <inheritdoc />
		public SharedMovementInputChangedSendNetworkedInputEventListener(IMovementInputChangedEventSubscribable subscriptionService, [NotNull] IPeerPayloadSendService<GameClientPacketPayload> sendService) 
			: base(subscriptionService)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, MovementInputChangedEventArgs args)
		{
			//SendService.SendMessage(new ClientMovementDataUpdateRequest())
		}
	}
}
