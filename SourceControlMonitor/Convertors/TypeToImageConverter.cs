using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Infrastructure.Models;

namespace SourceControlMonitor.Convertors
{
	public class TypeToImageConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Infrastructure.Models.Repository.RepositoryType type;
			if(Enum.TryParse(value.ToString(), out type))
			{
				return type == Repository.RepositoryType.Svn
						? new BitmapImage(new Uri("pack://application:,,,../Resources/svn48.png", UriKind.Absolute))
						: null;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { throw new NotImplementedException(); }

		#endregion
	}
}
