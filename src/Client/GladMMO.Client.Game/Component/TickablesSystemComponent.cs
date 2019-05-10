using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using SceneJect.Common;
using UnityEngine;

namespace GladMMO
{
	[Injectee]
	public sealed class TickablesSystemComponent : MonoBehaviour
	{
		[Inject]
		private IReadOnlyCollection<IGameTickable> GameTickables { get; set; }

		[Inject]
		private ILog Logger { get; set; }

		void Update()
		{
			if(GameTickables == null || GameTickables.Count == 0)
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"No gametickables; engine skipping tickables.");

				return;
			}

			//We just tick all tickables, they should be order independent
			//This moves the game simulation forward more or less, many things are scheduled to occur
			//via the main game loop on the main game thread
			foreach(var tickable in GameTickables)
			{
				try
				{
					tickable.Tick();
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Encountered Exception in Main GameLoop: {e.Message} \n\n Stack: {e.StackTrace}");
				}
			}
		}
	}
}
