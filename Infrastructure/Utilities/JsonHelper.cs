using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using Infrastructure.Interfaces;
using log4net.Repository.Hierarchy;

namespace Infrastructure.Utilities
{
	public static class JsonHelper
	{
		private static readonly ILoggerFacade Logger;

		static JsonHelper()
		{
			Logger = LoggerLocator.GetSharedLogger();
		}

		/// <summary>
		/// Use in conjunction with strongly typed DataContract classes to serialize to a JSON string
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToJSON<T>(this T obj) where T : class
		{
			using(var ms = new MemoryStream())
			{
				try
				{
					var ser = new DataContractJsonSerializer(obj.GetType());
					ser.WriteObject(ms, obj);
					return Encoding.UTF8.GetString(ms.ToArray());
				}
				catch(Exception ex1)
				{
					Logger.Default.Error(string.Format("ToJSON serialize object of type '{0}' ", typeof(T).Name), ex1);
					return "";
				}
			}
		}

		/// <summary>
		/// Use in conjunction with strongly typed DataContract classes to deserialize a JSON string to a strongly typed class
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <param name="logException"> </param>
		/// <param name="retry"> </param>
		/// <returns></returns>
		public static T FromJSON<T>(this string str, bool logException = true, bool retry = false) where T : class
		{
			try
			{
				str = str.Trim('\0');
				var bytes = Encoding.Default.GetBytes(str.ToCharArray());
				using(var ms = new MemoryStream(bytes))
				{
					var ser = new DataContractJsonSerializer(typeof(T));
					return ser.ReadObject(ms) as T;
				}
			}
			catch(SerializationException)
			{
				// in case some dummy (Oxygen??) didn't encode the string properly...
				if(!retry)
				{
					Logger.Default.ErrorFormat("SerializationException when deserializing {0} from JSON string. Reencoding as UTF8", typeof(T).Name);
					return Encoding.UTF8.GetString(Encoding.Default.GetBytes(str.ToCharArray())).FromJSON<T>(logException, true);
				}
				return null;
			}
			catch(Exception ex)
			{
				if(logException)
				{
					// don't want to bloat the log with the exception details...
					Logger.Default.Error(string.Format("Unable to deserialize to object of type '{0}' with string:\n\n{1}", typeof(T).Name, str), ex);
				}
				return null;
			}
		}

		public static object FromJSON(this string str, Type type, bool logException = true, bool retry = false, bool UTF8String = false)
		{
			try
			{
				str = str.Trim('\0');
				byte[] bytes = UTF8String ? Encoding.UTF8.GetBytes(str.ToCharArray()) : Encoding.Default.GetBytes(str.ToCharArray());
				using(var ms = new MemoryStream(bytes))
				{
					var ser = new DataContractJsonSerializer(type);
					return ser.ReadObject(ms);
				}
			}
			catch(SerializationException ex)
			{
				// in case some dummy (Oxygen??) didn't encode the string properly...
				if(!retry)
				{
					Logger.Default.Error(string.Format("SerializationException when deserializing {0} from JSON string. Reencoding as UTF8", type != null ? type.Name : "unknown"), ex);
					return Encoding.UTF8.GetString(Encoding.Default.GetBytes(str.ToCharArray())).FromJSON(type, logException, true);
				}
				return null;
			}
			catch(Exception ex)
			{
				if(logException)
				{
					// don't want to bloat the log with the exception details...
					Logger.Default.Error(string.Format("Unable to deserialize to object of type '{0}' with string:\n\n{1}", type != null ? type.Name : "unknown", str), ex);
				}
				return null;
			}
		}

		public static bool TryDeserialize<T>(string jsonString, out List<T> list, bool logException = false) where T : class
		{
			// this is an invalid sequence of characters for json deserialization
			CleanString(ref jsonString);
			list = jsonString.FromJSON<List<T>>(logException: logException, retry: true);
			return list != null;
		}

		public static bool TryDeserialize<T>(string jsonString, out T instance) where T : class
		{
			// this is an invalid sequence of characters for json deserialization
			CleanString(ref jsonString);
			instance = jsonString.FromJSON<T>();
			return instance != null;
		}

		private static void CleanString(ref string jsonString)
		{
			if(!string.IsNullOrWhiteSpace(jsonString))
			{
				var regex = new Regex(@"/""(\w*\d*)/""");
				jsonString = regex.Replace(jsonString, "\\\"$1\\\"");
				jsonString = jsonString.Replace(@"\/", @"/");
				jsonString = jsonString.Replace("\r\n", "");
			}
		}
	}
}
