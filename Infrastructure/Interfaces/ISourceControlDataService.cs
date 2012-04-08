using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Models;
using Infrastructure.Utilities;

namespace Infrastructure.Interfaces
{
	public interface ISourceControlDataService
	{
		ObservableCollectionEx<ICommitItem> GetLog(int limit = 30);
		void GetLogAsync(Action<ObservableCollectionEx<ICommitItem>> onComplete, int limit = 30);
	}
}
