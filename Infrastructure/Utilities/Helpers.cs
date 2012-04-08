using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Infrastructure.Interfaces;
using Infrastructure.Services;

namespace Infrastructure.Utilities
{
	public static class Helpers
	{
		private static readonly List<Type> _knownTypes = new List<Type>();
		private static readonly LoggerService Logger = new LoggerService();

		public static void Initialize(List<string> assembliesToLoad)
		{
			try
			{
				if(_knownTypes.Count == 0)
				{

					var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					if(!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
					{
						assembliesToLoad.ForEach(a =>
						{
							Logger.Default.DebugFormat("Trying to load {0}", a);
							try
							{
								var assembly = Assembly.LoadFrom(a);
								var types = assembly.GetTypes();
								_knownTypes.AddRange(types);
							}
							catch(Exception ex)
							{
								Logger.Default.Error(string.Format("Unable to load assembly {0}", a), ex);
							}
						});
					}
					else
					{
						throw new Exception("You cannot run this program at the root of a drive!");
					}

					if(_knownTypes.Count == 0)
					{
						throw new Exception("The impossible has happened.  This is not a .NET assembly.");
					}
				}
			}
			catch(Exception ex)
			{
				Logger.Default.Error("Unable to load types into the container", ex);
			}
		}

		static readonly ConcurrentDictionary<Type, object> AlreadyCreated = new ConcurrentDictionary<Type, object>();
		/// <summary>
		/// By reflection, finds the first instance of a particular interface in this assembly, creates it and returns it
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns></returns>
		public static TInterface GetSharedInstance<TInterface>() where TInterface : class
		{
			var typeInQuestion = typeof(TInterface);
			if(AlreadyCreated.ContainsKey(typeInQuestion))
			{
				var list = AlreadyCreated[typeInQuestion] as List<TInterface>;
				if(list != null && list.Count > 0)
				{
					return list[0];
				}
			}
			else
			{
				var instance = (from t in _knownTypes
								//Activator.CreateInstance demands an empty ctor
								where t.GetInterface(typeInQuestion.Name, false) != null && t.GetConstructor(Type.EmptyTypes) != null
								select Activator.CreateInstance(t) as TInterface).ToList();
				if(instance.Count > 0)
				{
					AlreadyCreated.GetOrAdd(typeInQuestion, instance);
				}
				return instance.Count > 0 ? instance[0] : null;
			}
			return null;
		}

		/// <summary>
		/// By reflection, finds all instances of a particular interface in this assembly, creates and returns them
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns></returns>
		public static List<TInterface> GetSharedInstances<TInterface>() where TInterface : class
		{
			var typeInQuestion = typeof(TInterface);
			if(AlreadyCreated.ContainsKey(typeInQuestion))
			{
				return AlreadyCreated[typeInQuestion] as List<TInterface>;
			}

			var instances = (from t in _knownTypes
							 //Activator.CreateInstance demands an empty ctor
							 where t.GetInterface(typeInQuestion.Name, false) != null && t.GetConstructor(Type.EmptyTypes) != null
							 select Activator.CreateInstance(t) as TInterface).ToList();

			if(instances.Count > 0)
			{
				AlreadyCreated.GetOrAdd(typeInQuestion, instances);
			}

			return instances;
		}
	}
}
