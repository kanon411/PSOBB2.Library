using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;

namespace Guardians
{
	public abstract class BaseExternalComponent
	{
		/// <summary>
		/// The component logger.
		/// </summary>
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected BaseExternalComponent([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
	}
}
