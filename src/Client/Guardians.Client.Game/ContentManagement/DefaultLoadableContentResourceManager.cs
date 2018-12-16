using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace Guardians
{
	//TODO: We should do some threading and safety stuff.
	public sealed class DefaultLoadableContentResourceManager : ILoadableContentResourceManager, IDisposable
	{
		private IContentServerServiceClient ContentClient { get; }

		private IReadonlyAuthTokenRepository AuthTokenRepo { get; }

		private ILog Logger { get; }

		//We should only tocuh this on the main thread, including cleanup and updating it.
		private Dictionary<long, ReferenceCountedPrefabContentResourceHandle> ResourceHandleCache { get; }

		/// <inheritdoc />
		public DefaultLoadableContentResourceManager(
			[NotNull] IContentServerServiceClient contentClient, 
			[NotNull] IReadonlyAuthTokenRepository authTokenRepo,
			[NotNull] ILog logger)
		{
			//TODO: We haven't implemented the refcounted cleanup. We ref count, but don't yet dispose.
			ProjectVersionStage.AssertAlpha();

			ContentClient = contentClient ?? throw new ArgumentNullException(nameof(contentClient));
			AuthTokenRepo = authTokenRepo ?? throw new ArgumentNullException(nameof(authTokenRepo));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));

			ResourceHandleCache = new Dictionary<long, ReferenceCountedPrefabContentResourceHandle>();
		}

		/// <inheritdoc />
		public bool IsAvatarResourceAvailable(long avatarId)
		{
			if(avatarId < 0) throw new ArgumentOutOfRangeException(nameof(avatarId));

			return ResourceHandleCache.ContainsKey(avatarId);
		}

		/// <inheritdoc />
		public async Task<IPrefabContentResourceHandle> LoadAvatarPrefabAsync(long avatarId)
		{
			//If it's already available, we can just return immediately
			if(IsAvatarResourceAvailable(avatarId))
				return TryLoadAvatarPrefab(avatarId);

			ContentDownloadURLResponse downloadUrlResponse = await ContentClient.RequestAvatarDownloadUrl(avatarId, AuthTokenRepo.RetrieveWithType())
				.ConfigureAwait(false);

			//TODO: Handle failure
			TaskCompletionSource<IPrefabContentResourceHandle> completionSource = new TaskCompletionSource<IPrefabContentResourceHandle>();

			//Asset bundle requests can sadly only happen on the main thread, so we must join the main thread.
			await new UnityYieldAwaitable();

			//TODO: We should handle caching, versioning and etc here.
			UnityWebRequestAsyncOperation asyncOperation = UnityWebRequestAssetBundle.GetAssetBundle(downloadUrlResponse.DownloadURL, 0).SendWebRequest();

			//TODO: We should render these operations to the loading screen UI.
			asyncOperation.completed += operation =>
			{
				var bundleHandle = new ReferenceCountedPrefabContentResourceHandle(DownloadHandlerAssetBundle.GetContent(asyncOperation.webRequest));

				//We should cache the bundle so it can be looked up non-async and loading faster in the future
				//then claim a reference
				this.ResourceHandleCache[avatarId] = bundleHandle;
				bundleHandle.ClaimReference();
				completionSource.SetResult(bundleHandle);

				//TODO: Handle failure
			};

			return await completionSource.Task
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public IPrefabContentResourceHandle TryLoadAvatarPrefab(long avatarId)
		{
			if(!IsAvatarResourceAvailable(avatarId))
				throw new InvalidOperationException($"Cannot load AvatarId: {avatarId} from memory. Call {nameof(LoadAvatarPrefabAsync)} if not already in memory.");

			//Important to claim reference, since this is ref counted.
			var handle = ResourceHandleCache[avatarId];
			handle.ClaimReference();

			return handle;
		}

		//TODO: Dispose is never called, but it should be.
		/// <inheritdoc />
		public void Dispose()
		{
			if(Logger.IsInfoEnabled)
				Logger.Info("Disposing of asset bundles.");

			//We need to dispose of all
			//assetbundle resources regardless of their refcount
			foreach(var entry in ResourceHandleCache.Values)
				entry.Bundle.Unload(true);
		}
	}
}
