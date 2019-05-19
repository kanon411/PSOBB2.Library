using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;

namespace GladMMO.Services
{
	public interface IGroupUnitFrameIssuable
	{
		/// <summary>
		/// Attempts to claim a group unitframe for
		/// the provided Player <see cref="guid"/>.
		/// </summary>
		/// <param name="guid">The guid of the player.</param>
		/// <returns>True if a unitframe could be claimed.</returns>
		bool TryClaimUnitFrame(ObjectGuid guid);

		/// <summary>
		/// Attempts to release a claimed unitframe
		/// for the provided Player <see cref="guid"/>.
		/// </summary>
		/// <param name="guid">The guid of the player.</param>
		/// <returns>True if a unitframe was claimed and successfully released for the player.</returns>
		bool TryReleaseUnitFrame(ObjectGuid guid);
	}
}
