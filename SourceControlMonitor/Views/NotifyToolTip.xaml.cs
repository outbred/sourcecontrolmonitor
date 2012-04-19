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

namespace SourceControlMonitor.Views
{
	/// <summary>
	/// Interaction logic for NotifyToolTip.xaml
	/// </summary>
	public partial class NotifyToolTip : UserControl
	{
		public NotifyToolTip()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The tooltip details.
		/// </summary>
		public static readonly DependencyProperty InfoTextProperty =
			DependencyProperty.Register("InfoText",
										typeof(string),
										typeof(NotifyToolTip),
										new FrameworkPropertyMetadata(""));

		/// <summary>
		/// A property wrapper for the <see cref="InfoTextProperty"/>
		/// dependency property:<br/>
		/// The tooltip details.
		/// </summary>
		public string InfoText
		{
			get { return (string)GetValue(InfoTextProperty); }
			set { SetValue(InfoTextProperty, value); }
		}
	}
}
