using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Guardians
{
	public sealed class InterestDequeueSetCommand
	{
		/// <summary>
		/// The dequeueable.
		/// </summary>
		private IEntityInterestDequeueable Dequeueable { get; }

		/// <summary>
		/// The interest set.
		/// </summary>
		private IEntityInterestSet InterestSet { get; }

		/// <inheritdoc />
		public InterestDequeueSetCommand([NotNull] IEntityInterestDequeueable dequeueable, [NotNull] IEntityInterestSet interestSet)
		{
			Dequeueable = dequeueable ?? throw new ArgumentNullException(nameof(dequeueable));
			InterestSet = interestSet ?? throw new ArgumentNullException(nameof(interestSet));
		}

		public void Execute()
		{
			//Adds all the entering entities.
			DequeueAddAllEntities(Dequeueable.EnteringDequeueable, InterestSet);
			DequeueAddAllEntities(Dequeueable.LeavingDequeueable, InterestSet);
		}

		private static void DequeueAddAllEntities([NotNull] IDequeable<NetworkEntityGuid> interestDequeueable, [NotNull] IEntityInterestSet interestSet)
		{
			if(interestDequeueable == null) throw new ArgumentNullException(nameof(interestDequeueable));
			if(interestSet == null) throw new ArgumentNullException(nameof(interestSet));

			while(!interestDequeueable.isEmpty)
			{
				NetworkEntityGuid entityGuid = interestDequeueable.Dequeue();
				interestSet.Add(entityGuid);
			}
		}
	}
}
