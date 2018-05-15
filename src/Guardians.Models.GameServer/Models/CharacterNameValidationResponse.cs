﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Guardians
{
	/// <summary>
	/// Response model for name validation requests.
	/// </summary>
	[JsonObject]
	public sealed class CharacterNameValidationResponse : IResponseModel<CharacterNameValidationResponseCode>, ISucceedable
	{
		/// <inheritdoc />
		[JsonProperty]
		public CharacterNameValidationResponseCode ResultCode { get; }

		/// <inheritdoc />
		[JsonIgnore]
		public bool isSuccessful => ResultCode == CharacterNameValidationResponseCode.Success;

		/// <inheritdoc />
		public CharacterNameValidationResponse(CharacterNameValidationResponseCode resultCode)
		{
			if(!Enum.IsDefined(typeof(CharacterNameValidationResponseCode), resultCode)) throw new InvalidEnumArgumentException(nameof(resultCode), (int)resultCode, typeof(CharacterNameValidationResponseCode));

			ResultCode = resultCode;
		}
	}
}