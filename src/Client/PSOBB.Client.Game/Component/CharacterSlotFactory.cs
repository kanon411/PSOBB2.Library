using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PSOBB
{
	//To simplify the implenmentation of this feature we will have this be a scene component
	//While this is rare, and avoidence to this is required by design of this project, there are
	//rare instances where it would just be too difficult and cumbersome to deal with this out-of-engine.
	public sealed class CharacterSlotFactory : MonoBehaviour, IFactoryCreatable<CharacterSlotUIElements, EmptyFactoryContext>
	{
		[Tooltip("The prefab for the slot.")]
		[SerializeField]
		private GameObject CharacterSlotPrefab;

		[SerializeField]
		private GameObject CharacterListContainer;

		[SerializeField]
		private ToggleGroup Group;

		[SerializeField]
		private UnityEvent OnCharacterSlotToggled;

		/// <inheritdoc />
		public CharacterSlotUIElements Create(EmptyFactoryContext context)
		{
			//Generates the slot and adds it to the UI container
			GameObject model = Instantiate<GameObject>(CharacterSlotPrefab);
			model.layer = CharacterListContainer.layer;
			model.transform.SetParent(CharacterListContainer.transform, false);
			model.transform.localScale = CharacterSlotPrefab.transform.localScale;
			model.transform.localPosition = CharacterSlotPrefab.transform.localPosition;
			model.transform.localRotation = CharacterSlotPrefab.transform.localRotation;

			//We need to get the Toggle to add it to the toggle group
			//so Unity can manage the annoying parts of Toggles
			Toggle toggle = model.GetComponent<Toggle>();
			toggle.group = Group;

			toggle.onValueChanged.AddListener(arg0 => OnCharacterSlotToggled?.Invoke());

			//TODO: Verify that this has the component
			//Now we just pull the CharacterSlotUIElements off the slot
			//created and we can return it to consumers.
			return model.GetComponent<CharacterSlotUIElements>();
		}
	}
}
