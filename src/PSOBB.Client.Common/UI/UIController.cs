using System;
using Common.Logging;
using JetBrains.Annotations;
using SceneJect.Common;
using UnityEngine;

namespace PSOBB
{
	[Injectee]
	public abstract class UIController<TViewType> : MonoBehaviour
		where TViewType : class
	{
		[Inject]
		protected ILog Logger { get; private set; }

		[Inject]
		protected TViewType View { get; private set; }

		[Inject]
		protected IErrorUIView ErrorView { get; private set; }
	}
}
