using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
	public class LoggerService : ILoggerFacade
	{
		static LoggerService()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		private static ILog _debugLogger;
		public ILog Default
		{
			get
			{
				if(_debugLogger == null)
				{
					_debugLogger = log4net.LogManager.GetLogger("debug");
				}
				return _debugLogger;
			}
		}
	}
}
