using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Guardians
{
	public sealed class SceneManagementService : ISceneManager
	{
		[SerializeField]
		private event UnityAction _OnSceneLoaded;

		public event Action OnSceneLoaded
		{
			add
			{
				_OnSceneLoaded += () => value?.Invoke();
			}
			remove
			{
				//TODO: How should we support removal?
			}
		}

		[SerializeField]
		private UnityAction _OnBeforeSceneChange;

		public event Action OnBeforeSceneChange
		{
			add
			{
				_OnBeforeSceneChange += () => value?.Invoke();
			}
			remove
			{
				//TODO: How should we support removal?
			}
		}

		void Awake()
		{
			SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
		}

		private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			_OnBeforeSceneChange?.Invoke();
		}


		void OnDestroy()
		{
			SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
		}

		public void LoadLevel(int id, LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsync(id, mode).completed += operation =>
			{
				_OnBeforeSceneChange?.Invoke();
				operation.allowSceneActivation = true;
			};
		}

		public void LoadLevel(string name, LoadSceneMode mode)
		{
			SceneManager.LoadSceneAsync(name, mode).completed += operation =>
			{
				_OnBeforeSceneChange?.Invoke();
				operation.allowSceneActivation = true;
			};
		}
	}
}
