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

		private static Repository _testRepo = null;
		private static List<Repository> _testRepos = null;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			_testRepo = new Repository() { Path = new Uri("https://addev/svn/ad/trunk/world/MPE") };
			_testRepos =
				new List<Repository>()
				    {
						new Repository() { Path = new Uri("https://mefedmvvm.svn.codeplex.com/svn", UriKind.Absolute) }, 
						new Repository() { Path = new Uri("http://commitmonitor.googlecode.com/svn/trunk", UriKind.Absolute) } 
					};
		}


		#endregion


		/// <summary>
		///A test for GetLog
		///</summary>
		[TestMethod()]
		public void GetLogForAddressesOnAddressOneAdressTest()
		{
			var target = new SvnDataService();
			int limit = 30;
			var actual = target.GetLog(_testRepo, limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 30);

			limit = 10;
			actual = target.GetLog(_testRepo, limit);
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Count > 0);
			Assert.IsTrue(actual.Count == 10);
		}
	}
}
