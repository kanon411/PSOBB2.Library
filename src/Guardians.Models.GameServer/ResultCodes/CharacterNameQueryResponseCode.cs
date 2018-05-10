namespace Guardians
{
	/// <summary>
	/// Enumeration of potential results of a name query.
	/// </summary>
	public enum CharacterNameQueryResponseCode
	{
		/// <summary>
		/// Indicates success.
		/// </summary>
		Success = 0,

		/// <summary>
		/// Indicates that the id requested as unknown.
		/// </summary>
		UnknownIdError = 1,

		GeneralServerError = 2
	}
}