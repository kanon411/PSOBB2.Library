﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using ProtoBuf;

namespace Guardians
{
	[ProtoContract]
	public sealed class EntityCreationData : IEntityGuidContainer
	{
		/// <summary>
		/// The GUID of the entity.
		/// </summary>
		[ProtoMember(1, IsRequired = true)]
		public NetworkEntityGuid EntityGuid { get; }

		/// <summary>
		/// The initial movement data to use to
		/// create the entity.
		/// This is sent because we need to know position and movement details to
		/// create the entity and move it until movement updates are recieved.
		/// </summary>
		[ProtoMember(2, IsRequired = true)]
		public MovementInformation InitialMovementData { get; }

		/// <inheritdoc />
		public EntityCreationData([NotNull] NetworkEntityGuid entityGuid, [NotNull] MovementInformation initialMovementData)
		{
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
			InitialMovementData = initialMovementData ?? throw new ArgumentNullException(nameof(initialMovementData));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected EntityCreationData()
		{
			
		}
	}
}