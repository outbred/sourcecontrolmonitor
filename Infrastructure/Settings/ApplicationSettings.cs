using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Infrastructure.Models;
using Infrastructure.Utilities;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows;

namespace Infrastructure.Settings
{

	[DataContract]
	public class GlobalSettings
	{
		private ObservableCollectionEx<Repository> _repositories;

		[DataMember]
		public ObservableCollectionEx<Repository> Repositories
		{
			get
			{
				Contract.Ensures(Contract.Result<ObservableCollectionEx<Repository>>() != null);

				return _repositories ?? (_repositories = new ObservableCollectionEx<Repository>());
			}
			// Should only be set on deserialization
			private set
			{
				_repositories = value;
				if(_repositories != null)
				{
					Application.Current.MainWindow.Loaded += (s, e) =>
					{
						foreach(var repo in _repositories)
						{
							Task.Factory.StartNew(repo.Initialize);
						}
					};
				}
			}
		}

		public string DiffDirectory { get { return "Diffs"; } }
	}

	public class ApplicationSettings : OverwriteSettingsProfile<ApplicationSettings, GlobalSettings> { }
}
