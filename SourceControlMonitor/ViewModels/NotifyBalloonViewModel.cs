using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure.Interfaces;

namespace SourceControlMonitor.ViewModels
{
	public class NotifyBalloonViewModel : ObservableBase, INotifyBalloonViewModel
	{
		public DelegateCommand OnMouseDown
		{
			get
			{
				return new DelegateCommand(ignore => Mediator.NotifyColleaguesAsync<ShowApplicationEvent>(null));
			}
		}
	}
}
