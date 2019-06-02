using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PSOBB
{
	[AdditionalRegisterationAs(typeof(IMovementInputChangedEventSubscribable))]
	[SceneTypeCreate(GameSceneType.DefaultLobby)]
	public sealed class MovementInputBroadcastingTickable : IGameTickable, IMovementInputChangedEventSubscribable
	{
		/// <inheritdoc />
		public event EventHandler<MovementInputChangedEventArgs> OnMovementInputDataChanged;

		private float LastHoritzontalInput { get; set; }

		private float LastVerticalInput { get; set; }

		/// <inheritdoc />
		public void Tick()
		{
			bool changed = false;

			float horizontal = Input.GetAxisRaw("Horizontal");

			if(Math.Abs(LastHoritzontalInput - horizontal) > 0.005f)
			{
				changed = true;
				LastHoritzontalInput = horizontal;
			}

			float vertical = Input.GetAxisRaw("Vertical");

			if(Math.Abs(LastVerticalInput - vertical) > 0.005f)
			{
				changed = true;
				LastVerticalInput = vertical;
			}

			//If the input has changed we should dispatch to anyone interested.
			if(changed)
				OnMovementInputDataChanged?.Invoke(this, new MovementInputChangedEventArgs(vertical, horizontal));
		}
	}
}