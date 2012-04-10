using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Utilities;

namespace Infrastructure.ViewModels
{
	public class PopUpViewModel : ViewModelBase, IPopUpViewModel
	{
		public PopUpViewModel()
		{
			this.IsPopupVisible = false;
			var mediator = MediatorLocator.GetSharedMediator();
			mediator.Subscribe<ShowPopupEvent>(options => this.Options = options as PopupOptions);
		}

		private PopupOptions _options;
		public PopupOptions Options
		{
			get { return _options; }
			set
			{
				_options = value;
				NotifyPropertyChanged("Options");
			}
		}

		private bool _isPopupVisible;
		public bool IsPopupVisible
		{
			get { return _isPopupVisible; }
			set
			{
				_isPopupVisible = value;
				NotifyPropertyChanged("IsPopupVisible");
			}
		}
	}
}
