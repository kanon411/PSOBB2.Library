using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	/// <summary>
	/// Response model to a character namequery request.
	/// </summary>
	[JsonObject]
	public sealed class CharacterNameQueryResponse : IResponseModel<CharacterNameQueryResponseCode>, ISucceedable
	{
		/// <summary>
		/// Optional name of the character from the request.
		/// </summary>
		[JsonProperty(Required = Required.AllowNull)]
		public string CharacterName { get; }

		/// <inheritdoc />
		[JsonRequired]
		[JsonProperty]
		public CharacterNameQueryResponseCode ResultCode { get; }

		/// <inheritdoc />
		[JsonIgnore]
		public bool isSuccessful => ResultCode == CharacterNameQueryResponseCode.Success;

		/// <inheritdoc />
		public CharacterNameQueryResponse(string characterName)
		{
			if(string.IsNullOrWhiteSpace(characterName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(characterName));

			CharacterName = characterName;
			ResultCode = CharacterNameQueryResponseCode.Success;
		}

		/// <inheritdoc />
		public CharacterNameQueryResponse(CharacterNameQueryResponseCode resultCode)
		{
			if(!Enum.IsDefined(typeof(CharacterNameQueryResponseCode), resultCode)) throw new InvalidEnumArgumentException(nameof(resultCode), (int)resultCode, typeof(CharacterNameQueryResponseCode));

			//We don't want success to be possible. It means we need to provide the actual data.
			if(resultCode == CharacterNameQueryResponseCode.Success)
				throw new ArgumentException($"Cannot provide success to response without providing optional success based data. Use other ctor.");

			ResultCode = resultCode;
		}

		//Serializer ctor
		public CharacterNameQueryResponse()
		{
			
		}
	}
}
