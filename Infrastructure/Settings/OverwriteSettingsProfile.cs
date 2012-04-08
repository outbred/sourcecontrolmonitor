
namespace Infrastructure.Settings
{
	public abstract class OverwriteSettingsProfile<TCallingClass, TSettingsClass>
		where TCallingClass : class
		where TSettingsClass : class, new()
	{
		private static readonly object Locker = new object();

		private static TSettingsClass _instance = null;
		public static TSettingsClass Instance
		{
			get
			{
				lock(Locker)
				{
					if(_instance == null)
					{
						TSettingsClass temp;
						IsNew = !SettingsManager.RetrieveSettings<TCallingClass, TSettingsClass>(out temp);
						_instance = temp;
					}
					return _instance;
				}
			}
		}

		private static bool _isNew = true;
		public static bool IsNew
		{
			get
			{
				if(_instance == null)
				{
					var dummy = Instance;
				}
				return _isNew;
			}
			private set
			{
				_isNew = value;
			}
		}

		/// <summary>
		/// Overwrites the MPE GUI settings file with the MPEGUISettings objects in memory
		/// </summary>
		/// <returns></returns>
		public static bool Save()
		{
			var settings = Instance;
			if(settings != null)
			{
				bool exists;
				var result = SettingsManager.SaveSettings<TCallingClass, TSettingsClass>(Instance, out exists, overwrite: true);
				lock(Locker)
				{
					_instance = null;
				}
				return result;
			}
			return false;
		}
	}
}
