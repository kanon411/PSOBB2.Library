using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class DefaultLocalPlayerDetails : ILocalPlayerDetails, IReadonlyLocalPlayerDetails
	{
		/// <inheritdoc />
		public NetworkEntityGuid LocalPlayerGuid { get; set; }

		//TODO: Come up with a better way of storing entity data, without downcasting.
		/// <inheritdoc />
		public IEntityDataFieldContainer<EntityDataFieldType> EntityData => (IEntityDataFieldContainer<EntityDataFieldType>)FieldDataMap[LocalPlayerGuid];

		/// <summary>
		/// Entity data map used to access the entity data through <see cref="EntityData"/>
		/// </summary>
		private IReadonlyEntityGuidMappable<IEntityDataFieldContainer> FieldDataMap { get; }

		/// <inheritdoc />
		public DefaultLocalPlayerDetails(IReadonlyEntityGuidMappable<IEntityDataFieldContainer> fieldDataMap)
		{
			FieldDataMap = fieldDataMap ?? throw new ArgumentNullException(nameof(fieldDataMap));
		}

		private DefaultLocalPlayerDetails()
		{
			
		}
	}
}
