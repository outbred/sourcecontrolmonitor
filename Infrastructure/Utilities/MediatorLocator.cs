using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;

namespace MvvmTwitter.Utilities
{
	// Architecture prompt: Why even have this class?
	public class MediatorLocator
	{
		public static IMediatorService GetSharedMediator()
		{
			return Helpers.GetSharedInstance<IMediatorService>();
		}
	}
}
