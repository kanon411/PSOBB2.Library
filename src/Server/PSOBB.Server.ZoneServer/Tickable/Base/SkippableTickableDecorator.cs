using System;
using System.Collections.Generic;
using System.Text;

namespace Guardians
{
	public sealed class SkippableTickableDecorator : IGameTickable
	{
		private IGameTickable TickableReference { get; }

		private ITickableSkippable SkippableReference { get; }

		/// <inheritdoc />
		public SkippableTickableDecorator([NotNull] IGameTickable tickableReference, [NotNull] ITickableSkippable skippableReference)
		{
			TickableReference = tickableReference ?? throw new ArgumentNullException(nameof(tickableReference));
			SkippableReference = skippableReference ?? throw new ArgumentNullException(nameof(skippableReference));

			if(Object.ReferenceEquals(tickableReference, skippableReference))
				throw new ArgumentException($"Both the parameters MUST be the same reference.");
		}

		/// <inheritdoc />
		public void Tick()
		{
			//Just skip, avoiding a bit of overhead.
			if(SkippableReference.IsTickableSkippable)
				return;

			TickableReference.Tick();
		}
	}
}
