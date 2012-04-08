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

	}
}
