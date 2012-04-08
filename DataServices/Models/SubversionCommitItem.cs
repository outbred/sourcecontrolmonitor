using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;

namespace DataServices.Models
{
	public class SubversionCommitItem : ICommitItem
	{
		#region Implementation of ICommitItem

		public string Author { get; set; }

		public DateTime Date { get; set; }

		public string LogMessage { get; set; }

		public long Revision { get; set; }

		public bool? HasLocalEditsOnAnyFile { get; set; }

		public ObservableCollectionEx<IItemChanged> ItemChanges { get; set; }

		#endregion
	}
}
