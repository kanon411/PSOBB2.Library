using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	/// <summary>
	/// Simplified creatable type for <see cref="GameInitializableRegisterationAutofacModule"/>
	/// </summary>
	public sealed class GameInitializablesRegisterModule : AutofacBasedDependencyRegister<GameInitializableRegisterationAutofacModule>
	{

	}
}
