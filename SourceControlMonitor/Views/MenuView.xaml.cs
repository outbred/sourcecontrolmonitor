using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure.Utilities;
using Shell.Tablet.Views;
using SourceControlMonitor.Interfaces;

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for MenuView.xaml
	/// </summary>
	public partial class MenuView : IMenuView
	{
		public MenuView()
		{
			InitializeComponent();
			this.DataContext = ViewModelLocator.GetSharedViewModel<IMenuViewModel>();
		}

		private void OnAboutClick(object sender, RoutedEventArgs e)
		{
			var about = new AboutBox();
			about.ShowDialog();
		}
	}
}
