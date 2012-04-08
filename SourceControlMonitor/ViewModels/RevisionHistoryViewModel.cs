using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataServices.Models;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure.Settings;

namespace SourceControlMonitor.ViewModels
{
	public class RevisionHistoryViewModel : ViewModelBase, IRevisionHistoryViewModel
	{
		private readonly List<ISourceControlDataService> _dataServices = null;
		public RevisionHistoryViewModel()
		{
			_dataServices = DataServiceLocator.GetShareSources();
			if(_dataServices != null)
			{
				_dataServices.ForEach(s => s.GetLogAsync(items =>
															{
																if(items != null)
																{
																	CommitItems.AddRange(items);
																}
															}));
			}
		}

		private ObservableCollectionEx<ICommitItem> _commitItems = new ObservableCollectionEx<ICommitItem>();
		public ObservableCollectionEx<ICommitItem> CommitItems
		{
			get { return _commitItems; }
			set
			{
				_commitItems = value;
				NotifyPropertyChanged("CommitItems");
			}
		}
	}
}
