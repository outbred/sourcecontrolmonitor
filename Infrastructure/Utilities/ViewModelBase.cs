using System.ComponentModel;
using System.Runtime.Serialization;
using Infrastructure.Interfaces;
using MvvmTwitter.Utilities;
using System;

namespace Infrastructure.Utilities
{
	[DataContract]
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		protected readonly IMediatorService MediatorService;
		protected readonly IUiDispatcherService UiDispatcherService;
		protected readonly ILoggerFacade Logger;
		protected readonly IMessageBoxService MessageBoxService;

		public ViewModelBase()
		{
			MediatorService = MediatorLocator.GetSharedMediator();
			UiDispatcherService = UiDispatcherLocator.GetSharedDispatcher();
			Logger = LoggerLocator.GetSharedLogger();
			MessageBoxService = MessageBoxLocator.GetSharedService();
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void NotifyPropertyChanged(string propertyName)
		{
			if(UiDispatcherService != null)
			{
				UiDispatcherService.InvokeAsync(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
			}
		}

		#endregion
	}
}
