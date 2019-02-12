using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PSOBB
{
	/// <summary>
	/// The base controller Type for guardians ASP MVC controllers.
	/// </summary>
	public abstract class BaseGuardiansController : Controller
	{
		/// <summary>
		/// The logging service for the controller.
		/// </summary>
		protected ILogger<BaseGuardiansController> Logger { get; }

		/// <inheritdoc />
		protected BaseGuardiansController([FromServices] ILogger<BaseGuardiansController> logger)
		{
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;
		}
	}
}
