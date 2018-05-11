namespace Guardians
{
	/// <summary>
	/// Enumeration of all potential results/codes
	/// for a character creation operation.
	/// </summary>
	public enum CharacterCreationResponseCode
	{
		/// <summary>
		/// Indicates the creation was successful.
		/// </summary>
		Success = 0,

		//TODO: Add more fine grained information about failure.
		NameUnavailableError = 1
	}
}