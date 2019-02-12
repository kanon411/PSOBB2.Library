using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PSOBB
{
	public sealed class CharacterSlotUIElements : MonoBehaviour
	{
		[SerializeField]
		private UnityTextUITextAdapter _CharacterNameText;

		[SerializeField]
		private UnityToggleUIToggleAdapter _CharacterSlotToggle;

		/// <summary>
		/// The character name UI Text.
		/// </summary>
		public IUIText CharacterNameText => _CharacterNameText;

		/// <summary>
		/// The toggle element for the character slot.
		/// </summary>
		public IUIToggle CharacterSlotToggle => _CharacterSlotToggle;
	}
}
