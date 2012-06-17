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
				switch(type)
				{
					case Repository.RepositoryType.Svn:
						return new BitmapImage(new Uri("pack://application:,,,../Resources/svn48.png", UriKind.Absolute));
					case Repository.RepositoryType.Tfs:
						break;
					case Repository.RepositoryType.Git:
						return new BitmapImage(new Uri("pack://application:,,,../Resources/git-logo.png", UriKind.Absolute));
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { throw new NotImplementedException(); }

		#endregion
	}
}
