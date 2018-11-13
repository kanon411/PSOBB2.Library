using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
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

			//TODO: Verify that this has the component
			//Now we just pull the CharacterSlotUIElements off the slot
			//created and we can return it to consumers.
			return model.GetComponent<CharacterSlotUIElements>();
		}
	}
}
