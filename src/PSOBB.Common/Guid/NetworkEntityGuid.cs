﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;

namespace PSOBB
{
	//Format: 0xSSEE XXXX IIII IIII
	//[IIII IIII] unsigned 32bit unique identifier.
	//[X] unsued
	//[SS] Server ID. Will probably remain unimplemented for awhile until cross-server is needed.
	//[EE] Entity type. Reserves 255 indicies for possible entity types.
	/// <summary>
	/// Unique GUID identifier for network entites. Based on Blizzard's 64bit GUID implementation
	/// which encodes flags and uses masking to dervive information about a GUID.
	/// See: http://wowwiki.wikia.com/wiki/API_UnitGUID
	/// </summary>
	[JsonObject]
	[ProtoContract]
	public class NetworkEntityGuid : IEntityIdentifiable, IEquatable<NetworkEntityGuid>
	{
		//Sent over the network.
		/// <summary>
		/// Raw 64bit numerical representation of the GUID.
		/// </summary>
		[JsonProperty(PropertyName = "GuidValue")]
		[ProtoMember(1, IsRequired = true)]
		public ulong RawGuidValue { get; private set; } //We added a private set for unity3d JSON

		/// <summary>
		/// Represents an Empty or uninitialized <see cref="NetworkEntityGuid"/>.
		/// </summary>
		[JsonIgnore]
		[ProtoIgnore]
		public static NetworkEntityGuid Empty { get; } = new NetworkEntityGuid(0);

		/// <summary>
		/// Indicates the <see cref="EntityType"/> that this <see cref="NetworkEntityGuid"/> is for.
		/// </summary>
		[JsonIgnore]
		[ProtoIgnore]
		public EntityType EntityType => (EntityType)(byte)((RawGuidValue & 0x00FF000000000000) >> 48); //mask out to the EE (entity Type) and then shift it down to a byte

		/// <summary>
		/// Indiciates if the GUID is an empty or unitialized GUID.
		/// </summary>
		/// <returns></returns>
		[JsonIgnore]
		[ProtoIgnore]
		public bool isEmpty => RawGuidValue == 0;

		/// <summary>
		/// Indiciates the current GUID of the entity. This is the last chunk represents the actual ID without any type or identifying information.
		/// </summary>
		[JsonIgnore]
		[ProtoIgnore]
		public int EntityId => (int)(RawGuidValue & 0x00000000FFFFFFFF); //FFFF FFFF masks out everything but an unsigned integer. Casts to int. We waste bits this way but we gain considerable perf.

		public NetworkEntityGuid(ulong guidValue)
		{
			RawGuidValue = guidValue;
		}

		//For serializers
		public NetworkEntityGuid()
		{

		}

		/// <summary>
		/// Creates a <see cref="NetworkEntityGuid"/> building service.
		/// </summary>
		/// <returns>A non-null <see cref="NetworkEntityGuidBuilder"/> service.</returns>
		public NetworkEntityGuidBuilder CreateBuilder()
		{
			return new NetworkEntityGuidBuilder();
		}

		public static bool operator ==(NetworkEntityGuid guid1, NetworkEntityGuid guid2)
		{
			if (Object.ReferenceEquals(guid1, null))
			{
				if (Object.ReferenceEquals(guid2, null))
					return true;
				else
					return false;
			}

			return guid1.Equals(guid2);
		}

		public static bool operator !=(NetworkEntityGuid guid1, NetworkEntityGuid guid2)
		{
			return !(guid1 == guid2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NetworkEntityGuid))
				return false;

			return this.Equals((NetworkEntityGuid) obj);
		}

		public bool Equals(NetworkEntityGuid other)
		{
			if (Object.ReferenceEquals(other, null))
				return false;
			else
				return other.RawGuidValue == this.RawGuidValue;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return RawGuidValue.GetHashCode();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{EntityType}:{EntityId}";
		}
	}
}
