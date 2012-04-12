using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Models;
using Infrastructure.Utilities;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using SourceControlMonitor.Interfaces;
using SourceControlMonitor.ViewModels;

namespace SourceControlMonitor.Controllers
{
	public class SourceControlController : ISourceControlController
	{
		private readonly IRepositoryEditorViewModel _repoViewModel = null;

		public SourceControlController()
		{
			_repoViewModel = ViewModelLocator.GetSharedViewModel<IRepositoryEditorViewModel>();
		}
	}
}
