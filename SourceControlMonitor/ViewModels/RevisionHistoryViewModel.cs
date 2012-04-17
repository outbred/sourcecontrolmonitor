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
	public class RevisionHistoryViewModel : ViewModelBase, IRevisionHistoryViewModel
	{
		private ObservableCollectionEx<ICommitItem> _commitItems = new ObservableCollectionEx<ICommitItem>();
		private readonly List<ISourceControlDataService> _dataServices;

		private ConcurrentDictionary<Repository, ObservableCollectionEx<ICommitItem>> _histories =
			new ConcurrentDictionary<Repository, ObservableCollectionEx<ICommitItem>>();
		DispatcherTimer _historyChecker = new DispatcherTimer() { IsEnabled = false, Interval = new TimeSpan(0, 0, 5, 0) };
		private readonly object _locker = new object(), _fetcherUpdate = new object();
		private bool _alreadyUpdating = false;

		public RevisionHistoryViewModel()
		{
			_historyChecker.Tick += (s, e) => RefreshCommitHistory();
			_historyChecker.IsEnabled = true;
			_historyChecker.Start();

			CommitItems = CollectionViewSource.GetDefaultView(_commitItems);
			//CommitItems.SortDescriptions.Add(new SortDescription("Revision", ListSortDirection.Descending));
			var listCollectionView = CommitItems as ListCollectionView;
			if(listCollectionView != null)
			{
				listCollectionView.CustomSort = new CommitItemSorter() { Direction = ListSortDirection.Descending };
			}

			RefreshCommitHistory();
			Mediator.Subscribe<RefreshRepositoryHistoriesEvent>(ignore => RefreshCommitHistory());
			Mediator.Subscribe<RepositoriesSelectedEvent>(repo =>
			{
				var repos = repo as List<Repository>;
				CurrentRepositories = null;
				UiDispatcherService.Invoke(() => _commitItems.Clear());

				if(repos != null)
				{
					repos.Where(r => r != null).ToList().ForEach(r =>
					{
						if(!_histories.ContainsKey(r))
						{
							_histories.TryAdd(r, new ObservableCollectionEx<ICommitItem>());
						}
					});

					CurrentRepositories = repo as List<Repository>;
				}
				RefreshCommitHistory();
			});
			_dataServices = DataServiceLocator.GetSharedServices();
		}

		private List<Repository> CurrentRepositories { get; set; }

		private void RefreshCommitHistory()
		{
			if(!_alreadyUpdating && _dataServices != null && CurrentRepositories != null)
			{
				lock(_locker)
				{
					_alreadyUpdating = true;
					Status = "Updating...";
					CurrentRepositories.RemoveAll(c => c == null);
				}

				// match the repo to its service...could be done by the repo itself (??)
				var matches = (from d in _dataServices
							   let repos = CurrentRepositories.Where(c => c.Type == d.RepositoryType).ToList()
							   where repos.Count > 0
							   select new { Repos = repos, Service = d }).ToList();

				var reposToFetch = matches.Select(m => m.Repos).Count();

				matches.ForEach(m =>
				{
					m.Repos.ForEach(r =>
					{
						long? startRevision = null;
						if(_histories[r].Count > 0)
						{
							startRevision = _histories[r].OrderByDescending(item => item.Revision).Select(item => item.Revision).FirstOrDefault();
						}


						m.Service.GetLogAsync(r, items =>
						{
							lock(_fetcherUpdate)
							{
								--reposToFetch;
							}
							if(items != null)
							{
								// the items here may be duplicate data, but new objects since they may be new from the service, so match by revision
								_histories[r].MergeAdd(items,
									(item, collection) => collection.Any(c => c.Revision == item.Revision),
									(item, collection) => collection.FirstOrDefault(i => i.Revision == item.Revision));

								UiDispatcherService.InvokeAsync(() =>
								{
									_commitItems.Clear();
									_commitItems.AddRange(_histories[r]);
								});
							}
						}, startRevision: startRevision);
					});
				});

				Task.Factory.StartNew(() =>
				{
					while(reposToFetch != 0)
					{
						Thread.Sleep(100);
					}
					lock(_locker)
					{
						_alreadyUpdating = false;
						Status = string.Format("Last Update: {0}", DateTime.Now.ToShortTimeString());
					}
				});
			}
		}

		public ICollectionView CommitItems { get; set; }

		private string _status;
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				NotifyPropertyChanged("Status");
			}
		}
	}
}
