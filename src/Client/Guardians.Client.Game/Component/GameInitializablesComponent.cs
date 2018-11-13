using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class GameInitializablesComponent : MonoBehaviour
	{
		[Inject]
		private IReadOnlyCollection<IGameInitializable> Initializables { get; }

		[Inject]
		private ILog Logger { get; }

		private async Task Start()
		{
			try
			{
				var taskList = Initializables
					.Select(i => i.OnGameInitialized())
					.ToList();

				await Task.WhenAll(taskList)
					.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Encounter Exception In Game Initializables: {e.Message}\n\nStack: {e.StackTrace}");
				throw;
			}
		}
	}
}
