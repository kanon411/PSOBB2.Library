using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	public sealed class DefaultTextChatEventFactory : ITextChatEventFactory
	{
		/// <inheritdoc />
		public TextChatEventData CreateChatData<TMessageType>([NotNull] EntityAssociatedData<TMessageType> incomingChatMessageEventData, [NotNull] string associatedEntityName)
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable
		{
			if(incomingChatMessageEventData == null) throw new ArgumentNullException(nameof(incomingChatMessageEventData));
			if(associatedEntityName == null) throw new ArgumentNullException(nameof(associatedEntityName));

			/*ChatMessageType messageType = MessageTypeFromChannel(incomingChatMessageEventData.Data.TargetChannel);

			string renderableMessage = $"<color=#{ComputeColorFromChatType(messageType)}>{ComputeChannelText(messageType)} {associatedEntityName}: {incomingChatMessageEventData.Data.Message}</color>";

			return new TextChatEventData(renderableMessage, incomingChatMessageEventData.EntityGuid, messageType);*/
			throw new NotImplementedException($"Haven't reimplemented chat yet.");
		}

		/// <inheritdoc />
		public TextChatEventData CreateChatData<TMessageType>([NotNull] TMessageType incomingChatMessageEventData) 
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable
		{
			if(incomingChatMessageEventData == null) throw new ArgumentNullException(nameof(incomingChatMessageEventData));

			/*ChatMessageType messageType = MessageTypeFromChannel(incomingChatMessageEventData.TargetChannel);

			string renderableMessage = $"<color=#{ComputeColorFromChatType(messageType)}>{ComputeChannelText(messageType)}: {incomingChatMessageEventData.Message}</color>";

			return new TextChatEventData(renderableMessage, messageType);*/
			throw new NotImplementedException($"Haven't reimplemented chat yet.");
		}

		private string ComputeColorFromChatType(ChatMessageType messageType)
		{
			switch(messageType)
			{
				case ChatMessageType.System:
					return "ff0000ff";
				case ChatMessageType.Zone:
					return "AA9E92ff";
				case ChatMessageType.Guild:
					return "42f442ff";
			}

			throw new NotImplementedException($"Cannot handle Chat Type: {messageType}:{(int)messageType}");
		}

		private string ComputeChannelText(ChatMessageType messageType)
		{
			switch(messageType)
			{
				case ChatMessageType.System:
					return "[System]";
				case ChatMessageType.Zone:
					return "[1. Zone]";
				case ChatMessageType.Guild:
					return "[Guild]";
			}

			throw new NotImplementedException($"Cannot handle Chat Type: {messageType}:{(int)messageType}");
		}

		private ChatMessageType MessageTypeFromChannel(ChatChannels channel)
		{
			switch(channel)
			{
				case ChatChannels.Internal:
					return ChatMessageType.System;
				case ChatChannels.ZoneChannel:
					return ChatMessageType.Zone;
				case ChatChannels.GuildChannel:
					return ChatMessageType.Guild;
			}

			throw new NotImplementedException($"Cannot handle Chat Channel: {channel}:{(int)channel}");
		}
	}
}