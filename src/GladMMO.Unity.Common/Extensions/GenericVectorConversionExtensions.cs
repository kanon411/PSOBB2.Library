using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore;
using UnityEngine;

namespace GladMMO
{
	public static class GenericVectorConversionExtensions
	{
		public static Vector3 ToUnityVector(this Vector3<float> freecraftVector)
		{
			//HelloKitty: So, in WoW compared to Unity3D the coordinate system is as follows
			//Z is actually up in WoW.
			return new Vector3(freecraftVector.X, freecraftVector.Z, freecraftVector.Y);
		}
	}
}
