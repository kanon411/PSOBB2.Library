using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public sealed class UILabeledBar
	{
		public IUIText BarCurrent { get; }

		public IUIText BarMaximum { get; }

		public IUIFillableImage BarFillable { get; }

		/// <inheritdoc />
		public UILabeledBar([NotNull] IUIText barCurrent, [NotNull] IUIText barMaximum, [NotNull] IUIFillableImage barFillable)
		{
			BarCurrent = barCurrent ?? throw new ArgumentNullException(nameof(barCurrent));
			BarMaximum = barMaximum ?? throw new ArgumentNullException(nameof(barMaximum));
			BarFillable = barFillable ?? throw new ArgumentNullException(nameof(barFillable));
		}
	}
}
