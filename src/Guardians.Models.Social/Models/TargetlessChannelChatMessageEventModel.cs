using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	/// <summary>
	/// Data model that represents an enveloped <see cref="TargetlessChannelChatMessageRequestModel"/>
	/// that is associated with the <see cref="NetworkEntityGuid"/> of the message sender.
	/// </summary>
	[JsonObject]
	public sealed class TargetlessChannelChatMessageEventModel
	{
		/// <summary>
		/// The entity associated <see cref="TargetlessChannelChatMessageRequestModel"/>.
		/// The <see cref="NetworkEntityGuid"/> represents the original sender of the message.
		/// </summary>
		[JsonProperty]
		public EntityAssociatedData<TargetlessChannelChatMessageRequestModel> ChannelMessage { get; private set; }

		/// <inheritdoc />
		public TargetlessChannelChatMessageEventModel([JetBrains.Annotations.NotNull] EntityAssociatedData<TargetlessChannelChatMessageRequestModel> channelMessage)
		{
			ChannelMessage = channelMessage ?? throw new ArgumentNullException(nameof(channelMessage));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private TargetlessChannelChatMessageEventModel()
		{
			
		}
	}
}
