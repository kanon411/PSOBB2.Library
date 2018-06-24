using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public abstract class UIController<TViewType> : MonoBehaviour
		where TViewType : class
	{
		[Inject]
		protected ILogger<UIController<TViewType>> Logger { get; }

		[Inject]
		protected TViewType View { get; }

		/// <summary>
		/// Service used for validating a model.
		/// </summary>
		[Inject]
		protected IObjectModelValidator ModelValidator { get; }
	}
}
