using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DataServices.Models;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure.Settings;
using Infrastructure.Models;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace SourceControlMonitor.ViewModels
{
	public class RevisionHistoryViewModel : ObservableBase, IRevisionHistoryViewModel
	{
		private ObservableCollectionEx<ICommitItem> _commitItems = new ObservableCollectionEx<ICommitItem>();

		public RevisionHistoryViewModel()
		{
			Mediator.Subscribe<RefreshRepositoryHistoriesEvent>(ignore =>
			{
				if(CurrentRepository != null)
				{
					CurrentRepository.RefreshCommitHistory();
				}
			});

			Mediator.Subscribe<RepositorySelectedEvent>(r =>
			{
				var repo = r as Repository;
				CurrentRepository = repo;
			});
		}

		private Repository _currentRepository;
		public Repository CurrentRepository
		{
			get { return _currentRepository; }
			set
			{
				_currentRepository = value;
				NotifyPropertyChanged("CurrentRepository");
			}
		}
	}
}
