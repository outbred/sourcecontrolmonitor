using System;
using System.Collections.Generic;
using Infrastructure.Utilities;
using SourceControlMonitor.Interfaces;
using Infrastructure;
using System.Windows;

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
			Container.Initialize(new List<string>() { "SourceControlMonitor.exe", "Infrastructure.dll", "DataServices.dll" });
			this.DataContext = ViewModelLocator.GetSharedViewModel<IShellViewModel>();
			var mediator = MediatorLocator.GetSharedMediator();
			mediator.Subscribe<EditRepositoryEvent>(repo => Application.Current.Dispatcher.BeginInvoke(new Action(() => this.childWindow.Show())));
			mediator.Subscribe<AddRepositoryEvent>(repo => Application.Current.Dispatcher.BeginInvoke(new Action(() => this.childWindow.Show())));
			mediator.Subscribe<HideChildWindowEvent>(ignore => Application.Current.Dispatcher.BeginInvoke(new Action(() => this.childWindow.Close())));
		}
	}
}
