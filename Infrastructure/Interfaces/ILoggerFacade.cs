using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Infrastructure.Interfaces
{
	public interface ILoggerFacade
	{
		ILog Default { get; }
	}
}
