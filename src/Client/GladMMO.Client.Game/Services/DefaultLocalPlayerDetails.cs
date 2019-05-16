using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO
{
	public sealed class DefaultLocalPlayerDetails : ILocalPlayerDetails, IReadonlyLocalPlayerDetails
	{
		private Lazy<ObjectGuid> _localPlayerGuid;

		/// <inheritdoc />
		public ObjectGuid LocalPlayerGuid
		{
			get => _localPlayerGuid.Value;
			set => throw new NotSupportedException();
		}

		//TODO: Come up with a better way of storing entity data, without downcasting.
		/// <inheritdoc />
		public IEntityDataFieldContainer EntityData => FieldDataMap[LocalPlayerGuid];

		/// <summary>
		/// Entity data map used to access the entity data through <see cref="EntityData"/>
		/// </summary>
		private IReadonlyEntityGuidMappable<IEntityDataFieldContainer> FieldDataMap { get; }

		private ICharacterDataRepository CharacterDataRepo { get; }

		/// <inheritdoc />
		public DefaultLocalPlayerDetails(IReadonlyEntityGuidMappable<IEntityDataFieldContainer> fieldDataMap, [NotNull] ICharacterDataRepository characterDataRepo)
		{
			FieldDataMap = fieldDataMap ?? throw new ArgumentNullException(nameof(fieldDataMap));
			CharacterDataRepo = characterDataRepo ?? throw new ArgumentNullException(nameof(characterDataRepo));

			_localPlayerGuid = new Lazy<ObjectGuid>(() => new ObjectGuid(((ulong)characterDataRepo.CharacterId + ((ulong)EntityGuidMask.Player << 48))));
		}
	}
}
