using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
	public class SubversionDataService : ISourceControlDataService
	{
		private readonly IMediatorService _mediator = null;
		public SubversionDataService()
		{
			_mediator = MediatorLocator.GetSharedMediator();
		}

		public void GetLogAsync(Action<ObservableCollectionEx<ICommitItem>> onComplete, int limit = 30)
		{
			Task.Factory.StartNew(() => onComplete(GetLog(limit)));
		}

		public ObservableCollectionEx<ICommitItem> GetLog(int limit = 30)
		{
			var repos = ApplicationSettings.Instance.SvnRepositories;
			if(repos == null || repos.Count == 0)
			{
				return null;
			}

			var args = new SvnLogArgs { Limit = limit };
			using(var client = new SvnClient())
			{
				var allItems = new List<ICommitItem>();

				MediatorLocator.GetSharedMediator().NotifyColleaguesAsync<BeginBusyEvent>("Downloading log...");
				repos.ToList().ForEach(repo =>
				{
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
						allItems.AddRange(logItems.ToCommitItems(new Uri(root), _mediator));
					}
				});
				MediatorLocator.GetSharedMediator().NotifyColleaguesAsync<EndBusyEvent>(null);
				return new ObservableCollectionEx<ICommitItem>(allItems);
			}
		}

		//void CheckStatus(string path)
		//{
		//    using(var client = new SvnClient())
		//    {
		//        var statusHandler = new EventHandler<SvnStatusEventArgs>(HandleStatusEvent);
		//        client.Status(path, statusHandler);
		//    }
		//}

		//void HandleStatusEvent(object sender, SvnStatusEventArgs args)
		//{
		//    switch(args.LocalContentStatus)
		//    {
		//        case SvnStatus.Added: // Handle appropriately
		//            break;
		//    }

		//    // review other properties of 'args'
		//}
	}
}
