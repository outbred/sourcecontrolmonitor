using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure.Settings;

namespace SourceControlMonitor.ViewModels
{
	public class RepositoryEditorViewModel : ObservableBase, IRepositoryEditorViewModel
	{
		private Repository _repoBeforeEdit = null;
		private bool _isNewRepo = false;

		public RepositoryEditorViewModel()
		{
			Mediator.Subscribe<EditRepositoryEvent>(repo =>
			{
				AcceptText = "Commit";
				Repository = repo as Repository;
				_isNewRepo = false;
				if(Repository != null)
				{
					Repository.BlockUpdates = true;
				}

				_repoBeforeEdit = Repository != null ? new Repository()
				{
					Password = Repository.Password,
					Name = Repository.Name,
					Path = Repository.Path,
					Type = Repository.Type,
					UserName = Repository.UserName
				} : null;

				// Make sure to get the details view in place for this repo type
				if(Repository != null)
				{
					Mediator.NotifyColleaguesAsync<RepositoryTypeSelectedInEditEvent>(this.Repository.Type);
				}
			});

			Mediator.Subscribe<AddRepositoryEvent>(repo =>
			{
				AcceptText = "Add";
				Repository = new Repository();
				_isNewRepo = true;
			});

			// When a user selects a different repo type from the combobox, we update the repository object with that type,
			// then publish that new type so the corresponding dataservice can provide a details view for that type
			RepositoryServices = CollectionViewSource.GetDefaultView(DataServiceLocator.GetSharedServices());
			RepositoryServices.CurrentChanged += (s, e) =>
			{
				var current = RepositoryServices.CurrentItem as ISourceControlDataService;
				if(current != null)
				{
					this.Repository.Type = current.RepositoryType;
					Mediator.NotifyColleaguesAsync<RepositoryTypeSelectedInEditEvent>((RepositoryServices.CurrentItem as ISourceControlDataService).RepositoryType);
				}
			};

			// this is the response from the data service providing a custom details view for the seleted repo type
			Mediator.Subscribe<ShowRepositoryDetailsInEditorEvent>(repoDetailsView =>
			{
				RepositoryDetailsView = repoDetailsView;
				var match = (from r in RepositoryServices.Cast<ISourceControlDataService>()
							 where r.RepositoryType == Repository.Type
							 select r).FirstOrDefault();
				RepositoryServices.MoveCurrentTo(match);
			});
		}

		private Repository _repository;
		public Repository Repository
		{
			get { return _repository; }
			set
			{
				_repository = value;
				NotifyPropertyChanged("Repository");
			}
		}

		private string _acceptText;
		public string AcceptText
		{
			get { return _acceptText; }
			set
			{
				_acceptText = value;
				NotifyPropertyChanged("AcceptText");
			}
		}

		private ICollectionView _repositoryServices;
		public ICollectionView RepositoryServices
		{
			get { return _repositoryServices; }
			set
			{
				_repositoryServices = value;
				NotifyPropertyChanged("RepositoryServices");
			}
		}

		private object _repositoryDetailsView;
		public object RepositoryDetailsView
		{
			get { return _repositoryDetailsView; }
			set
			{
				_repositoryDetailsView = value;
				NotifyPropertyChanged("RepositoryDetailsView");
			}
		}

		public DelegateCommand OnOkClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					if(_isNewRepo)
					{
						ApplicationSettings.Instance.Repositories.Add(Repository);
					}
					Repository.Initialize();
					ApplicationSettings.Save();

					Mediator.NotifyColleaguesAsync<RefreshRepositoryHistoriesEvent>(null);
					Mediator.NotifyColleaguesAsync<HideChildWindowEvent>(null);
				});
			}
		}

		public DelegateCommand OnCancelClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					if(Repository != null && _repoBeforeEdit != null)
					{
						Repository.Overwrite(_repoBeforeEdit);
						ApplicationSettings.Save();
					}
					Mediator.NotifyColleaguesAsync<HideChildWindowEvent>(null);
				});
			}
		}
	}
}
