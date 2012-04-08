using System.IO;
using System.Linq;
using DataServices;
using Infrastructure.Models;
using Infrastructure.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using DataServices.Models;
using System.Collections.ObjectModel;
using Infrastructure.Settings;

namespace DataServices.Test
{


	/// <summary>
	///This is a test class for SubversionDataServiceTest and is intended
	///to contain all SubversionDataServiceTest Unit Tests
	///</summary>
	[TestClass()]
	public class SubversionDataServiceTest
	{
		public SubversionDataServiceTest()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;
		}

		private static System.Reflection.Assembly CustomResolve(object sender, System.ResolveEventArgs args)
		{
			if(args.Name.StartsWith("SharpSVN"))
			{
				var fileName = Path.GetFullPath(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + args.Name);
				if(File.Exists(fileName))
				{
					return System.Reflection.Assembly.LoadFile(fileName);
				}
			}
			return null;
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		private static ObservableCollectionEx<Repository> _currentRepos = null;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			_currentRepos = ApplicationSettings.Instance.SvnRepositories;
			if(_currentRepos == null || !_currentRepos.Any(r => r.Path.ToString() == "https://addev/svn/ad/trunk/world/MPE"))
			{
				if(_currentRepos == null)
				{
					ApplicationSettings.Instance.SvnRepositories = new ObservableCollectionEx<Repository>();
				}
				ApplicationSettings.Instance.SvnRepositories.Add(new Repository() { Path = new Uri("https://addev/svn/ad/trunk/world/MPE") });
				ApplicationSettings.Save();
			}
		}


		[ClassCleanup()]
		public static void MyClassCleanup()
		{
			ApplicationSettings.Instance.SvnRepositories = _currentRepos;
			ApplicationSettings.Save();
		}
		#endregion


		/// <summary>
		///A test for GetLog
		///</summary>
		[TestMethod()]
		public void GetLogForAddressesOnAddressOneAdressTest()
		{
			var target = new SubversionDataService();
			int limit = 30;
			var actual = target.GetLog(limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 30);

			limit = 10;
			actual = target.GetLog(limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 10);
		}

		/// <summary>
		///A test for GetLog
		///</summary>
		[TestMethod()]
		public void GetLogForAddressesOnAddressManyAddressesTest()
		{
			var target = new SubversionDataService();
			var addresses = new List<Repository>() { 
				new Repository() { Path = new Uri("https://mefedmvvm.svn.codeplex.com/svn", UriKind.Absolute) }, 
				new Repository() { Path = new Uri("http://commitmonitor.googlecode.com/svn/trunk", UriKind.Absolute) } 
			};
			ApplicationSettings.Instance.SvnRepositories = new ObservableCollectionEx<Repository>(addresses);
			ApplicationSettings.Save();

			int limit = 30;
			var actual = target.GetLog(limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 60);

			limit = 10;
			actual = target.GetLog(limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 20);
		}
	}
}
