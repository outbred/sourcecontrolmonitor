using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Models;
using Infrastructure.Utilities;

namespace Infrastructure.Interfaces
{
	public interface ISourceControlDataService
	{
		ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, long? startRevision = null, long? endRevision = null);
		void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, long? startRevision = null, long? endRevision = null);
		Repository.RepositoryType RepositoryType { get; }
	}
}
