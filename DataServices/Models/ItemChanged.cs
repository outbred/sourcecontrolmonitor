using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;

namespace DataServices.Models
{
	public class ItemChanged : IItemChanged
	{
		#region Implementation of IItemChanged

		public string ChangeType { get; set; }

		public string FilePath { get; set; }

		public bool? HasLocalEdits { get; set; }

		public DelegateCommand OnViewChanges { get; set; }

		#endregion
	}
}
