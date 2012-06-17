using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataServices.Models;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using LibGit2Sharp;

namespace DataServices.Services
{
	public static class GitDataServiceExtensions
	{
		public static List<ICommitItem> ToCommitItems(this ICommitCollection list, Uri path, IMediatorService mediator, int secondsToTimeout,
			Action<Commit, ObjectId, int> onViewChangeDetails, string repoName)
		{
			var result = new List<ICommitItem>();
			list.ToList().ForEach(i =>
			{
				var gitItem = new CommitItem()
				{
					Author = string.Format("{0} <{1}>", i.Committer.Name, i.Committer.Email),
					Date = i.Committer.When.DateTime,
					LogMessage = i.Message,
					Revision = i.Id.ToString(),
					// TODO: Need to provide the user a way to see the list of changes, change types, and when they click, a diff
					ItemChanges = null,// i.Tree != null ? i.Tree.ToItemsChanged(i, path, mediator, secondsToTimeout, onViewChangeDetails) : null,
					RepoPath = path,
					RepositoryName = repoName
				};
				result.Add(gitItem);
			});
			return result;
		}

		public static ObservableCollectionEx<IItemChanged> ToItemsChanged(this Tree collection, Commit commit, Uri repoPath, IMediatorService mediator, int secondsToTimeout,
			Action<Commit, ObjectId, int> onViewChangeDetails)
		{
			var result = new List<IItemChanged>();
			collection.ToList().ForEach(p =>
			{
				//var isModified = p.Target.
				var itemChanged = new ItemChanged()
				{

					FilePath = p.Path,
					//ChangeType = p.Mode,
					//HasBeenModified = isModified,
					OnViewChanges = new DelegateCommand(ignore =>
					{
						// TODO: figure out how to do this
						if(false)
						{
							onViewChangeDetails(commit, p.Target.Id, secondsToTimeout);
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
