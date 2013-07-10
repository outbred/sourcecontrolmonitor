using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Win32;

namespace Infrastructure.Utilities
{
	/// <summary>
	/// Helper accessors to various OS related things
	/// Adapted from http://andrewensley.com/2009/06/c-detect-windows-os-part-1/
	/// </summary>
	public static class OSHelper
	{
		private static readonly LoggerService Logger = new LoggerService();
		private static string _currentDir;
		private static List<FileInfo> _dependentFiles;

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetDllDirectory(string lpPathName);

		public enum OSDescription { Win95, Win98SE, Win98, WinMe, WinNT351, WinNT40, Win2000, WinXP, WinVista, Win7 };
		public enum OSArchitecture { x32, x64 };

		internal const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
		internal const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
		internal const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
		internal const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

		[StructLayout(LayoutKind.Sequential)]
		internal struct SYSTEM_INFO
		{
			public ushort wProcessorArchitecture;
			public ushort wReserved;
			public uint dwPageSize;
			public IntPtr lpMinimumApplicationAddress;
			public IntPtr lpMaximumApplicationAddress;
			public UIntPtr dwActiveProcessorMask;
			public uint dwNumberOfProcessors;
			public uint dwProcessorType;
			public uint dwAllocationGranularity;
			public ushort wProcessorLevel;
			public ushort wProcessorRevision;
		};

		[DllImport("kernel32.dll")]
		internal static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

		public class OSDetails
		{
			public override string ToString()
			{
				string result = "unknown";
				if(this.Description.HasValue)
				{
					//Got something.  Let's prepend "Windows" and get more info.
					result = this.Description.ToString();
					//See if there's a service pack installed.
					if(!String.IsNullOrWhiteSpace(this.ServicePack))
					{
						//Append it to the OS name.  i.e. "Windows XP Service Pack 3"
						result += " " + this.ServicePack;
					}
					//Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
					result += " " + this.Architecture.ToString();
				}
				return result;
			}

			public OSDescription? Description { get; set; }
			public OSArchitecture? Architecture { get; set; }
			public string ServicePack { get; set; }
		}

		static OSArchitecture? GetOSArchitecture()
		{
			SYSTEM_INFO sysInfo = new SYSTEM_INFO();
			GetNativeSystemInfo(ref sysInfo);

			switch(sysInfo.wProcessorArchitecture)
			{
				case PROCESSOR_ARCHITECTURE_AMD64:
					return OSArchitecture.x64;

				case PROCESSOR_ARCHITECTURE_INTEL:
					return OSArchitecture.x32;
			}
			return null;
		}

		public static OSDetails GetOSDetails()
		{
			//Get Operating system information.
			OperatingSystem os = Environment.OSVersion;
			//Get version information about the os.
			Version vs = os.Version;
			var detail = new OSDetails();

			if(os.Platform == PlatformID.Win32Windows)
			{
				//This is a pre-NT version of Windows
				switch(vs.Minor)
				{
					case 0:
						detail.Description = OSDescription.Win95;
						break;
					case 10:
						if(vs.Revision.ToString() == "2222A")
							detail.Description = OSDescription.Win98SE;
						else
							detail.Description = OSDescription.Win98;
						break;
					case 90:
						detail.Description = OSDescription.WinMe;
						break;
					default:
						break;
				}
			}
			else if(os.Platform == PlatformID.Win32NT)
			{
				switch(vs.Major)
				{
					case 3:
						detail.Description = OSDescription.WinNT351;
						break;
					case 4:
						detail.Description = OSDescription.WinNT40;
						break;
					case 5:
						if(vs.Minor == 0)
							detail.Description = OSDescription.Win2000;
						else
							detail.Description = OSDescription.WinXP;
						break;
					case 6:
						if(vs.Minor == 0)
							detail.Description = OSDescription.WinVista;
						else
							detail.Description = OSDescription.Win7;
						break;
					default:
						break;
				}
			}

			detail.ServicePack = os.ServicePack;
			detail.Architecture = GetOSArchitecture();

			return detail;
		}

		/// <summary>
		/// Will setup the environment to find unmanaged c++ dependencies and resolve .NET dependencies, based on the OS architecture
		/// </summary>
		/// <remarks>
		/// Should be called by an .exe right at startup
		/// Presumes there are x86 and x64 subdir's with these dependencies from the exe's location
		/// </remarks>
		public static void HandleDependencies()
		{
			_currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if(OSHelper.GetOSDetails().Architecture == OSHelper.OSArchitecture.x32)
			{
				_currentDir = Path.Combine(_currentDir, "x86");
			}
			else
			{
				_currentDir = Path.Combine(_currentDir, "x64");
			}

			if(Directory.Exists(_currentDir))
			{
				var isItSet = OSHelper.SetDllDirectory(_currentDir);
				Logger.Default.Debug(string.Format("{0} set dll sub dir", isItSet ? "Successfull" : "NOT successfully...unsuccessfully even"));

				var info = new DirectoryInfo(_currentDir);
				_dependentFiles = info.GetFiles("*.dll").ToList();

				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
			}
		}

		private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			var parts = args.Name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			if(parts.Count > 0)
			{
				var match = _dependentFiles.FirstOrDefault(f => f.Name == (parts[0] + ".dll"));
				if(match != null)
				{
					Logger.Default.DebugFormat("Found matching assembly for {0}", args.Name);
					return Assembly.LoadFrom(match.FullName);
				}
			}
			return null;
		}
	}
}
