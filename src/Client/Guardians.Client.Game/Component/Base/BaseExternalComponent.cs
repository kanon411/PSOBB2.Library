using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;

namespace Guardians
{
	public abstract class BaseExternalComponent<TInitializationContext>
	{
		/// <summary>
		/// The component logger.
		/// </summary>
		protected ILog Logger { get; }

		/// <summary>
		/// True if <see cref="Initialize"/> has been called.
		/// </summary>
		protected bool isInitialized { get; private set; }

		/// <summary>
		/// Interanl component locking object.
		/// </summary>
		protected readonly object SyncObj = new object();

		/// <inheritdoc />
		protected BaseExternalComponent([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void Initialize(TInitializationContext context)
		{
			//TODO: Should we allow multiple calls??
			isInitialized = true;
			OnInitialization(context);
		}

		/// <summary>
		/// Called when <see cref="Initialize"/> is called publicy.
		/// Should be used to initialize the component with external non-injectable dependencies.
		/// </summary>
		/// <param name="context">The context for initialization.</param>
		protected abstract void OnInitialization(TInitializationContext context);

		protected void ThrowIfNotInitialized()
		{
			if(!isInitialized)
				throw new InvalidOperationException($"{nameof(Initialize)} was never called. Component must be initialized.");
		}
	}
}
