using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface IUIUnitFrame
	{
		UILabeledBar HealthBar { get; }

		UILabeledBar TechniquePointsBar { get; }

		IUIText UnitName { get; }

		IUIText UnitLevel { get; }
	}
}
