using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PSOBB
{
	public interface IMovementDirectionSettable
	{
		Vector2 Direction { get; set; }
	}
}
