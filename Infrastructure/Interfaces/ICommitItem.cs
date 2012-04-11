using System;
using System.Collections.ObjectModel;
using Infrastructure.Utilities;

namespace Infrastructure.Interfaces
{
	public interface ICommitItem
	{
		string Author { get; }
		DateTime Date { get; }
		string LogMessage { get; }
		long Revision { get; }
		bool? HasLocalEditsOnAnyFile { get; }
		ObservableCollectionEx<IItemChanged> ItemChanges { get; }
	}

	public interface IItemChanged
	{
		string ChangeType { get; }
		string FilePath { get; }
		bool? HasLocalEdits { get; }
		DelegateCommand OnViewChanges { get; }
		bool HasBeenModified { get; }
	}
}
