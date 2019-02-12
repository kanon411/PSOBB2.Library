using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Nito.AsyncEx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Guardians
{
	[ExternalBehaviour]
	public sealed class CharacterNameQuerier : BaseExternalComponent<object>
	{
		private INameQueryService NameQueryService { get; }

		private NetworkEntityGuid CharacterEntityGuid { get; }

		public event Action<string> OnNameQueryFinished;

		/// <inheritdoc />
		public CharacterNameQuerier(ILog logger, [NotNull] INameQueryService nameQueryService, [NotNull] NetworkEntityGuid characterEntityGuid) 
			: base(logger)
		{
			NameQueryService = nameQueryService ?? throw new ArgumentNullException(nameof(nameQueryService));
			CharacterEntityGuid = characterEntityGuid ?? throw new ArgumentNullException(nameof(characterEntityGuid));
		}

		/// <inheritdoc />
		protected override void OnInitialization(object o)
		{
			//TODO: Create a non-generic base that doesn't require init.
			//No init is needed.
		}

		public void QueryNameService()
		{
			UnityExtended.UnityMainThreadContext.PostAsync(async () =>
			{
				try
				{
					string name = await NameQueryService.RetrieveAsync(CharacterEntityGuid.EntityId)
						.ConfigureAwait(true);

					OnNameQueryFinished?.Invoke(name);
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Failed to NameQuery for Entity: {CharacterEntityGuid}. Error: {e.Message}");
				}
			});
		}
	}

	public sealed class CharacterNamePlateNameInitializer : ExternalizedDependencyMonoBehaviour<CharacterNameQuerier>
	{
		[Serializable]
		public class UnityEventString : UnityEvent<string> { }

		[SerializeField]
		private UnityEventString OnCharacterNameChange;

		void Start()
		{
			//On start we should tell the
			//querier to send a name query
			ExternalDependency.OnNameQueryFinished += InvokeCharacterNameChangeEvent;

			//Now we fire the querier
			ExternalDependency.QueryNameService();
		}
		
		//The reason we don't jusr egister the delegate to the invoke method
		//is because it could be null.
		private void InvokeCharacterNameChangeEvent([NotNull] string characterName)
		{
			if(string.IsNullOrWhiteSpace(characterName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(characterName));

			OnCharacterNameChange?.Invoke(characterName);
		}

		void OnDestroy()
		{
			ExternalDependency.OnNameQueryFinished += InvokeCharacterNameChangeEvent;
		}
	}
}
