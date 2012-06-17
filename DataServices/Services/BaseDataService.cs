using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Utilities;

namespace DataServices
{
	public abstract class BaseDataService : ISourceControlDataService
	{
		protected static readonly IMediatorService _mediator = null;
		protected static readonly IFileDiffService _diffService = null;
		protected static readonly IMessageBoxService _messageBoxService = null;
		protected static readonly IUiDispatcherService _dispatcherService = null;

		static BaseDataService()
		{
			_mediator = MediatorLocator.GetSharedMediator();
			_diffService = DiffServiceLocator.GetPriorityService();
			_messageBoxService = MessageBoxLocator.GetSharedService();
			_dispatcherService = UiDispatcherLocator.GetSharedDispatcher();

			// handle ssl servers with crappy..umm...unregistered certificates
			ServicePointManager.ServerCertificateValidationCallback += ((sender, certificate, chain, sslPolicyErrors) => true);
		}

		/// <summary>
		/// Only use if the address can be pinged...even if credentials are correct, may return
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		protected bool WebAddressIsAccessible(Repository repo)
		{
			try
			{
				// Create a request to the passed URI.  
				var req = WebRequest.Create(repo.Path);

				if(!string.IsNullOrWhiteSpace(repo.UserName))
				{
					req.Credentials = new NetworkCredential(repo.UserName, repo.Password);
				}
				else
				{
					req.Credentials = CredentialCache.DefaultNetworkCredentials;
				}
				req.Method = "HEAD";

				// Get the response object.  
				var res = req.GetResponse() as HttpWebResponse;

				return res != null && res.StatusCode == HttpStatusCode.OK;
			}
			catch(WebException e)
			{
				return e.Status != WebExceptionStatus.Timeout && e.Status != WebExceptionStatus.NameResolutionFailure && e.Status != WebExceptionStatus.SendFailure
					&& e.Status != WebExceptionStatus.ConnectFailure && e.Status != WebExceptionStatus.UnknownError;
			}
			catch(Exception)
			{
				return false;
			}
		}

		#region Implementation of ISourceControlDataService

		public abstract ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, string startRevision = null, string endRevision = null);
		public abstract void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, string startRevision = null, string endRevision = null);
		public abstract Repository.RepositoryType RepositoryType { get; }

		#endregion
	}
}