using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Contract for types that can indentify a Entity.
	/// </summary>
	[ProtoContract]
	[ProtoInclude(1, typeof(NetworkEntityGuid))]
	public interface IEntityIdentifiable
	{
		/// <summary>
		/// Represents the type of the entity.
		/// </summary>
		[ProtoIgnore]
		EntityType EntityType { get; }

		/// <summary>
		/// Represents the unique entity integer indentifier.
		/// </summary>
		[ProtoIgnore]
		int EntityId { get; }
	}
}
