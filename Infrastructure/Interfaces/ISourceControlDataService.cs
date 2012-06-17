using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Models;
using Infrastructure.Utilities;

namespace Infrastructure.Interfaces
{
	public interface ISourceControlDataService
	{
		ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, string startRevision = null, string endRevision = null);
		void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, string startRevision = null, string endRevision = null);
		Repository.RepositoryType RepositoryType { get; }
	}
}
