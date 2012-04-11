using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataServices.Models;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using SharpSvn;
using SharpSvn.Implementation;
using System.Diagnostics;

namespace DataServices
{
	public static class SvnDataServiceExtensions
	{
		public static List<ICommitItem> ToCommitItems(this Collection<SvnLogEventArgs> list, Uri repoPath, IMediatorService mediator)
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
													ItemChanges = i.ChangedPaths != null ? i.ChangedPaths.ToItemsChanged(i.Revision, repoPath, mediator) : null,
													RepoPath = repoPath
												};
								var path = i.ChangedPaths;
								var itemsChanges = new List<IItemChanged>();
								result.Add(svnItem);
							});
			return result;
		}

		public static ObservableCollectionEx<IItemChanged> ToItemsChanged(this SvnChangeItemCollection collection, SvnRevision latestRevision, Uri repoPath, IMediatorService mediator)
		{
			var result = new List<IItemChanged>();
			collection.ToList().ForEach(p =>
			{
				var isModified = p.Action == SvnChangeAction.Modify;
				var itemChanged = new ItemChanged()
				{
					FilePath = p.RepositoryPath.ToString(),
					ChangeType = Enum.GetName(typeof(SvnChangeAction), p.Action)[0].ToString(),
					HasBeenModified = isModified,
					OnViewChanges = new DelegateCommand(ignore =>
					{
						if(isModified)
						{
							mediator.NotifyColleaguesAsync<BeginBusyEvent>("Downloading unified diff...");
							Task.Factory.StartNew(() =>
							{
								using(var client = new SvnClient())
								{
									try
									{
										SvnInfoEventArgs info;
										var absolutePath = Path.Combine(repoPath.ToString(), p.RepositoryPath.ToString());
										client.GetInfo(absolutePath, out info);
										using(var diff = new MemoryStream())
										{
											if(client.Diff(new SvnUriTarget(absolutePath),
															new SvnRevisionRange(latestRevision, new SvnRevision(latestRevision.Revision - 1)), diff))
											{
												diff.Seek(0, SeekOrigin.Begin);
												var strReader = new StreamReader(diff);
												string str = strReader.ReadToEnd();
												var path = Path.Combine(ApplicationSettings.Instance.DiffDirectory, "tempdiff.txt");
												if(!string.IsNullOrWhiteSpace(str))
												{
													File.WriteAllText(path, str);
													var process = Process.Start(new ProcessStartInfo("notepad", path));
													mediator.NotifyColleaguesAsync<EndBusyEvent>(null);
													process.WaitForExit();
													File.Delete(path);
												}
												else
												{
													mediator.NotifyColleaguesAsync<EndBusyEvent>(null);
													MessageBoxLocator.GetSharedService().ShowError("Unable to retrieve unified diff.", "Error Retrieving Unified Diff");
												}
											}
										}
									}
									catch(Exception ex)
									{
										// log it?
									}
								}
							});
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
