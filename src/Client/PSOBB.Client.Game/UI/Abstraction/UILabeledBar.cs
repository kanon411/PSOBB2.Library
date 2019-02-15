using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class UILabeledBar
	{
		public IUIText BarText { get; }

		public IUIFillableImage BarFillable { get; }

		/// <inheritdoc />
		public UILabeledBar([NotNull] IUIText barText, [NotNull] IUIFillableImage barFillable)
		{
			BarText = barText ?? throw new ArgumentNullException(nameof(barText));
			BarFillable = barFillable ?? throw new ArgumentNullException(nameof(barFillable));
		}
	}
}
