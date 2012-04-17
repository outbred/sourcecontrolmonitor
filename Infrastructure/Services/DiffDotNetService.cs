using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure.Utilities
{
	public class DiffDotNetService : IFileDiffService
	{
		private static readonly string DiffDotNet = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DiffDotNet.exe");
		public Process ShowDiffs(string fileNameRight, string fileNameLeft)
		{
			if(!File.Exists(fileNameLeft) || !File.Exists(fileNameRight) || !File.Exists(DiffDotNet))
			{
				return null;
			}

			return Process.Start(new ProcessStartInfo { FileName = DiffDotNet, CreateNoWindow = true, Arguments = string.Format("{0} {1} /f:ta", fileNameLeft, fileNameRight) });
		}

		public bool IsSupported { get { return File.Exists(DiffDotNet); } }

		public DiffServicePriority Priority { get { return DiffServicePriority.Third; } }
	}
}
