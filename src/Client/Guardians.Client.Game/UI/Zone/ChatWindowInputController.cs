using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;

namespace Guardians
{
	[GameInitializableSceneSpecification(GameInitializableSceneSpecificationAttribute.SceneType.ZoneGameScene)]
	public sealed class ChatWindowInputController : IGameInitializable
	{
		private IUIButton ChatInputButton { get; }

		private IUIText ChatInputText { get; }

		private IRemoteSocialTextChatHubServer ChatService { get; }

		/// <inheritdoc />
		public ChatWindowInputController(
			[KeyFilter(UnityUIRegisterationKey.ChatBox)] [NotNull] IUIButton chatInputButton,
			[KeyFilter(UnityUIRegisterationKey.ChatBox)] [NotNull] IUIText chatInputText,
			[NotNull] IRemoteSocialTextChatHubServer chatService)
		{
			ChatInputButton = chatInputButton ?? throw new ArgumentNullException(nameof(chatInputButton));
			ChatInputText = chatInputText ?? throw new ArgumentNullException(nameof(chatInputText));
			ChatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			//Allow the user to chat
			ChatInputButton.IsInteractable = true;

			ChatInputButton.AddOnClickListener(() => { ChatInputButton.IsInteractable = false; });

			//This ALWAYS runs after the sync so it's safe to renable here after it's done
			ChatInputButton.AddOnClickListenerAsync(async () =>
			{
				await ChatService.SendZoneChannelTextChatMessageAsync(new ZoneChatMessageRequestModel(ChatInputText.Text))
					.ConfigureAwait(true);

				//Do we need to rejoin the unity thread??
				await new UnityYieldAwaitable();

				//Clear and renable chat.
				ChatInputText.Text = "";
				ChatInputButton.IsInteractable = true;
			});

			return Task.CompletedTask;
		}
	}
}
