﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PSOBB.Unity;

namespace PSOBB
{
	public sealed class ServerGameFrameworkManager : DefaultGameFrameworkManager
	{
		/// <inheritdoc />
		protected override IEnumerable<IGameTickable> OrderTickables(IEnumerable<IGameTickable> tickables)
		{
			//Server tickables may not be order independent.
			return tickables.OrderBy(tickable =>
				{
					if(tickable.GetType().GetCustomAttribute<GameInitializableOrderingAttribute>() is GameInitializableOrderingAttribute attri)
					{
						return attri.Order;
					}
					else
						return GameInitializableOrderingAttribute.DefaultOrderValue;
				}) //ordering was added because sometimes we want certain tickables to run before others.
				.ToArray();
		}
	}
}
