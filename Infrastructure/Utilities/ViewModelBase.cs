using System.ComponentModel;
using System.Runtime.Serialization;
using Infrastructure.Interfaces;
using System;

namespace Infrastructure.Utilities
{
	[DataContract]
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		private static IUiDispatcherService _uiDispatcherService = null;
		protected static IUiDispatcherService UiDispatcherService
		{
			get
			{
				if(_uiDispatcherService == null)
				{
					_uiDispatcherService = UiDispatcherLocator.GetSharedDispatcher();
				}
				return _uiDispatcherService;
			}
		}

		private static IMediatorService _mediator = null;
		protected static IMediatorService Mediator
		{
			get
			{
				if(_mediator == null)
				{
					_mediator = MediatorLocator.GetSharedMediator();
				}
				return _mediator;
			}
		}

		private static IMessageBoxService _messageBoxService = null;
		protected static IMessageBoxService MessageBoxService
		{
			get
			{
				if(_messageBoxService == null)
				{
					_messageBoxService = MessageBoxLocator.GetSharedService();
				}
				return _messageBoxService;
			}
		}

		private static ILoggerFacade _logger = null;
		protected static ILoggerFacade Logger
		{
			get
			{
				if(_logger == null)
				{
					_logger = LoggerLocator.GetSharedLogger();
				}
				return _logger;
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void NotifyPropertyChanged(string propertyName)
		{
			if(UiDispatcherService != null && PropertyChanged != null)
			{
				UiDispatcherService.InvokeAsync(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
			}
		}

		#endregion
	}
}
