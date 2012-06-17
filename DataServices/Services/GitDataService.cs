using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataServices.Interfaces;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Settings;
using Infrastructure.Utilities;
using LibGit2Sharp;
using SharpSvn;
using Repository = Infrastructure.Models.Repository;

namespace DataServices.Services
{
	public class GitDataService : BaseDataService
	{
		public GitDataService()
		{
			// Because our event's payloads are not strongly typed, paranoid usage is the only way to properly consume them
			// For strongly typed payloads - see Prism's EventAggregator
			// I do want strongly typed payloads, but to have it, I would have had to re-implement
			// Prism's EventAggregator, which I could do but is beyond the scope of this project
			_mediator.Subscribe<RepositoryTypeSelectedInEditEvent>(arg =>
			{
				Repository.RepositoryType type;
				if(arg != null && Enum.TryParse(arg.ToString(), out type) && type == Repository.RepositoryType.Git)
				{
					_dispatcherService.InvokeAsync(() =>
					{
						var detailsView = ViewLocator.GetSharedInstance<IGitRepositoryDetailsView>();
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
			get { return Repository.RepositoryType.Git; }
		}

		public override ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, string startRevision = null, string endRevision = null)
		{
			if(repo == null)
			{
				return null;
			}

			var allItems = new List<ICommitItem>();
			using(var localRepo = new LibGit2Sharp.Repository(repo.Path.LocalPath))
			{
				// per http://stackoverflow.com/questions/10470838/looping-through-every-commit-in-git-repository-with-libgit2sharp
				var commits = localRepo.Commits.QueryBy(new Filter { Since = localRepo.Refs });
				allItems.AddRange(commits.ToCommitItems(repo.Path, _mediator, repo.SecondsToTimeoutDownload, OnViewChangeDetails, repo.Name));
			}

			return new ReadOnlyObservableCollection<ICommitItem>(new ObservableCollection<ICommitItem>(allItems));
		}

		private void OnViewChangeDetails(LibGit2Sharp.Commit p, ObjectId blob, int secondsToTimeout)
		{
			//    Task.Factory.StartNew(() =>
			//    {
			//        _mediator.NotifyColleaguesAsync<BeginBusyEvent>("Downloading change details...");
			//        Task.Factory.StartNew(() =>
			//        {
			//            using(var client = new SvnClient())
			//            {
			//                try
			//                {
			//                    SvnInfoEventArgs info;
			//                    var absolutePath = Path.Combine(repoPath.ToString(), p.RepositoryPath.ToString());
			//                    client.GetInfo(absolutePath, out info);
			//                    using(MemoryStream latest = new MemoryStream(), previous = new MemoryStream())
			//                    {
			//                        client.Write(new SvnUriTarget(absolutePath, latestRevision), latest);
			//                        client.Write(new SvnUriTarget(absolutePath, new SvnRevision(latestRevision.Revision - 1)), previous);
			//                        latest.Seek(0, SeekOrigin.Begin);
			//                        previous.Seek(0, SeekOrigin.Begin);

			//                        var ext = Path.HasExtension(p.Path) ? Path.GetExtension(p.Path) : ".txt";
			//                        string latFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(p.Path), latestRevision.Revision, ext));
			//                        using(var latestFile = File.OpenWrite(latFileOnDisk))
			//                        {
			//                            latest.WriteTo(latestFile);
			//                            latestFile.Flush();
			//                        }

			//                        string prevFileOnDisk = Path.Combine(ApplicationSettings.Instance.DiffDirectory, string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(p.Path), (latestRevision.Revision - 1), ext));
			//                        using(var previousFile = File.OpenWrite(prevFileOnDisk))
			//                        {
			//                            previous.WriteTo(previousFile);
			//                            previousFile.Flush();
			//                        }

			//                        _mediator.NotifyColleaguesAsync<EndBusyEvent>(null);

			//                        // background this so that our callbacks are not waiting on this complete (unnecessary)
			//                        Task.Factory.StartNew(() =>
			//                        {
			//                            var process = _diffService.ShowDiff(prevFileOnDisk, latFileOnDisk);

			//                            if(process != null)
			//                            {
			//                                process.WaitForExit();
			//                            }
			//                            File.Delete(prevFileOnDisk);
			//                            File.Delete(latFileOnDisk);
			//                        });
			//                    }
			//                }
			//                catch(Exception)
			//                {
			//                    // log it?
			//                }
			//            }
			//        });

			//        // TODO: either cleanly handle a cancel or just let it go forever...
			//        //if(!task.Wait(new TimeSpan(0, 0, 0, secondsToTimeout)))
			//        //{
			//        //    token.Cancel();
			//        //    _messageBoxService.ShowError("Diff download timed out. Please increase the timeout if you continue to receive this error.");
			//        //    _mediator.NotifyColleaguesAsync<EndBusyEvent>(null);
			//        //}
			//    });
		}
	}
}
