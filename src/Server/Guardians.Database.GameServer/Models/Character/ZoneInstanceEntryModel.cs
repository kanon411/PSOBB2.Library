using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Guardians
{
	[Table("zone_endpoints")]
	public class ZoneInstanceEntryModel
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ZoneId { get; private set; }

		[Required]
		public Guid ZoneGuid { get; private set; }

		/// <summary>
		/// The type of the registered zone.
		/// Indicates if the zone itself is a static zone, not being transient.
		/// (Dynamic zones are like instances that are spun up and spun down)
		/// </summary>
		[Required]
		public GameZoneType ZoneType { get; private set; }

		/// <summary>
		/// The address (IP or even domain) of the instance/zone.
		/// </summary>
		[Required]
		public string ZoneServerAddress { get; private set; }

		[Required]
		public short ZoneServerPort { get; private set; }

		//TODO: Add zone type
		//TODO: Health checks?

		/// <inheritdoc />
		public ZoneInstanceEntryModel(Guid zoneGuid, GameZoneType zoneType, string zoneServerAddress, short zoneServerPort)
		{
			if(!Enum.IsDefined(typeof(GameZoneType), zoneType)) throw new InvalidEnumArgumentException(nameof(zoneType), (int)zoneType, typeof(GameZoneType));
			if(string.IsNullOrWhiteSpace(zoneServerAddress)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(zoneServerAddress));
			if(zoneServerPort < 0) throw new ArgumentOutOfRangeException(nameof(zoneServerPort));
			if(zoneGuid == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(zoneGuid), "A Zone Guid must not be the empty guid.");

			ZoneGuid = zoneGuid;
			ZoneType = zoneType;
			ZoneServerAddress = zoneServerAddress;
			ZoneServerPort = zoneServerPort;
		}

		private ZoneInstanceEntryModel()
		{
			
		}
	}
}
