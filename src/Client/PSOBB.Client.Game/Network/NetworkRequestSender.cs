using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using SceneJect.Common;
using Sirenix.OdinInspector;

namespace GladMMO
{
	/// <summary>
	/// Base for network request senders.
	/// </summary>
	[Injectee]
	public abstract class NetworkRequestSender : SerializedMonoBehaviour
	{
		/// <summary>
		/// The network send service used to send the messages
		/// in the request sender.
		/// </summary>
		[Inject]
		protected IPeerPayloadSendService<GameClientPacketPayload> SendService { get; set; }

		/// <summary>
		/// The network send service used to send the messages
		/// in the request sender.
		/// </summary>
		[Inject]
		protected IPeerRequestSendService<GameClientPacketPayload> SendServiceAsync { get; set; }
	}
}
