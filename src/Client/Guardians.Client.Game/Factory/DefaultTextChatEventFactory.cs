using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class DefaultTextChatEventFactory : ITextChatEventFactory
	{
		/// <inheritdoc />
		public TextChatEventData CreateChatData<TMessageType>([NotNull] EntityAssociatedData<TMessageType> incomingChatMessageEventData, [NotNull] string associatedEntityName)
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable
		{
			if(incomingChatMessageEventData == null) throw new ArgumentNullException(nameof(incomingChatMessageEventData));
			if(associatedEntityName == null) throw new ArgumentNullException(nameof(associatedEntityName));

			ChatMessageType messageType = MessageTypeFromChannel(incomingChatMessageEventData.Data.TargetChannel);

			string renderableMessage = $"{ComputeChannelText(messageType)} {associatedEntityName}: {incomingChatMessageEventData.Data.Message}";

			return new TextChatEventData(renderableMessage, incomingChatMessageEventData.EntityGuid, messageType);
		}

		/// <inheritdoc />
		public TextChatEventData CreateChatData<TMessageType>([NotNull] TMessageType incomingChatMessageEventData) 
			where TMessageType : ITextMessageContainable, IChatChannelAssociatable
		{
			if(incomingChatMessageEventData == null) throw new ArgumentNullException(nameof(incomingChatMessageEventData));

			ChatMessageType messageType = MessageTypeFromChannel(incomingChatMessageEventData.TargetChannel);

			string renderableMessage = $"{ComputeChannelText(messageType)}: {incomingChatMessageEventData.Message}";

			return new TextChatEventData(WrapMessageInColorBlock(messageType, renderableMessage), messageType);
		}

		private string WrapMessageInColorBlock(ChatMessageType messageType, string message)
		{
			return $"<color=#{ComputeColorFromChatType(messageType)}>{message}</color>";
		}

		private string ComputeColorFromChatType(ChatMessageType messageType)
		{
			switch(messageType)
			{
				case ChatMessageType.System:
					return "ff0000ff";
				case ChatMessageType.Zone:
					return "AA9E92";
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
			}

			throw new NotImplementedException($"Cannot handle Chat Channel: {channel}:{(int)channel}");
		}
	}
}