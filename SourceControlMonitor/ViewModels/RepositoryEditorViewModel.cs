﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Models;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure.Settings;

namespace SourceControlMonitor.ViewModels
{
	public class RepositoryEditorViewModel : ViewModelBase, IRepositoryEditorViewModel
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
				_repoBeforeEdit = Repository != null ? new Repository()
									{
										Password = Repository.Password,
										Name = Repository.Name,
										Path = Repository.Path,
										Type = Repository.Type,
										UserName = Repository.UserName
									} : null;
			});

			Mediator.Subscribe<AddRepositoryEvent>(repo =>
			{
				AcceptText = "Add";
				Repository = new Repository();
				_isNewRepo = true;
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

		public DelegateCommand OnOkClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					if(_isNewRepo)
					{
						if(ApplicationSettings.Instance.SvnRepositories == null)
						{
							ApplicationSettings.Instance.SvnRepositories = new ObservableCollectionEx<Repository>();
						}
						ApplicationSettings.Instance.SvnRepositories.Add(Repository);
					}
					ApplicationSettings.Save();
					Mediator.NotifyColleaguesAsync<RefreshRepositoryHistories>(null);
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