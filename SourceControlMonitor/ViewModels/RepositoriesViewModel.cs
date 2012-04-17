using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
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
		public RepositoriesViewModel()
		{
			Repositories = new CollectionViewSource { Source = ApplicationSettings.Instance.Repositories };
			Repositories.View.CurrentChanged += (s, e) => Mediator.NotifyColleaguesAsync<RepositorySelectedEvent>(Repositories.View.CurrentItem as Repository);
			if(ApplicationSettings.Instance.Repositories.Count > 0)
			{
				Repositories.View.MoveCurrentToFirst();
				Mediator.NotifyColleaguesAsync<RepositorySelectedEvent>(Repositories.View.CurrentItem as Repository);
			}

			Mediator.Subscribe<DeleteRepositoryEvent>(r =>
			{
				var repo = r as Repository;
				if(repo != null && ApplicationSettings.Instance.Repositories.Contains(repo))
				{
					UiDispatcherService.InvokeAsync(() =>
					{
						ApplicationSettings.Instance.Repositories.Remove(repo);
						ApplicationSettings.Save();
					});
				}
			});
		}

		public CollectionViewSource Repositories { get; set; }

		public DelegateCommand OnAddClick { get { return new DelegateCommand((ignore) => Mediator.NotifyColleaguesAsync<AddRepositoryEvent>(null)); } }
	}
}
