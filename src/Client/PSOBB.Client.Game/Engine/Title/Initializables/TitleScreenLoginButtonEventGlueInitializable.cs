﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using PSOBB.Client;

namespace PSOBB
{
	[AdditionalRegisterationAs(typeof(ILoginButtonClickedEventSubscribable))]
	[SceneTypeCreate(GameSceneType.TitleScreen)]
	public sealed class TitleScreenLoginButtonEventGlueInitializable : IGameInitializable, ILoginButtonClickedEventSubscribable
	{
		private IUIButton LoginButton { get; }

		/// <inheritdoc />
		public event EventHandler OnLoginButtonClicked;

		/// <inheritdoc />
		public TitleScreenLoginButtonEventGlueInitializable([KeyFilter(UnityUIRegisterationKey.Login)] [NotNull] IUIButton loginButton)
		{
			LoginButton = loginButton ?? throw new ArgumentNullException(nameof(loginButton));
		}

		/// <inheritdoc />
		public Task OnGameInitialized()
		{
			//When login button is pressed we should temporarily disable the interactivity of the login button.
			LoginButton.AddOnClickListener(() =>
			{
				LoginButton.IsInteractable = false;

				//We should just dispatch on click.
				OnLoginButtonClicked?.Invoke(this, EventArgs.Empty);
			});

			return Task.CompletedTask;
		}
	}
}
