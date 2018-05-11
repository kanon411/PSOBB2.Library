using System;
using System.Collections.Generic;
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

		public ZoneInstanceEntryModel()
		{
			
		}
	}
}
