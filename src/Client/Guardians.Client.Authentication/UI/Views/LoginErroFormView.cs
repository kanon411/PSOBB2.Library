using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Guardians
{
    public sealed class LoginErroFormView : MonoBehaviour, IErrorUIView
	{
		[Serializable]
		private class ErrorAction : UnityEvent<string> { }

		/// <inheritdoc />
		public bool IsEnabled => RootViewObject.activeSelf;

		[Tooltip("The root gameobject for the error form.")]
		[SerializeField]
		private GameObject RootViewObject;

		[Tooltip("Fired when an error event is recieved. (Do not activate form; already implictly does that.")]
		[SerializeField]
		private ErrorAction OnErrorEvent;

		/// <inheritdoc />
		public void SetError(string errorMessage)
		{
			if(errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

			OnErrorEvent?.Invoke(errorMessage);
			RootViewObject.SetActive(true);
		}

		/// <summary>
		/// Closed the error form.
		/// </summary>
		public void CloseErrorForm()
		{
			RootViewObject.SetActive(false);
		}
	}
}
