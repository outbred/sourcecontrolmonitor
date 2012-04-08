using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Infrastructure.Models;
using Infrastructure.Utilities;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Infrastructure.Settings
{

	[DataContract]
	public class GlobalSettings
	{
		[DataMember]
		public ObservableCollectionEx<Repository> SvnRepositories { get; set; }
	}

	public class ApplicationSettings : OverwriteSettingsProfile<ApplicationSettings, GlobalSettings> { }
}
