using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Guardians
{
	public sealed class CharacterSlotUIElements : MonoBehaviour
	{
		[SerializeField]
		private UnityTextUITextAdapter _CharacterNameText;

		[SerializeField]
		private UnityButtonUIButtonAdapter _CharacterSlotButton;

		/// <summary>
		/// The character name UI Text.
		/// </summary>
		public IUIText CharacterNameText => _CharacterNameText;

		/// <summary>
		/// The button element for the character slot.
		/// </summary>
		public IUIButton CharacterSlotButton => _CharacterSlotButton;
	}
}
