﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Guardians
{
	public sealed class LoginFormView : MonoBehaviour, IUIView
	{
		/// <inheritdoc />
		public bool IsEnabled => RootGameObject.activeSelf;

		[Tooltip("Should be the root of the login form.")]
		[SerializeField]
		private GameObject RootGameObject;

		[Tooltip("The Username input field.")]
		[SerializeField]
		private InputField UsernameInputField;

		[Tooltip("The Password input field.")]
		[SerializeField]
		private InputField PasswordInputField;

		[Tooltip("The login button.")]
		[SerializeField]
		private Button LoginButton;

		void Awake()
		{
			LoginButton.interactable = false;
		}


		//TODO: Doc
		public void SetLoginButtonState(bool state)
		{
			LoginButton.interactable = state;
		}
	}
}