using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(!_suspendCollectionChangeNotification)
			{
				base.OnCollectionChanged(e);
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
		/// <param name="isMatch">Is essentially a delegate for IEqualityComparer<T></param>
		/// <param name="updatedItem">Delegate used to retrieve the update item (if there is one) given the old one</param>
		public void MergeAdd(IEnumerable<T> updated, Func<T, IEnumerable<T>, bool> isMatch = null, Func<T, IEnumerable<T>, T> updatedItem = null)
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
			else if(isMatch == null || updatedItem == null)
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
								   where isMatch(i, updated)
								   select new { OldItem = i, NewItem = updatedItem(i, updated) }).ToList();

				toBeUpdated.ForEach(pair =>
				{
					this.Remove(pair.OldItem);
					this.Add(pair.NewItem);
				});

				var toAdd = (from newbie in updated
							 where !this.Contains(newbie)
							 select newbie).ToList();

				this.AddRange(toAdd);
			}
		}

	}
}
