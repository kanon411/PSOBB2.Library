using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Packet sent when object visibility changes.
	/// Such as a player has come into view or a player 
	/// </summary>
	[ProtoContract]
	[GamePayload(GamePayloadOperationCode.EntityVisibilityChange)]
	public sealed class NetworkObjectVisibilityChangeEventPayload : GameServerPacketPayload
	{
		//TODO: We should also include any initialization stuff, such as update values or initial movement data.
		[ProtoMember(1)]
		private readonly NetworkEntityGuid[] _NowVisibleEntities;

		[ProtoMember(2)]
		private readonly NetworkEntityGuid[] _OutOfRangeEntities;

		/// <summary>
		/// List of now newly visible entites.
		/// </summary>
		[ProtoIgnore]
		public IReadOnlyCollection<NetworkEntityGuid> NowVisibleEntities => _NowVisibleEntities;

		/// <summary>
		/// List of now out-of-range entities.
		/// </summary>
		[ProtoIgnore]
		public IReadOnlyCollection<NetworkEntityGuid> OutOfRangeEntities => _OutOfRangeEntities;

		/// <inheritdoc />
		public NetworkObjectVisibilityChangeEventPayload(NetworkEntityGuid[] nowVisibleEntities, NetworkEntityGuid[] outOfRangeEntities)
		{
			_NowVisibleEntities = nowVisibleEntities ?? throw new ArgumentNullException(nameof(nowVisibleEntities));
			_OutOfRangeEntities = outOfRangeEntities ?? throw new ArgumentNullException(nameof(outOfRangeEntities));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected NetworkObjectVisibilityChangeEventPayload()
		{
			
		}
	}
}
