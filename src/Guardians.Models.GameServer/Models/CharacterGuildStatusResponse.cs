using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	public sealed class CharacterGuildStatusResponse : IResponseModel<CharacterGuildStatusResponseCode>, ISucceedable
	{
		/// <inheritdoc />
		[JsonProperty]
		public CharacterGuildStatusResponseCode ResultCode { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public bool isSuccessful => ResultCode == CharacterGuildStatusResponseCode.Success;

		[JsonProperty]
		public int GuildId { get; private set; }

		/// <inheritdoc />
		public CharacterGuildStatusResponse(CharacterGuildStatusResponseCode resultCode)
		{
			//TODO: Verify that it's not success.
			ResultCode = resultCode;
		}

		/// <inheritdoc />
		public CharacterGuildStatusResponse(int guildId)
		{
			if(guildId <= 0) throw new ArgumentOutOfRangeException(nameof(guildId), "Should use other ctor if a guild does not exist for the character.");

			ResultCode = CharacterGuildStatusResponseCode.Success;
			GuildId = guildId;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private CharacterGuildStatusResponse()
		{
			
		}
	}

	public enum CharacterGuildStatusResponseCode
	{
		/// <summary>
		/// Indicates the creation was successful.
		/// </summary>
		Success = 1,

		/// <summary>
		/// Indicates that the character is not in a guild.
		/// </summary>
		NoGuild = 2,

		/// <summary>
		/// Indicates an unknown server error.
		/// </summary>
		GeneralServerError = 3,
	}
}
