﻿using System;
using System.Collections.ObjectModel;
using Infrastructure.Utilities;

namespace Infrastructure.Interfaces
{
	public interface ICommitItem
	{
		string RepositoryName { get; set; }
		string Author { get; set; }
		DateTime Date { get; set; }
		string LogMessage { get; set; }
		string Revision { get; set; }
		bool? HasLocalEditsOnAnyFile { get; set; }
		ObservableCollectionEx<IItemChanged> ItemChanges { get; set; }
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
