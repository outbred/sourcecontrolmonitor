using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SourceControlMonitor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;

			base.OnStartup(e);
			AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
		}

		private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;
			if(ex == null)
			{
				return;
			}

			Trace.TraceError("Unhandled exception in this simple app. Did someone forget to eat their Wheaties?\n\n{0}", ex.Message);
			if(ex.InnerException != null)
			{
				Trace.TraceError("Inner exception:\n", ex.InnerException.Message);
			}
			//ExceptionPolicy.HandleException(ex, "Default Policy");
			MessageBox.Show(string.Format("An unhandled exception has occurred: {0}\n{1}\n\n{2}\n{3}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : null, ex.InnerException != null ? ex.InnerException.StackTrace : null), "Error",
							MessageBoxButton.OK, MessageBoxImage.Error);
			Environment.Exit(1);
		}

		private static System.Reflection.Assembly CustomResolve(object sender, System.ResolveEventArgs args)
		{
			if(args.Name.StartsWith("SharpSVN"))
			{
				var fileName = Path.GetFullPath(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + args.Name);
				if(File.Exists(fileName))
				{
					return System.Reflection.Assembly.LoadFile(fileName);
				}
			}
			return null;
		}
	}
}
