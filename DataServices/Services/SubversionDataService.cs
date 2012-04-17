using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DataServices.Models;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Utilities;
using SharpSvn;
using SharpSvn.Security;
using Infrastructure.Settings;

namespace DataServices
{
	public class SubversionDataService : BaseDataService
	{
		private readonly IMediatorService _mediator = null;
		private readonly IFileDiffService _diffService = null;

		public SubversionDataService()
		{
			_mediator = MediatorLocator.GetSharedMediator();
			_diffService = DiffServiceLocator.GetPriorityService();
		}

		public override void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, long? startRevision = null, long? endRevision = null)
		{
			var task = Task.Factory.StartNew(() =>
			{
				try
				{
					onComplete(GetLog(repo, limit, startRevision, endRevision));
				}
				catch(Exception ex)
				{
					MessageBoxLocator.GetSharedService().ShowError(string.Format("Unable to get the log from '{0}'.\n\n{1}", repo.Path, ex.Message), "Error Downloading Log");
					onComplete(null);
				}
			});

			task.Wait(new TimeSpan(0, 0, 0, repo.SecondsToTimeoutDownload));
			if(task.Status != TaskStatus.RanToCompletion)
			{
				MessageBoxLocator.GetSharedService().ShowError(string.Format("Unable to get the log from '{0}'.", repo.Path), "Error Downloading Log");
				onComplete(null);
			}
		}

		public override Repository.RepositoryType RepositoryType
		{
			get { return Repository.RepositoryType.Svn; }
		}

		public override ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, long? startRevision = null, long? endRevision = null)
		{
			if(repo == null)
			{
				return null;
			}

			// not reliable...unfortunately
			if(!AddressIsAccessible(repo))
			{
				MessageBoxLocator.GetSharedService().ShowError(string.Format("Unable to connect to '{0}'.", repo.Path.ToString()));
				return null;
			}

			var args = new SvnLogArgs { Limit = limit };
			using(var client = new SvnClient())
			{
				var allItems = new List<ICommitItem>();

				if(!string.IsNullOrWhiteSpace(repo.UserName))
				{
					client.Authentication.DefaultCredentials = new NetworkCredential(repo.UserName, repo.Password);
				}

				Collection<SvnLogEventArgs> logItems;
				client.GetLog(repo.Path, args, out logItems);
				if(logItems != null)
				{
					var rootIndex = repo.Path.ToString().IndexOf("svn/");
					var root = repo.Path.ToString().EndsWith("svn/") ? repo.Path.ToString() : repo.Path.ToString().Remove(rootIndex + 4);
					allItems.AddRange(logItems.ToCommitItems(new Uri(root), _mediator, repo.SecondsToTimeoutDownload, OnViewChangeDetails));
				}
				return new ReadOnlyObservableCollection<ICommitItem>(new ObservableCollection<ICommitItem>(allItems));
			}
		}

		private void OnViewChangeDetails(SvnRevision latestRevision, Uri repoPath, SvnChangeItem p, int secondsToTimeout)
		{

			Task.Factory.StartNew(() =>
			{
				_mediator.NotifyColleaguesAsync<BeginBusyEvent>("Downloading change details...");
				var task = Task.Factory.StartNew(() =>
				{
					using(var client = new SvnClient())
					{
						try
						{
							SvnInfoEventArgs info;
							var absolutePath = Path.Combine(repoPath.ToString(), p.RepositoryPath.ToString());
							client.GetInfo(absolutePath, out info);
							using(MemoryStream latest = new MemoryStream(), previous = new MemoryStream())
							{
								client.Write(new SvnUriTarget(absolutePath, latestRevision), latest);
								client.Write(new SvnUriTarget(absolutePath, new SvnRevision(latestRevision.Revision - 1)), previous);
								latest.Seek(0, SeekOrigin.Begin);
								previous.Seek(0, SeekOrigin.Begin);

								string latFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, latestRevision.Revision.ToString() + ".txt");
								using(var latestFile = File.OpenWrite(latFileOnDisk))
								{
									latest.WriteTo(latestFile);
									latestFile.Flush();
								}

								string prevFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, (latestRevision.Revision - 1).ToString(CultureInfo.InvariantCulture) + ".txt");
								using(var previousFile = File.OpenWrite(prevFileOnDisk))
								{
									previous.WriteTo(previousFile);
									previousFile.Flush();
								}

								_mediator.NotifyColleaguesAsync<EndBusyEvent>(null);

								// background this so that our callbacks are not waiting on this complete (unnecessary)
								Task.Factory.StartNew(() =>
								{
									var process = _diffService.ShowDiffs(prevFileOnDisk, latFileOnDisk);

									if(process != null)
									{
										process.WaitForExit();
									}
									File.Delete(prevFileOnDisk);
									File.Delete(latFileOnDisk);
								});
							}
						}
						catch(Exception ex)
						{
							// log it?
						}
					}
				});
				task.Wait(new TimeSpan(0, 0, 0, secondsToTimeout));
				if(task.Status != TaskStatus.RanToCompletion)
				{
					MessageBoxLocator.GetSharedService().ShowError("Unable to download change details.  Dash it all!");
					_mediator.NotifyColleaguesAsync<EndBusyEvent>(null);
				}
			});
		}
	}
}
