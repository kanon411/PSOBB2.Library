﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Nito.AsyncEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSOBB
{
	[SceneTypeCreate(GameSceneType.TitleScreen)]
	public sealed class TitleScreenOnAuthenticationResultSuccessfulEventListener : BaseSingleEventListenerInitializable<IAuthenticationResultRecievedEventSubscribable, AuthenticationResultEventArgs>
	{
		//TODO: Don't expose Unity directors directly.
		private IUIPlayable SceneEndPlayable { get; }

		/// <inheritdoc />
		public TitleScreenOnAuthenticationResultSuccessfulEventListener(IAuthenticationResultRecievedEventSubscribable subscriptionService,
			[KeyFilter(UnityUIRegisterationKey.Login)] [NotNull] IUIPlayable sceneEndPlayable) 
			: base(subscriptionService)
		{
			SceneEndPlayable = sceneEndPlayable ?? throw new ArgumentNullException(nameof(sceneEndPlayable));
		}

		/// <inheritdoc />
		protected override void OnEventFired(object source, [NotNull] AuthenticationResultEventArgs args)
		{
			if(args == null) throw new ArgumentNullException(nameof(args));

			if(!args.isSuccessful)
				return;

			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				SceneEndPlayable.Play();

				//TODO: Use the scene manager service.
				AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(1);
				loadSceneAsync.allowSceneActivation = false;

				await Task.Delay(4500);

				loadSceneAsync.allowSceneActivation = true;
			});
		}
	}
}
