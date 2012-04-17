using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Infrastructure.Models;
using Infrastructure.Utilities;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

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
			private set { _repositories = value; }
		}

		public string DiffDirectory { get { return "Diffs"; } }
	}

	public class ApplicationSettings : OverwriteSettingsProfile<ApplicationSettings, GlobalSettings> { }
}
