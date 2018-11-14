using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Guardians
{
    public interface IAvatarTextureQueryable
	{
		Task<Texture2D> GetAvatarByCharacterId(int characterId);

		Task<Texture2D> GetAvatarByCharacterName(string characterName);
	}
}
