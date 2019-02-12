using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Guardians
{
	/// <summary>
	/// Decorated for the <see cref="CollectionsLockingAttribute"/> around
	/// <see cref="IGameTickable"/>s.
	/// </summary>
	public sealed class GlobalCollectionsLockingPolicyTickableDecorator : IGameTickable
	{
		/// <summary>
		/// The collections locking policy.
		/// </summary>
		private GlobalEntityCollectionsLockingPolicy LockingPolicy { get; }

		/// <summary>
		/// The decorated game tickable to be locked around.
		/// </summary>
		private IGameTickable DecoratedGameTickable { get; }

		private LockType DesiredLockingType { get; }

		/// <inheritdoc />
		public GlobalCollectionsLockingPolicyTickableDecorator(
			[NotNull] GlobalEntityCollectionsLockingPolicy lockingPolicy,
			[NotNull] IGameTickable decoratedGameTickable, 
			LockType desiredLockingType)
		{
			if(!Enum.IsDefined(typeof(LockType), desiredLockingType)) throw new InvalidEnumArgumentException(nameof(desiredLockingType), (int)desiredLockingType, typeof(LockType));
			LockingPolicy = lockingPolicy ?? throw new ArgumentNullException(nameof(lockingPolicy));
			DecoratedGameTickable = decoratedGameTickable ?? throw new ArgumentNullException(nameof(decoratedGameTickable));
			DesiredLockingType = desiredLockingType;
		}

		/// <inheritdoc />
		public void Tick()
		{
			using(var lockObj = CreateLockObject())
			{
				DecoratedGameTickable.Tick();
			}
		}

		private IDisposable CreateLockObject()
		{
			switch(DesiredLockingType)
			{
				case LockType.Read:
					return LockingPolicy.ReaderLock(null, CancellationToken.None);
				case LockType.Write:
					return LockingPolicy.WriterLock(null, CancellationToken.None);
			}

			throw new NotSupportedException($"Unexpected {nameof(LockType)}: {(int)DesiredLockingType} encountered.");
		}
	}
}
