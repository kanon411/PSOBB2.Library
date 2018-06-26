using System;
using Common.Logging;
using JetBrains.Annotations;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public abstract class UIController<TViewType> : MonoBehaviour
		where TViewType : class
	{
		[Inject]
		protected ILog Logger { get; }

		[Inject]
		protected TViewType View { get; }

		[Inject]
		protected IErrorUIView ErrorView { get; }
	}
}
