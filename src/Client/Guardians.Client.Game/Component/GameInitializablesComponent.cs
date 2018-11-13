using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SceneJect.Common;
using UnityEngine;

namespace Guardians
{
	[Injectee]
	public sealed class GameInitializablesComponent : MonoBehaviour
	{
		[Inject]
		private IReadOnlyCollection<IGameInitializable> Initializables { get; }

		private async Task Start()
		{
			var taskList = Initializables
				.Select(i => i.OnGameInitialized())
				.ToList();

			await Task.WhenAll(taskList)
				.ConfigureAwait(false);
		}
	}
}
