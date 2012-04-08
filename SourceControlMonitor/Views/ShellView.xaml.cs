using System.Collections.Generic;
using Infrastructure.Utilities;
using MvvmTwitter.Utilities;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView
	{
		public ShellView()
		{
			InitializeComponent();
			Helpers.Initialize(new List<string>() { "SourceControlMonitor.exe", "Infrastructure.dll", "DataServices.dll" });
			this.DataContext = ViewModelLocator.GetSharedViewModel<IShellViewModel>();
		}
	}
}
