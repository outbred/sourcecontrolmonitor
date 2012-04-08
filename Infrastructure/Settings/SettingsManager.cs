using System;
using System.Diagnostics.Contracts;
using System.IO;
using log4net;
using System.Runtime.Serialization;
using Infrastructure.Utilities;
using System.Reflection;

namespace Infrastructure.Settings
{
	public static class SettingsManager
	{
		private static readonly ILog Logger = log4net.LogManager.GetLogger("debug");

		/// <summary>
		/// Saves the settingsObjects provided out to disk, if possible.
		/// </summary>
		/// <typeparam name="CallingClass">The calling class's type - used to generate a unique settings filename</typeparam>
		/// <typeparam name="SettingsClass">The settings object type to be saved</typeparam>
		/// <param name="settingsObject">Any serializable object</param>
		/// <param name="fileExistsAlready">Lets the user know if the settings file was overwritten</param>
		/// <param name="nameMarker">Additional filename specifier for the case where many of the same types of settings are used for the same settingsObject in the same class</param>
		/// <param name="overwrite">If the file already exists, must be true to persist the current settings.</param>
		/// <returns>True if successful, false if not</returns>
		public static bool SaveSettings<CallingClass, SettingsClass>(SettingsClass settingsObject, out bool fileExistsAlready, string nameMarker = null, bool overwrite = true)
			where SettingsClass : class
			where CallingClass : class
		{
			Contract.Requires(typeof(SettingsClass).IsSerializable);
			Contract.Requires(settingsObject != null);
			fileExistsAlready = false;

			try
			{
				// get the filename 
				var fileName = BuildFileName<CallingClass, SettingsClass>(nameMarker);
				fileExistsAlready = File.Exists(fileName);
				if(fileExistsAlready && !overwrite)
				{
					return false;
				}

				if(fileExistsAlready)
				{
					try
					{
						File.Delete(fileName);
					}
					catch(Exception ex)
					{
						Logger.ErrorFormat("Error deleting settings file {0}.\nException:{1}\n{2}", fileName, ex.Message, ex.StackTrace);
						return false;
					}
				}
				else
				{
					if(!Directory.Exists(Path.GetDirectoryName(fileName)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(fileName));
					}
				}

				File.WriteAllText(fileName, settingsObject.ToJSON());

				return true;
			}
			catch(Exception ex)
			{
				Logger.ErrorFormat("Error saving settings file for calling class={0}, settings class={1}, nameMarker={2} .\nException:{3}\n{4}",
					typeof(CallingClass).Name, typeof(SettingsClass).Name, nameMarker, ex.Message, ex.StackTrace);
				return false;
			}
		}

		/// <summary>
		/// Retrieves the settings object specified, if it exists on disk.
		/// </summary>
		/// <typeparam name="CallingClass">The calling class's type - used to generate a unique settings filename</typeparam>
		/// <typeparam name="SettingsClass">The settings object type to be retrieved</typeparam>
		/// <param name="settingsObject">The settings object to be returned</param>
		/// <param name="nameMarker">Additional filename specifier for the case where many of the same types of settings are used for the same settingsObject in the same class</param>
		/// <returns>True if retrieved from file, false if empty object</returns>
		public static bool RetrieveSettings<CallingClass, SettingsClass>(out SettingsClass settingsObject, string nameMarker = null)
			where SettingsClass : class, new()
			where CallingClass : class
		{
			Contract.Requires(typeof(SettingsClass).IsSerializable);
			settingsObject = null;

			Contract.Ensures(settingsObject != null);
			// build the file name
			try
			{
				var fileName = BuildFileName<CallingClass, SettingsClass>(nameMarker);
				if(File.Exists(fileName))
				{
					var data = File.ReadAllText(fileName);
					settingsObject = data.FromJSON<SettingsClass>();
					return true;
				}
				else
				{
					settingsObject = new SettingsClass();
					return false;
				}
			}
			catch(Exception ex)
			{
				Logger.ErrorFormat("Error retrieving settings file for calling class={0}, settings class={1}, nameMarker={2} .\nException:{3}\n{4}",
					typeof(CallingClass).Name, typeof(SettingsClass).Name, nameMarker, ex.Message, ex.StackTrace);
				settingsObject = new SettingsClass();
				return false;
			}
		}

		private static string BuildFileName<CallingClass, SettingsClass>(string nameMarker = null)
			where SettingsClass : class
			where CallingClass : class
		{
			Contract.Ensures(Contract.Result<string>() != null);
			// create the file name: ".(CallingClass.Name).(SettingsClass.Name).nameMarker (if supplied).settings"
			string fileName = null;
			if(!string.IsNullOrWhiteSpace(nameMarker))
			{
				fileName = string.Format("{0}.{1}.{2}.settings", typeof(CallingClass).Name, typeof(SettingsClass).Name, nameMarker);
			}
			else
			{
				fileName = string.Format("{0}.{1}.settings", typeof(CallingClass).Name, typeof(SettingsClass).Name);
			}
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings", fileName);
		}
	}
}
