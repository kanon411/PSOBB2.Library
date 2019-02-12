using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public sealed class DebugShowMouseUnlockComponent : MonoBehaviour
	{
		void Update()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
