using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace DataServices
{
	public abstract class BaseDataService : ISourceControlDataService
	{
		static BaseDataService()
		{
			// handle ssl servers with crappy..umm...unregistered certificates
			ServicePointManager.ServerCertificateValidationCallback += ((sender, certificate, chain, sslPolicyErrors) => true);
		}

		/// <summary>
		/// Only use if the address can be pinged...even if credentials are correct, may return
		/// </summary>
		/// <param name="repo"></param>
		/// <returns></returns>
		protected bool AddressIsAccessible(Repository repo)
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
			catch(Exception ex)
			{
				return false;
			}
		}

		#region Implementation of ISourceControlDataService

		public abstract ReadOnlyObservableCollection<ICommitItem> GetLog(Repository repo, int limit = 30, long? startRevision = new long?(), long? endRevision = new long?());
		public abstract void GetLogAsync(Repository repo, Action<ReadOnlyObservableCollection<ICommitItem>> onComplete, int limit = 30, long? startRevision = new long?(), long? endRevision = new long?());
		public abstract Repository.RepositoryType RepositoryType { get; }

		#endregion
	}
}