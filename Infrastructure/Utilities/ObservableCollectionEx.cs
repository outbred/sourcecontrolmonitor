using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	/// <summary>
	/// Much faster AddRange than a manual Add for many items
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks href="http://www.damonpayne.com/Permalink.aspx?title=AddRangeForObservableCollectionInSilverlight3&date=2010-03-04"></remarks>
	public class ObservableCollectionEx<T> : ObservableCollection<T>
	{
		private readonly IUiDispatcherService _uiDispatcherService = null;

		public ObservableCollectionEx()
			: base()
		{
			_suspendCollectionChangeNotification = false;
			_uiDispatcherService = UiDispatcherLocator.GetSharedDispatcher();
		}

		public ObservableCollectionEx(IEnumerable<T> list) : base(list) { }

		bool _suspendCollectionChangeNotification;

		// Override the event so this class can access it
		public override event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Taken from http://geekswithblogs.net/NewThingsILearned/archive/2008/01/16/have-worker-thread-update-observablecollection-that-is-bound-to-a.aspx
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(!_suspendCollectionChangeNotification)
			{
				// Be nice - use BlockReentrancy like MSDN said
				using(BlockReentrancy())
				{
					NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
					if(eventHandler == null)
					{
						return;
					}

					Delegate[] delegates = eventHandler.GetInvocationList();
					// Walk thru invocation list
					foreach(NotifyCollectionChangedEventHandler handler in delegates)
					{
						var dispatcherObject = handler.Target as DispatcherObject;
						// If the subscriber is a DispatcherObject and different thread
						if(dispatcherObject != null && dispatcherObject.CheckAccess() == false)
						{
							// Invoke handler in the target dispatcher's thread
							dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
						}
						else // Execute handler as is
						{
							handler(this, e);
						}
					}
				}
			}
		}


		public void SuspendCollectionChangeNotification()
		{
			_suspendCollectionChangeNotification = true;
		}

		public void ResumeCollectionChangeNotification()
		{
			_suspendCollectionChangeNotification = false;
		}


		public void AddRange(IEnumerable<T> items)
		{
			this.SuspendCollectionChangeNotification();
			int index = base.Count;
			try
			{
				foreach(var i in items)
				{
					base.InsertItem(base.Count, i);
				}
			}
			finally
			{
				this.ResumeCollectionChangeNotification();
				NotifyCollectionChangedEventArgs arg = null;
				if(_uiDispatcherService != null)
				{
					_uiDispatcherService.InvokeAsync(() =>
					{
						arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
						this.OnCollectionChanged(arg);
					});
				}
			}
		}

		/// <summary>
		/// Merges two sets of data - an old and a new set.  Updates the old with new data based on isMatch.  Adds in new items only
		/// </summary>
		/// <param name="updated">Updated/new items</param>
		/// <param name="findMatch ">Finds a match in the existing collection given the new item</param>
		/// <param name="updateItem">Delegate used to update the old item given the new one (if there is one)</param>
		public void MergeAdd(IEnumerable<T> updated, Func<T, IEnumerable<T>, T> findMatch = null, Action<T, T> updateItem = null)
		{
			if(updated == null)
			{
				return;
			}

			this.SuspendCollectionChangeNotification();

			if(this.Count == 0)
			{
				AddRange(updated);
			}
			else if(findMatch == null || updateItem == null)
			{
				// add new ones only...safety check
				var toAdd = (from c in updated
							 where !this.Contains(c)
							 select c).ToList();

				this.AddRange(toAdd);
			}
			else
			{
				// check by more advanced supplied checker
				var toBeUpdated = (from i in this
								   let match = findMatch(i, updated)
								   where match != null
								   select new { OldItem = i, NewItem = match }).ToList();

				toBeUpdated.ForEach(pair =>
				{
					updateItem(pair.OldItem, pair.NewItem);
				});

				var toAdd = (from pair in toBeUpdated.ToDictionary(o => o.OldItem, n => n.NewItem)
							 where !this.Contains(pair.Key)
							 select pair.Value).ToList();

				this.AddRange(toAdd);
			}
		}

	}
}
