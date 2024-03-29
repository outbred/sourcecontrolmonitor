﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DataServices.Interfaces;
using DataServices.Models;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Utilities;
using SharpSvn;
using SharpSvn.Security;
using Infrastructure.Settings;
using System.Threading;

namespace DataServices
{
	public class SvnDataService : BaseDataService
	{
		public SvnDataService()
		{
			// Because our event's payloads are not strongly typed, paranoid usage is the only way to properly consume them
			// For strongly typed payloads - see Prism's EventAggregator
			// I do want strongly typed payloads, but to have it, I would have had to re-implement
			// Prism's EventAggregator, which I could do but is beyond the scope of this project
			_mediator.Subscribe<RepositoryTypeSelectedInEditEvent>(arg =>
			{
				Repository.RepositoryType type;
				if(arg != null && Enum.TryParse(arg.ToString(), out type) && type == Repository.RepositoryType.Svn)
				{
					_dispatcherService.InvokeAsync(() =>
					{
						var detailsView = ViewLocator.GetSharedInstance<ISvnRepositoryDetailsView>();
						_mediator.NotifyColleaguesAsync<ShowRepositoryDetailsInEditorEvent>(detailsView);
					});
				}
			});
		}

		public override void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, string startRevision = null, string endRevision = null)
		{
			Task.Factory.StartNew(() =>
			{
				try
				{
					onComplete(GetLog(repo, limit, startRevision, endRevision));
				}
				catch(Exception ex)
				{
					_messageBoxService.ShowError(string.Format("Unable to get the log from '{0}'.\n\n{1}", repo.Path, ex.Message), "Error Downloading Log");
					onComplete(null);
				}
			});

			// TODO: either cleanly handle a cancel or just let it go forever...
			//if(!task.Wait(new TimeSpan(0, 0, 0, repo.SecondsToTimeoutDownload)))
			//{
			//    token.Cancel();
			//    _messageBoxService.ShowError(string.Format("Unable to get the log from '{0}'.", repo.Path), "Error Downloading Log");
			//    onComplete(null);
			//}
		}

		public override Repository.RepositoryType RepositoryType
		{
			get { return Repository.RepositoryType.Svn; }
		}

		public override ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, string startRevision = null, string endRevision = null)
		{
			if(repo == null)
			{
				return null;
			}

			// not reliable...unfortunately
			if(!WebAddressIsAccessible(repo))
			{
				_messageBoxService.ShowError(string.Format("Unable to connect to '{0}'.", repo.Path.ToString()));
				return null;
			}

			SvnLogArgs args;
			long start, end;
			if(long.TryParse(startRevision, out start) && long.TryParse(endRevision, out end))
			{
				args = new SvnLogArgs { Limit = limit, Start = new SvnRevision(start), End = new SvnRevision(end) };
			}
			else
			{
				args = new SvnLogArgs { Limit = limit };
			}

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
					allItems.AddRange(logItems.ToCommitItems(new Uri(root), _mediator, repo.SecondsToTimeoutDownload, OnViewChangeDetails, repo.Name));
				}
				return new ReadOnlyObservableCollection<ICommitItem>(new ObservableCollection<ICommitItem>(allItems));
			}
		}

		private void OnViewChangeDetails(SvnRevision latestRevision, Uri repoPath, SvnChangeItem p, int secondsToTimeout)
		{

			Task.Factory.StartNew(() =>
			{
				_mediator.NotifyColleaguesAsync<BeginBusyEvent>("Downloading change details...");
				Task.Factory.StartNew(() =>
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

								var ext = Path.HasExtension(p.Path) ? Path.GetExtension(p.Path) : ".txt";
								string latFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(p.Path), latestRevision.Revision, ext));
								using(var latestFile = File.OpenWrite(latFileOnDisk))
								{
									latest.WriteTo(latestFile);
									latestFile.Flush();
								}

								string prevFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(p.Path), (latestRevision.Revision - 1), ext));
								using(var previousFile = File.OpenWrite(prevFileOnDisk))
								{
									previous.WriteTo(previousFile);
									previousFile.Flush();
								}

								_mediator.NotifyColleaguesAsync<EndBusyEvent>(null);

								// background this so that our callbacks are not waiting on this complete (unnecessary)
								Task.Factory.StartNew(() =>
								{
									var process = _diffService.ShowDiff(prevFileOnDisk, latFileOnDisk);

									if(process != null)
									{
										process.WaitForExit();
									}
									File.Delete(prevFileOnDisk);
									File.Delete(latFileOnDisk);
								});
							}
						}
						catch(Exception)
						{
							// log it?
						}
					}
				});

				// TODO: either cleanly handle a cancel or just let it go forever...
				//if(!task.Wait(new TimeSpan(0, 0, 0, secondsToTimeout)))
				//{
				//    token.Cancel();
				//    _messageBoxService.ShowError("Diff download timed out. Please increase the timeout if you continue to receive this error.");
				//    _mediator.NotifyColleaguesAsync<EndBusyEvent>(null);
				//}
			});
		}
	}
}
