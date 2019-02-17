using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common.Logging;
using Fasterflect;

namespace PSOBB
{
	public abstract class EventQueueBasedTickable<TEventInterface, TEventArgs> : BaseSingleEventListenerInitializable<TEventInterface, TEventArgs>, IGameTickable
		where TEventInterface : class 
		where TEventArgs : EventArgs
	{
		/// <summary>
		/// Indicates if all events in <see cref="EventQueue"/> should be serviced each tick.
		/// </summary>
		public bool ServiceAllQueue { get; }

		/// <summary>
		/// Syncronization object.
		/// </summary>
		protected readonly object SyncObj = new object();

		private Queue<TEventArgs> EventQueue { get; set; }

		protected ILog Logger { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subscriptionService">The subscription service.</param>
		/// <param name="serviceAllQueue">Indicates if all events should be serviced from the event queue each tick.</param>
		/// <param name="logger"></param>
		protected EventQueueBasedTickable(TEventInterface subscriptionService, bool serviceAllQueue, [NotNull] ILog logger) 
			: base(subscriptionService)
		{
			ServiceAllQueue = serviceAllQueue;
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			EventQueue = new Queue<TEventArgs>(5);
		}

		/// <inheritdoc />
		protected sealed override void OnEventFired(object source, TEventArgs args)
		{
			//For event queue based tickables we just enqueue the args recieved.
			lock(SyncObj)
				EventQueue.Enqueue(args);
		}

		/// <inheritdoc />
		public void Tick()
		{
			//On tick we just check if the eventqueue has events
			//if it does we can dispatch one. Otherwise,
			if(EventQueue.Count == 0)
				return;

			if(ServiceAllQueue)
			{
				//We don't want to loop on count since it
				//could change midloop and exploit could cause
				//a server to stall under some circumstances.
				int count = EventQueue.Count;

				lock(SyncObj)
					for(int i = 0; i < count; i++)
					{
						HandleEvent(EventQueue.Dequeue());
					}

				//At this point, it's possible that the EventQueue has more events
				//but we are NOT going to handle them until next tick.
			}
			else
			{
				lock(SyncObj)
					HandleEvent(EventQueue.Dequeue());
			}
		}

		/// <summary>
		/// Implementers should handle the event <see cref="args"/>
		/// that will be dispatched during <see cref="Tick"/> on the main thread.
		/// </summary>
		/// <param name="args"></param>
		protected abstract void HandleEvent(TEventArgs args);

		protected void RemoveEventMatchingPredicate([NotNull] Func<TEventArgs, bool> predicate)
		{
			if(predicate == null) throw new ArgumentNullException(nameof(predicate));

			lock(SyncObj)
			{
				EventQueue = new Queue<TEventArgs>(EventQueue.Where(i => !predicate(i)).ToArrayTryAvoidCopy());
			}
		}
	}
}
