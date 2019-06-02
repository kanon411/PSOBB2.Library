﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public abstract class BaseGuardiansMonoBehaviour : MonoBehaviour
	{
		[Inject]
		protected ILog Logger { get; private set; }
	}
}
