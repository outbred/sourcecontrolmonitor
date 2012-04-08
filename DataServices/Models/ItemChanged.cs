using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;

namespace DataServices.Models
{
	public class ItemChanged : IItemChanged
	{
		#region Implementation of IItemChanged

		public string Type { get; set; }

		public string FilePath { get; set; }

		public bool? HasLocalEdits { get; set; }

		#endregion
	}
}
