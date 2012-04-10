using System;
using System.Windows;
using System.Windows.Data;

namespace Infrastructure.Convertors
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return (Visibility)value == Visibility.Visible; }

		#endregion
	}
}
