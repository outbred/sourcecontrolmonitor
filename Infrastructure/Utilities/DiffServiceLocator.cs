using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Utilities
{
	public class DiffServiceLocator
	{
		public static List<IFileDiffService> GetSharedServices()
		{
			return Container.GetSharedInstances<IFileDiffService>();
		}

		public static IFileDiffService GetPriorityService()
		{
			return GetSharedServices().Where(s => s.IsSupported).OrderBy(s => s.Priority).FirstOrDefault();
		}
	}
}
