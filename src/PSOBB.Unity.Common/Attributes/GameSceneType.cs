using System;

namespace PSOBB
{
	/// <summary>
	/// Enumeration of scene types for a game initializable.
	/// </summary>
	public enum GameSceneType
	{
		TitleScreen = 0,

		//TODO: Remove this at some point. 
		[Obsolete("ZoneGameScene isn't a thing anymore. In PSOBB there are lobby and there are game. Maybe other types too.")]
		ZoneGameScene = 1,
		
		CharacterSelection = 2,

		//TODO: Remove this at some point.
		[Obsolete("WorldDownloadingScreen does not exist in PSOBB. See PreZoneBurstingScreen instead.")]
		WorldDownloadingScreen = 3,

		PreZoneBurstingScreen = 4,

		/// <summary>
		/// The default lobby type for PSOBB.
		/// </summary>
		DefaultLobby = 5,
	}
}
