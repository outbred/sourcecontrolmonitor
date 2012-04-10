using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Models;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using System.Collections.ObjectModel;

namespace SourceControlMonitor.ViewModels
{
	public class RepositoriesViewModel : ViewModelBase, IRepositoriesViewModel
	{
		public RepositoriesViewModel()
		{
			ApplicationSettings.Instance.SvnRepositories = new ObservableCollectionEx<Repository>()
			{
			    new Repository() { Name = "AccessData", Path = new Uri("https://addev/svn/ad/trunk/world/MPE")}
			};
			ApplicationSettings.Save();
		}

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
	}
}
