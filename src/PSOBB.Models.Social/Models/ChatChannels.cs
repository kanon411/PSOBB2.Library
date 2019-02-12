using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Enumeration of chat channel types.
	/// </summary>
	public enum ChatChannels
	{
		/// <summary>
		/// Internal messaging.
		/// </summary>
		Internal = 0,

		/// <summary>
		/// The chat channel for zone chat.
		/// </summary>
		ZoneChannel = 1,

		/// <summary>
		/// The chat channel for guild chat.
		/// </summary>
		GuildChannel = 2,
	}
}
