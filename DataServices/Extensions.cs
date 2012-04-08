using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataServices.Models;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using SharpSvn;
using SharpSvn.Implementation;

namespace DataServices.Extensions
{
	public static class Extensions
	{
		public static List<ICommitItem> ToCommitItems(this Collection<SvnLogEventArgs> list)
		{
			var result = new List<ICommitItem>();
			list.ToList().ForEach(i =>
							{
								var svnItem = new SubversionCommitItem()
												{
													Author = i.Author,
													Date = i.Time,
													LogMessage = i.LogMessage,
													Revision = i.Revision,
													ItemChanges = i.ChangedPaths != null ? i.ChangedPaths.ToItemsChanged() : null
												};
								var path = i.ChangedPaths;
								var itemsChanges = new List<IItemChanged>();
								result.Add(svnItem);
							});
			return result;
		}

		public static ObservableCollectionEx<IItemChanged> ToItemsChanged(this SvnChangeItemCollection collection)
		{
			var result = new List<IItemChanged>();
			collection.ToList().ForEach(p =>
			{
				var itemChanged = new ItemChanged()
									{
										FilePath = p.RepositoryPath.ToString(),
										Type = Enum.GetName(typeof(SvnChangeAction), p.Action)
									};
				result.Add(itemChanged);
			});
			return new ObservableCollectionEx<IItemChanged>(result);
		}
	}
}
