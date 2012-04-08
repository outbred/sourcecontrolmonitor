using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.ViewModels
{
	public class ShellViewModel : ViewModelBase, IShellViewModel
	{
		public ShellViewModel()
		{
			MenuView = ViewLocator.GetSharedInstance<IMenuView>();
			MainView = ViewLocator.GetSharedInstance<IRevisionHistoryView>();
			RepositoriesView = ViewLocator.GetSharedInstance<IRepositoriesView>();
		}

		private object _menuView;
		public object MenuView
		{
			get { return _menuView; }
			set
			{
				_menuView = value;
				NotifyPropertyChanged("MenuView");
			}
		}

		private object _mainView;
		public object MainView
		{
			get { return _mainView; }
			set
			{
				_mainView = value;
				NotifyPropertyChanged("MainView");
			}
		}

		private object _repositoriesView;
		public object RepositoriesView
		{
			get { return _repositoriesView; }
			set
			{
				_repositoriesView = value;
				NotifyPropertyChanged("RepositoriesView");
			}
		}
	}
}
