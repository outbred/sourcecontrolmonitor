using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Data;
using System.Windows.Threading;
using Infrastructure.Services;
using Infrastructure.Utilities;
using Infrastructure.Interfaces;
using System.ComponentModel;
using Infrastructure.Settings;

namespace Infrastructure.Models
{
	[DataContract]
	public class Repository : ObservableBase, IDataErrorInfo
	{
		private string _name;
		private RepositoryType _type;
		private ISourceControlDataService _dataService = null;
		private DispatcherTimer _historyChecker = null;
		private ObservableCollectionEx<ICommitItem> _items = null;
		private object _locker = null;
		private bool _alreadyUpdating;

		public bool BlockUpdates { get; set; }

		public void Initialize()
		{
			BlockUpdates = false;

			if(CommitItems == null)
			{
				_items = new ObservableCollectionEx<ICommitItem>();
				_locker = new object();

				// Apparently, not only must CommitItems be set on the UI thread, but the ICollectionView object
				// must also be created on it!
				UiDispatcherService.Invoke(() =>
				{
					CommitItems = CollectionViewSource.GetDefaultView(_items);
					var listCollectionView = CommitItems as ListCollectionView;
					if(listCollectionView != null)
					{
						listCollectionView.CustomSort = new CommitItemSorter() { Direction = ListSortDirection.Descending };
					}
				});
			}

			if(_historyChecker != null)
			{
				_historyChecker.Stop();
				_historyChecker = null;
			}

			_dataService = (from d in DataServiceLocator.GetSharedServices()
							where this.Type == d.RepositoryType
							select d).FirstOrDefault();

			this.RefreshCommitHistory();

			_historyChecker = new DispatcherTimer() { IsEnabled = false, Interval = new TimeSpan(0, 0, UpdateInterval ?? 3, 0) };
			_historyChecker.Tick += (s, e) => RefreshCommitHistory();
			_historyChecker.IsEnabled = true;
			_historyChecker.Start();
		}

		public void RefreshCommitHistory()
		{
			if(BlockUpdates)
			{
				return;
			}

			Debug.WriteLine("Refreshing commit history for {0}", this.Name);

			long? startRevision = this._items.OrderByDescending(item => item.Revision).Select(item => item.Revision).FirstOrDefault();

			if(!_alreadyUpdating && _dataService != null)
			{
				lock(_locker)
				{
					_alreadyUpdating = true;
					Status = "Updating...";
				}

				_dataService.GetLogAsync(this, items =>
				{
					if(items != null)
					{
						UiDispatcherService.InvokeAsync(() =>
						{
							var added = this._items.MergeAdd(items,
								(item, collection) => collection.FirstOrDefault(c => c.Revision == item.Revision),
								(itemOld, itemNew) =>
								{
									itemOld.Revision = itemNew.Revision;
									itemOld.Author = itemNew.Author;
									itemOld.Date = itemNew.Date;
									itemOld.HasLocalEditsOnAnyFile = itemNew.HasLocalEditsOnAnyFile;
									itemOld.LogMessage = itemOld.LogMessage;
									itemOld.ItemChanges = itemNew.ItemChanges;
								});
							var list = new List<ICommitItem>();
							if(added != null)
							{
								list.AddRange(added);
							}

							Mediator.NotifyColleaguesAsync<CommitsPublishedEvent>(list);
						});
					}

					lock(_locker)
					{
						_alreadyUpdating = false;
						Status = string.Format("Last Update: {0}", DateTime.Now.ToShortTimeString());
					}
				}, startRevision: startRevision);
			}
		}

		public DelegateCommand OnEditClick
		{
			get
			{
				return new DelegateCommand(ignore => Mediator.NotifyColleaguesAsync<EditRepositoryEvent>(this));
			}
		}

		public DelegateCommand OnDeleteClick
		{
			get
			{
				return new DelegateCommand(ignore =>
				{
					if(MessageBoxService.ShowYesNo(string.Format("Are you sure you want to delete repository '{0}'?", this.Name)) ?? false)
					{
						Mediator.NotifyColleaguesAsync<DeleteRepositoryEvent>(this);
					}
				});
			}
		}

		public string PathString
		{
			get { return Path != null ? Path.ToString() : null; }
			set
			{
				Uri test = null;
				try
				{
					test = new Uri(value, UriKind.Absolute);
				}
				catch(Exception)
				{
					test = null;
				}
				Path = test;
				NotifyPropertyChanged("PathString");
			}
		}

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

		[DataMember]
		public Uri Path { get; set; }

		[DataMember]
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		public enum RepositoryType { Svn, Tfs, Git }

		[DataMember]
		public RepositoryType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				NotifyPropertyChanged("Type");
			}
		}

		private string _userName;

		[DataMember]
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				NotifyPropertyChanged("UserName");
			}
		}

		[DataMember]
		public string EncodedPassword { get; set; }

		public string Password
		{
			get { return !string.IsNullOrWhiteSpace(EncodedPassword) ? Encoding.UTF8.GetString(Convert.FromBase64String(EncodedPassword)) : null; }
			set
			{
				EncodedPassword = null;
				if(!string.IsNullOrWhiteSpace(value))
				{
					EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
				}
				NotifyPropertyChanged("Password");
			}
		}

		private int? _secondsToTimeoutDownload;
		[DataMember]
		public int SecondsToTimeoutDownload
		{
			// default is 15
			get { return _secondsToTimeoutDownload ?? 15; }
			set
			{
				_secondsToTimeoutDownload = value;
				NotifyPropertyChanged("SecondsToTimeoutDownload");
			}
		}

		private ICollectionView _commitItems;
		public ICollectionView CommitItems
		{
			get { return _commitItems; }
			set
			{
				_commitItems = value;
				NotifyPropertyChanged("CommitItems");
			}
		}

		private int? _updateInterval;
		[DataMember]
		public int? UpdateInterval
		{
			get { return _updateInterval; }
			set
			{
				_updateInterval = value;

				if(_historyChecker != null)
				{
					_historyChecker.Stop();
					_historyChecker.Interval = new TimeSpan(0, 0, _updateInterval ?? 3, 0);
					_historyChecker.Start();
				}

				NotifyPropertyChanged("UpdateInterval");
			}
		}

		private bool _suspendUpdates;
		[DataMember]
		public bool SuspendUpdates
		{
			get { return _suspendUpdates; }
			set
			{
				_suspendUpdates = value;
				NotifyPropertyChanged("SuspendUpdates");
			}
		}

		private bool _isSelected;
		[DataMember]
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				NotifyPropertyChanged("IsSelected");
			}
		}

		#region Implementation of IDataErrorInfo

		public string Error
		{
			get { throw new NotImplementedException(); }
		}

		public string this[string columnName]
		{
			get
			{
				string result = null;
				switch(columnName)
				{
					case "PathString":
						if(string.IsNullOrEmpty(PathString) || !new Uri(PathString).IsAbsoluteUri)
						{
							result = "Please enter an absolute Url.";
						}
						break;
					case "Name":
						if(string.IsNullOrEmpty(Name))
						{
							result = "Please enter a nickname for this repository.";
						}
						break;
					case "Password":
						if(string.IsNullOrEmpty(Password) && !string.IsNullOrWhiteSpace(UserName))
						{
							result = "Your password is weak.  Weakness will not be tolerated.";
						}
						break;
				}
				return result;
			}
		}

		#endregion

		public void Overwrite(Repository repoBeforeEdit)
		{
			this.Path = repoBeforeEdit.Path;
			this.UserName = repoBeforeEdit.UserName;
			this.Name = repoBeforeEdit.Name;
			this.Type = repoBeforeEdit.Type;
			this.Password = repoBeforeEdit.Password;
		}
	}
}
