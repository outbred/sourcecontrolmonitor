﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	public class DataServiceLocator
	{
		public static List<ISourceControlDataService> GetShareSources()
		{
			return Helpers.GetSharedInstances<ISourceControlDataService>();
		}
	}
}
