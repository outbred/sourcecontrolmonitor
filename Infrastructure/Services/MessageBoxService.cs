using Infrastructure.Interfaces;
using Infrastructure.Utilities;
using System.Windows;

namespace Infrastructure.Services
{
	public class MessageBoxService : IMessageBoxService
	{
		private static readonly IUiDispatcherService _uiDispatcherService = null;
		static MessageBoxService()
		{
			_uiDispatcherService = Container.GetSharedInstance<IUiDispatcherService>();
		}

		public void ShowError(string message, string caption = null)
		{
			if(_uiDispatcherService != null)
			{
				_uiDispatcherService.Invoke(() =>
				{
					if(_uiDispatcherService.CurrentApplication != null)
					{
						MessageBox.Show(_uiDispatcherService.CurrentApplication, message, caption, MessageBoxButton.OK,
										MessageBoxImage.Error);
					}
					else
					{
						MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
					}
				});
			}
		}

		public void ShowInfo(string message, string caption = null)
		{
			if(_uiDispatcherService != null)
			{
				_uiDispatcherService.Invoke(() =>
				{
					if(_uiDispatcherService.CurrentApplication != null)
					{
						MessageBox.Show(_uiDispatcherService.CurrentApplication, message, caption, MessageBoxButton.OK,
										MessageBoxImage.Information);
					}
					else
					{
						MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
					}
				});
			}
		}

		/// <summary>
		/// Shows ok and cancel dialog
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		/// <returns>Null if unable to show dialog; true if 'Ok'</returns>
		public bool? ShowOkCancel(string message, string caption = null)
		{
			if(_uiDispatcherService != null)
			{
				var result = MessageBoxResult.Cancel;
				_uiDispatcherService.Invoke(() =>
				{
					if(_uiDispatcherService.CurrentApplication != null)
					{
						result = MessageBox.Show(_uiDispatcherService.CurrentApplication, message, caption, MessageBoxButton.OKCancel,
												 MessageBoxImage.Question);
					}
					else
					{
						result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
					}
				});
				return result == MessageBoxResult.OK;
			}
			return null;
		}

		/// <summary>
		/// Shows yes and no dialog
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		/// <returns>Null if unable to show dialog; true if 'Yes'</returns>
		public bool? ShowYesNo(string message, string caption = null)
		{
			if(_uiDispatcherService != null)
			{
				var result = MessageBoxResult.Cancel;
				_uiDispatcherService.Invoke(() =>
				{
					if(_uiDispatcherService.CurrentApplication != null)
					{
						result = MessageBox.Show(_uiDispatcherService.CurrentApplication, message, caption, MessageBoxButton.YesNo,
												 MessageBoxImage.Question);
					}
					else
					{
						result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
					}
				});
				return result == MessageBoxResult.Yes;
			}
			return null;
		}
	}
}
