using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	public class MessageBoxLocator
	{
		public static IMessageBoxService GetSharedService()
		{
			return Container.GetSharedInstance<IMessageBoxService>();
		}
	}
}
