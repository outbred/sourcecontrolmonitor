using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.ViewModels
{
	public class MenuViewModel : ObservableBase, IMenuViewModel
	{
		public MenuViewModel()
		{

		}

		public DelegateCommand OnCheckCommitsClick
		{
			get { return new DelegateCommand(ignore => Mediator.NotifyColleaguesAsync<RefreshRepositoryHistoriesEvent>(null)); }
		}
	}
}
