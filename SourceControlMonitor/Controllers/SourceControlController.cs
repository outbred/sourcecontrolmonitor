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

		/// <summary>
		/// This object's sole purpose is to manage objects so that they work as correctly
		/// This can mean making sure they are instantiated for the life of the app, swapping Views in and out of a region, etc.
		/// </summary>
		public SourceControlController()
		{
			_repoViewModel = ViewModelLocator.GetSharedViewModel<IRepositoryEditorViewModel>();
		}
	}
}
