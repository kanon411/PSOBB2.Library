using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace GladMMO
{
	[SceneTypeCreate(GameSceneType.WorldDownloadingScreen)]
	public sealed class WorldDownloadingGameInitializable : IGameInitializable
	{
		//TODO: Refactor this behind its own object to provide download URL for character.
		private ICharacterService CharacterService { get; }

		private ICharacterDataRepository LocalCharacterData { get; }
		
		private IReadonlyAuthTokenRepository AuthTokenRepo { get; }

		private IZoneServerService ZoneDataService { get; }

		private IContentServerServiceClient ContentService { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public WorldDownloadingGameInitializable(ICharacterService characterService, ICharacterDataRepository localCharacterData, IReadonlyAuthTokenRepository authTokenRepo, IZoneServerService zoneDataService, IContentServerServiceClient contentService, ILog logger)
		{
			CharacterService = characterService;
			LocalCharacterData = localCharacterData;
			AuthTokenRepo = authTokenRepo;
			ZoneDataService = zoneDataService;
			ContentService = contentService;
			Logger = logger;
		}

		//TODO: Refactor this behind its own object to provide download URL for character.
		/// <inheritdoc />
		public async Task OnGameInitialized()
		{
			if(Logger.IsInfoEnabled)
				Logger.Info("About to start downloading map data.");

			//When we start the loading screen for the game
			//To know what world we should load we should
			CharacterSessionDataResponse characterSessionData = await CharacterService.GetCharacterSessionData(LocalCharacterData.CharacterId, AuthTokenRepo.RetrieveWithType())
				.ConfigureAwait(false);

			if(!characterSessionData.isSuccessful)
			{
				Logger.Error($"Failed to query Character Session Data: {characterSessionData.ResultCode}:{(int)characterSessionData.ResultCode}");
				return;
			}

			//TODO: Handle failure
			ProjectVersionStage.AssertAlpha();
			//TODO: Handle throwing/error
			//We need to know the world the zone is it, so we can request a download URL for it.
			long worldId = await ZoneDataService.GetZoneWorld(characterSessionData.ZoneId)
				.ConfigureAwait(false);

			//With the worldid we can get the download URL.
			ContentDownloadURLResponse urlDownloadResponse = await ContentService.RequestWorldDownloadUrl(worldId, AuthTokenRepo.RetrieveWithType())
				.ConfigureAwait(false);

			//TODO: Handle failure
			if(urlDownloadResponse.isSuccessful)
			{
				if(Logger.IsInfoEnabled)
					Logger.Info($"Download URL: {urlDownloadResponse.DownloadURL}");

				//Can't do web request not on the main thread, sadly.
				await new UnityYieldAwaitable();

				//TODO: Do we need to be on the main unity3d thread
				UnityWebRequestAsyncOperation asyncOperation = UnityWebRequestAssetBundle.GetAssetBundle(urlDownloadResponse.DownloadURL, 0).SendWebRequest();

				//TODO: We should render these operations to the loading screen UI.
				asyncOperation.completed += operation =>
				{
					AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(asyncOperation.webRequest);

					string[] paths = bundle.GetAllScenePaths();

					foreach(string p in paths)
						Debug.Log($"Found Scene in Bundle: {p}");

					AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(paths.First()));

					sceneAsync.completed += operation1 =>
					{
						//When the scene is finished loading we should cleanup the asset bundle
						//Don't clean up the WHOLE BUNDLE, just the compressed downloaded data
						bundle.Unload(false);

						//TODO: We need a way/system to reference the bundle later so it can be cleaned up inbetween scene loads.
					};

					sceneAsync.allowSceneActivation = true;
				};
			}
		}
	}
}
