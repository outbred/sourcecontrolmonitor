using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Models;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using System.Collections.ObjectModel;
using Infrastructure;

namespace SourceControlMonitor.ViewModels
{
	public class RepositoriesViewModel : ViewModelBase, IRepositoriesViewModel
	{
		public ObservableCollectionEx<Repository> Repositories
		{
			get { return ApplicationSettings.Instance.SvnRepositories; }
			set
			{
				ApplicationSettings.Instance.SvnRepositories = value;
				ApplicationSettings.Save();
				NotifyPropertyChanged("SvnRepositories");
			}
		}

		public DelegateCommand OnAddClick { get { return new DelegateCommand((ignore) => Mediator.NotifyColleaguesAsync<AddRepositoryEvent>(null)); } }
	}
}
