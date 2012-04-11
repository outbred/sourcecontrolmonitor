using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using System.IO;
using Infrastructure;

namespace SourceControlMonitor.ViewModels
{
	public class ShellViewModel : ViewModelBase, IShellViewModel
	{
		public ShellViewModel()
		{
			if(!Directory.Exists(ApplicationSettings.Instance.DiffDirectory))
			{
				Directory.CreateDirectory(ApplicationSettings.Instance.DiffDirectory);
			}
			var files = Directory.GetFiles(ApplicationSettings.Instance.DiffDirectory).ToList();
			files.ForEach(File.Delete);

			MenuView = ViewLocator.GetSharedInstance<IMenuView>();
			MainView = ViewLocator.GetSharedInstance<IRevisionHistoryView>();
			RepositoriesView = ViewLocator.GetSharedInstance<IRepositoriesView>();
			Mediator.Subscribe<BeginBusyEvent>(text =>
												{
													IsBusy = true;
													IsBusyText =  (text ?? "").ToString();
												});
			Mediator.Subscribe<EndBusyEvent>(text => IsBusy = false);
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

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				NotifyPropertyChanged("IsBusy");
			}
		}

		private string _isBusyText;
		public string IsBusyText
		{
			get { return _isBusyText; }
			set
			{
				_isBusyText = value;
				NotifyPropertyChanged("IsBusyText");
			}
		}
	}
}
