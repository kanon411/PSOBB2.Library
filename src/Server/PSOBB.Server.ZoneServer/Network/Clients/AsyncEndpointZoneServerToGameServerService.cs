﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace PSOBB
{
	public sealed class AsyncEndpointZoneServerToGameServerService : BaseAsyncEndpointService<IZoneServerToGameServerClient>, IZoneServerToGameServerClient
	{
		/// <inheritdoc />
		public AsyncEndpointZoneServerToGameServerService([NotNull] Task<string> futureEndpoint) 
			: base(futureEndpoint)
		{
		}

		/// <inheritdoc />
		public AsyncEndpointZoneServerToGameServerService([NotNull] Task<string> futureEndpoint, [NotNull] RefitSettings settings) 
			: base(futureEndpoint, settings)
		{
		}

		/// <inheritdoc />
		public async Task<ZoneServerNPCEntryCollectionResponse> GetNPCEntriesByMapId(int mapId)
		{
			return await (await GetService().ConfigureAwait(false)).GetNPCEntriesByMapId(mapId).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<ZoneServerCharacterLocationResponse> GetCharacterLocation(int characterId)
		{
			return await (await GetService().ConfigureAwait(false)).GetCharacterLocation(characterId).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task SaveCharacterLocation(ZoneServerCharacterLocationSaveRequest saveRequest)
		{
			await (await GetService().ConfigureAwait(false)).SaveCharacterLocation(saveRequest).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<ZoneServerWaypointQueryResponse> GetPathWaypoints(int pathId)
		{
			return await (await GetService().ConfigureAwait(false)).GetPathWaypoints(pathId).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task ReleaseActiveSession(int characterId)
		{
			await (await GetService().ConfigureAwait(false)).ReleaseActiveSession(characterId);
		}

		/// <inheritdoc />
		public async Task<ZoneServerTryClaimSessionResponse> TryClaimSession(ZoneServerTryClaimSessionRequest request)
		{
			return await (await GetService().ConfigureAwait(false)).TryClaimSession(request).ConfigureAwait(false);
		}

		//TODO: This should actually be in another service
		/// <inheritdoc />
		public async Task<int> GetAccountIdFromToken(string authToken)
		{
			return await (await GetService().ConfigureAwait(false)).GetAccountIdFromToken(authToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<ZoneServerRegisterationResponse> RegisterZoneServer(ZoneServerRegisterationRequest request)
		{
			return await (await GetService().ConfigureAwait(false)).RegisterZoneServer(request).ConfigureAwait(false);
		}
	}
}
