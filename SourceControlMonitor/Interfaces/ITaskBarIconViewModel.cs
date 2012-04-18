using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Utilities;

namespace SourceControlMonitor.Interfaces
{
	public interface ITaskBarIconViewModel
	{
		DelegateCommand OnDoubleClick { get; }
		DelegateCommand OnClick { get; }
	}
}
