using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Guardians
{
	public static class ControllerTestsHelpers
	{
		public static T GetActionResultObject<T>(IActionResult result)
		{
			if(result == null) throw new ArgumentNullException(nameof(result));

			ObjectResult objectResult = (result as ObjectResult);

			if(objectResult?.Value == null) throw new InvalidOperationException($"Failed to get object Type: {typeof(T).Name} from IActionResult.");

			if(!typeof(T).IsAssignableFrom(objectResult.Value.GetType()))
				throw new InvalidOperationException($"Result objects is not of Type: {typeof(T).Name} was Type: {objectResult.Value.GetType().Name}");

			return (T)objectResult.Value;
		}
	}
}
