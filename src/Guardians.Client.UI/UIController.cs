using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
		private IObjectModelValidator ModelValidator { get; }


		//Pulled from ASP Core MVC: https://github.com/aspnet/Mvc/blob/1c4b0fcdf38320b2f02c0bb7c31df5bd391ace07/src/Microsoft.AspNetCore.Mvc.WebApiCompatShim/ApiController.cs#L547
		/// <summary>
		/// Validates the given entity and adds the validation errors to the <see cref="ApiController.ModelState"/>
		/// under an empty prefix.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity to be validated.</typeparam>
		/// <param name="entity">The entity being validated.</param>
		protected ModelStateDictionary TryValidate<TEntity>(TEntity entity)
		{
			return TryValidate(entity, keyPrefix: string.Empty);
		}

		/// <summary>
		/// Validates the given entity and adds the validation errors to the <see cref="ApiController.ModelState"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity to be validated.</typeparam>
		/// <param name="entity">The entity being validated.</param>
		/// <param name="keyPrefix">
		/// The key prefix under which the model state errors would be added in the
		/// <see cref="ApiController.ModelState"/>.
		/// </param>
		private ModelStateDictionary TryValidate<TEntity>(TEntity entity, string keyPrefix)
		{
			ControllerContext context = new ControllerContext();

			ModelValidator.Validate(
				context, 
				validationState: null,
				prefix: keyPrefix,
				model: entity);

			return context.ModelState;
		}
	}
}
