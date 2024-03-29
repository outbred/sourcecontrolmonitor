﻿using System;
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
using SourceControlMonitor.Interfaces;
using Infrastructure.Utilities;

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for RevisionHistoryView.xaml
	/// </summary>
	public partial class RevisionHistoryView : IRevisionHistoryView
	{
		public RevisionHistoryView()
		{
			InitializeComponent();
			this.DataContext = ViewModelLocator.GetSharedViewModel<IRevisionHistoryViewModel>();
		}
	}
}
