namespace Guardians
{
	/// <summary>
	/// Enumeration of all character session enter/creat responses.
	/// </summary>
	public enum CharacterSessionEnterResponseCode
	{
		/// <summary>
		/// Indicates that a session enter request was successful.
		/// </summary>
		Success = 0,

		/// <summary>
		/// Indicates that the character resource is in an active session
		/// already and cannot be used.
		/// </summary>
		CharacterSessionAlreadyActiveError = 1,

		/// <summary>
		/// Indicates that an invalid character id was provided
		/// as a reques to to create a session.
		/// </summary>
		InvalidCharacterIdError = 2,

		/// <summary>
		/// While this seems unlike/impossible it's possible that the account tied to a characterid
		/// may not have a session on the character (<see cref="CharacterSessionAlreadyActiveError"/>) but
		/// may be on another character.
		/// </summary>
		AccountAlreadyHasCharacterSession = 3,

		/// <summary>
		/// Result of an unknown server error.
		/// </summary>
		GeneralServerError = 4
	}
}