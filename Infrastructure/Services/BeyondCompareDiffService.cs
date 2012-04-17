using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Infrastructure.Utilities;
using System.IO;

namespace Infrastructure.Services
{
	public class BeyondCompareDiffService : IFileDiffService
	{
		private readonly string _beyondCompare = null;
		public BeyondCompareDiffService()
		{
			var bcDirectory = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)).FirstOrDefault(d => d.Contains("Beyond Compare"));
			if(!string.IsNullOrWhiteSpace(bcDirectory))
			{
				_beyondCompare = Path.Combine(bcDirectory, "BCompare.exe");
			}
		}

		#region Implementation of IFileDiffService

		public Process ShowDiffs(string fileNameRight, string fileNameLeft)
		{
			if(!File.Exists(fileNameLeft) || !File.Exists(fileNameRight) || !File.Exists(_beyondCompare))
			{
				return null;
			}

			return Process.Start(new ProcessStartInfo { FileName = _beyondCompare, CreateNoWindow = true, Arguments = string.Format("{1} {0} ", fileNameLeft, fileNameRight) });
		}

		public bool IsSupported
		{
			get { return File.Exists(_beyondCompare); }
		}

		public DiffServicePriority Priority
		{
			get { return DiffServicePriority.First; }
		}

		#endregion
	}
}
