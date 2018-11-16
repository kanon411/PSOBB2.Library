﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Generic base-type for any model that is coupled/associated with an Entity.
	/// Specificially with the <see cref="NetworkEntityGuid"/>
	/// </summary>
	/// <typeparam name="TDataType"></typeparam>
	[ProtoContract]
	public class EntityAssociatedData<TDataType> : IEntityGuidContainer
	{
		/// <summary>
		/// The GUID of the entity.
		/// </summary>
		[ProtoMember(1, IsRequired = true)]
		public NetworkEntityGuid EntityGuid { get; private set; }

		[ProtoMember(2)]
		public TDataType Data { get; private set; }

		/// <inheritdoc />
		public EntityAssociatedData([NotNull] NetworkEntityGuid entityGuid, [NotNull] TDataType data)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			Data = data;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected EntityAssociatedData()
		{
			
		}
	}
}