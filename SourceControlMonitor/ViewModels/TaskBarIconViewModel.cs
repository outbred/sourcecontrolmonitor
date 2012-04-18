using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.ViewModels
{
	public class TaskBarIconViewModel : ObservableBase, ITaskBarIconViewModel
	{
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
