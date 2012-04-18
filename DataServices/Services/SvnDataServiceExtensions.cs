using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataServices.Models;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using SharpSvn;
using SharpSvn.Implementation;
using System.Diagnostics;

namespace DataServices
{
	public static class SvnDataServiceExtensions
	{
		public static List<ICommitItem> ToCommitItems(this Collection<SvnLogEventArgs> list, Uri repoPath, IMediatorService mediator, int secondsToTimeout,
			Action<SvnRevision, Uri, SvnChangeItem, int> onViewChangeDetails, string repoName)
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
					ItemChanges = i.ChangedPaths != null ? i.ChangedPaths.ToItemsChanged(i.Revision, repoPath, mediator, secondsToTimeout, onViewChangeDetails) : null,
					RepoPath = repoPath,
					RepositoryName = repoName
				};
				result.Add(svnItem);
			});
			return result;
		}

		public static ObservableCollectionEx<IItemChanged> ToItemsChanged(this SvnChangeItemCollection collection, SvnRevision latestRevision, Uri repoPath, IMediatorService mediator, int secondsToTimeout, Action<SvnRevision, Uri, SvnChangeItem, int> onViewChangeDetails)
		{
			var result = new List<IItemChanged>();
			collection.ToList().ForEach(p =>
			{
				var isModified = p.Action == SvnChangeAction.Modify;
				var itemChanged = new ItemChanged()
				{
					FilePath = p.RepositoryPath.ToString(),
					ChangeType = Enum.GetName(typeof(SvnChangeAction), p.Action)[0].ToString(CultureInfo.InvariantCulture),
					HasBeenModified = isModified,
					OnViewChanges = new DelegateCommand(ignore =>
					{
						if(isModified)
						{
							onViewChangeDetails(latestRevision, repoPath, p, secondsToTimeout);
						}
					})
					// TODO: implement
					//HasLocalEdits = 
				};
				result.Add(itemChanged);
			});
			return new ObservableCollectionEx<IItemChanged>(result);
		}
	}
}
