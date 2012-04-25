using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.ViewModels
{
	public class TaskBarIconViewModel : ObservableBase, ITaskBarIconViewModel
	{
		public TaskBarIconViewModel()
		{
			Mediator.Subscribe<CommitsPublishedEvent>(item => BalloonContext = item);
		}

		private object _balloonContext;
		public object BalloonContext
		{
			get { return _balloonContext; }
			set
			{
				_balloonContext = value;
				NotifyPropertyChanged("BalloonContext");
			}
		}

		public DelegateCommand OnClick
		{
			get
			{
				return new DelegateCommand(ignore => { });
			}
		}

		public DelegateCommand OnDoubleClick
		{
			get
			{
				return new DelegateCommand(ignore => { });
			}
		}
	}
}
