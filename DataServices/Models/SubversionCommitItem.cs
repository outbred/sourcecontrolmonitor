﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using SharpSvn;

namespace DataServices.Models
{
	public class SubversionCommitItem : ViewModelBase, ICommitItem
	{
		#region Implementation of ICommitItem

		private string _author;
		public string Author
		{
			get { return _author; }
			set
			{
				_author = value;
				NotifyPropertyChanged("Author");
			}
		}

		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				NotifyPropertyChanged("Date");
			}
		}

		private string _logMessage;
		public string LogMessage
		{
			get { return _logMessage; }
			set
			{
				_logMessage = value;
				NotifyPropertyChanged("LogMessage");
			}
		}

		private long _revision;
		public long Revision
		{
			get { return _revision; }
			set
			{
				_revision = value;
				NotifyPropertyChanged("Revision");
			}
		}

		private bool? _hasLocalEditsOnAnyFile;
		public bool? HasLocalEditsOnAnyFile
		{
			get { return _hasLocalEditsOnAnyFile; }
			set
			{
				_hasLocalEditsOnAnyFile = value;
				NotifyPropertyChanged("HasLocalEditsOnAnyFile");
			}
		}

		private ObservableCollectionEx<IItemChanged> _itemChanges;
		public ObservableCollectionEx<IItemChanged> ItemChanges
		{
			get { return _itemChanges; }
			set
			{
				_itemChanges = value;
				NotifyPropertyChanged("ItemChanges");
			}
		}

		private Uri _repoPath;
		public Uri RepoPath
		{
			get { return _repoPath; }
			set
			{
				_repoPath = value;
				NotifyPropertyChanged("RepoPath");
			}
		}
		#endregion
	}
}
