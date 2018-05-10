namespace Guardians
{
	/// <summary>
	/// Enumeration of all possible response codes
	/// for <see cref="CharacterNameValidationResponse"/>
	/// </summary>
	public enum CharacterNameValidationResponseCode
	{
		/// <summary>
		/// Indicates that the name is valid.
		/// </summary>
		Success = 0,
		
		//TODO: Doc
		NameIsUnavailable = 1,

		NameContainsInvalidCharacters = 2,

		NameLengthIsInvalid = 3,
	}
}