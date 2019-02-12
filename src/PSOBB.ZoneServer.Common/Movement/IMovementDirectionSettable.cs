using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Guardians
{
	public interface IMovementDirectionSettable
	{
		Vector2 Direction { get; set; }
	}
}
