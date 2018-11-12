using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Guardians
{
	public abstract class BaseUnityUIAdapter<TAdaptedUnityEngineType, TAdaptedToType> : SerializedMonoBehaviour, IUIAdapterRegisterable
	{
		static BaseUnityUIAdapter()
		{
			if(typeof(TAdaptedToType) == typeof(TAdaptedUnityEngineType))
				throw new InvalidOperationException($"Type: BaseUnityUIAdapter<{typeof(TAdaptedUnityEngineType).Name}, {typeof(TAdaptedToType).Name}> must not have the same parameter for both generic type parameters.");

			//TODO: Check that TAdaptedUnityEngineType is in the Unity namespace.
		}

		[SerializeField]
		private TAdaptedUnityEngineType _UnityUIObject;

		/// <summary>
		/// The Unity engine UI object being adapted.
		/// </summary>
		protected TAdaptedUnityEngineType UnityUIObject => _UnityUIObject;

		[Tooltip("Used to determine wiring for UI dependencies.")]
		[SerializeField]
		private UnityUIRegisterationKey _RegisterationKey;

		/// <summary>
		/// The registeration key for the adapted UI element.
		/// </summary>
		public UnityUIRegisterationKey RegisterationKey => _RegisterationKey;

		/// <inheritdoc />
		public Type UISerivdeType => typeof(TAdaptedToType);

		[Button]
		public void TryInitializeAdaptedObject()
		{
			TAdaptedUnityEngineType obj = GetComponent<TAdaptedUnityEngineType>();

			if(obj == null)
				throw new InvalidOperationException($"Failed to find {typeof(TAdaptedUnityEngineType).Name} on GameObject: {name}.");

			_UnityUIObject = obj;
		}
	}
}
