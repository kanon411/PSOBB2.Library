using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Guardians
{
	public sealed class CharacterSelectionView : MonoBehaviour, IUIView
	{
		[Serializable]
		public struct ButtonWithText
		{
			[SerializeField]
			public UnityEngine.UI.Button CharacterButton;

			[SerializeField]
			public UnityEngine.UI.Text CharacterNameLabel;
		}

		/// <inheritdoc />
		public bool IsEnabled => RootViewObject.activeInHierarchy;

		[Tooltip("The root gameobject for the error form.")]
		[SerializeField]
		private GameObject RootViewObject;

		[SerializeField]
		private ButtonWithText CharacterEntry;

		[SerializeField]
		private Image CharacterDisplayImage;

		public void SetCharacterSlot(string characterName, Action onCharacterSlotClickedCallback)
		{
			if(onCharacterSlotClickedCallback == null) throw new ArgumentNullException(nameof(onCharacterSlotClickedCallback));
			if(string.IsNullOrEmpty(characterName)) throw new ArgumentException("Value cannot be null or empty.", nameof(characterName));

			//TODO: When we actually have multiple character slots we should handle it. Right now we only have 1.
			CharacterEntry.CharacterNameLabel.text = characterName;
			CharacterEntry.CharacterButton.onClick.AddListener(onCharacterSlotClickedCallback.Invoke);
		}

		public void SetCharacterPreview(Texture2D texture)
		{
			CharacterDisplayImage.material.mainTexture = texture ?? throw new ArgumentNullException(nameof(texture));
			CharacterDisplayImage.SetMaterialDirty();
		}
	}
}
