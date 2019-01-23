using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GladNet;
using Microsoft.AspNetCore.SignalR;

namespace Guardians
{
	public sealed class HubConnectionMessageContext : IPeerMessageContext<object>
	{
		/// <inheritdoc />
		public IConnectionService ConnectionService { get; }

		/// <inheritdoc />
		public IPeerPayloadSendService<object> PayloadSendService { get; }

		/// <inheritdoc />
		public IPeerRequestSendService<object> RequestSendService { get; }

		public IGroupManager Groups { get; }

		/// <inheritdoc />
		public HubConnectionMessageContext(
			[JetBrains.Annotations.NotNull] IConnectionService connectionService,
			[JetBrains.Annotations.NotNull] IPeerPayloadSendService<object> payloadSendService,
			[JetBrains.Annotations.NotNull] IPeerRequestSendService<object> requestSendService,
			[JetBrains.Annotations.NotNull] IGroupManager groups)
		{
			ConnectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
			PayloadSendService = payloadSendService ?? throw new ArgumentNullException(nameof(payloadSendService));
			RequestSendService = requestSendService ?? throw new ArgumentNullException(nameof(requestSendService));
			Groups = groups ?? throw new ArgumentNullException(nameof(groups));
		}
	}
}
