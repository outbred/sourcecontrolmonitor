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
using DataServices.Interfaces;

namespace DataServices.Views
{
	/// <summary>
	/// Interaction logic for GitRepositoryDetailsView.xaml
	/// </summary>
	public partial class GitRepositoryDetailsView : IGitRepositoryDetailsView
	{
		public GitRepositoryDetailsView()
		{
			InitializeComponent();
			// does not need a viewmodel b/c the datacontext is supplied by the parent (RepositoryEditorView)
		}
	}
}
