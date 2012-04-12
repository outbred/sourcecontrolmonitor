using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	// Architecture prompt: Why even have this class?
	// [HINT] : abstraction, abstraction, abstraction
	public class LoggerLocator
	{
		public static ILoggerFacade GetSharedLogger()
		{
			return Container.GetSharedInstance<ILoggerFacade>();
		}
	}
}
