using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Guardians.Database;

namespace Guardians
{
	/// <summary>
	/// Table that represents an actual NPC entry that exists in the world.
	/// An instance of a defined <see cref="NPCTemplateModel"/>.
	/// </summary>
	[Table("npc_entry")]
	public class NPCEntryModel
	{
		/// <summary>
		/// Primary key for the NPC template.
		/// Must be unique.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int EntryId { get; private set; }

		/// <summary>
		/// Defines the template data from which this entry/instance of a creature
		/// should be created from.
		/// </summary>
		[Required]
		[ForeignKey(nameof(NpcTemplate))]
		public int NpcTemplateId { get; private set; }

		//Navigation property
		/// <summary>
		/// The NPC template.
		/// </summary>
		public virtual NPCTemplateModel NpcTemplate { get; private set; }

		/// <summary>
		/// The spawn position of the creature/NPC.
		/// </summary>
		[Required]
		public Vector3<float> SpawnPosition { get; private set; }

		/// <summary>
		/// The initial Y-axis orientation/rotation of the NPC/Creature when spawned.
		/// Especially important for stationary NPCs.
		/// </summary>
		[Required]
		public float InitialOrientation { get; private set; }

		/// <summary>
		/// The map/zone that this creature exists in.
		/// (Each map/zone has an origin and the <see cref="SpawnPosition"/> is based on that).
		/// </summary>
		[Required]
		public int MapId { get; private set; }

		/// <inheritdoc />
		public NPCEntryModel(int npcTemplateId, Vector3<float> spawnPosition, float initialOrientation, int mapId)
		{
			if(mapId <= 0) throw new ArgumentOutOfRangeException(nameof(mapId));
			if(npcTemplateId <= 0) throw new ArgumentOutOfRangeException(nameof(npcTemplateId));

			NpcTemplateId = npcTemplateId;
			SpawnPosition = spawnPosition ?? throw new ArgumentNullException(nameof(spawnPosition));
			MapId = mapId;
			InitialOrientation = initialOrientation;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected NPCEntryModel()
		{

		}
	}
}
