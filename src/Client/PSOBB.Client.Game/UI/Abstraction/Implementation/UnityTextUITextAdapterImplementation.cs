﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSOBB;
using UnityEngine.UI;

namespace PSOBB
{
	public sealed class UnityTextUITextAdapterImplementation : IUIText
	{
		private UnityEngine.UI.Text UnityText { get; }

		/// <inheritdoc />
		public UnityTextUITextAdapterImplementation([NotNull] Text unityText)
		{
			UnityText = unityText ?? throw new ArgumentNullException(nameof(unityText));
		}

		/// <inheritdoc />
		public string Text
		{
			get => UnityText.text;
			set => UnityText.text = value;
		}
	}
}
