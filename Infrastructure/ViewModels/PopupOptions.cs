using System;
using Infrastructure.Utilities;

namespace Infrastructure.ViewModels
{
	public class PopupOptions
	{
		public enum ButtonType { OkCancel, Ok, YesNo }

		public PopupOptions(ButtonType buttons, string message, Action<bool> onClose = null)
		{
			Buttons = buttons;
			Message = message;
			OnPopupClose = onClose;
		}

		public ButtonType Buttons { get; private set; }
		public string Message { get; private set; }
		private Action<bool> OnPopupClose { get; set; }
		public DelegateCommand OnOptionSelected
		{
			get
			{
				return new DelegateCommand(result =>
				                           	{
				                           		if(OnPopupClose != null)
				                           		{
				                           			OnPopupClose((bool)result);
				                           		}
				                           	}, ignore => true);
			}
		}

		public string AffirmativeText { get { return Buttons == ButtonType.Ok || Buttons == ButtonType.OkCancel ? "Ok" : "Yes"; } }
		public string NegativeText { get { return Buttons == ButtonType.OkCancel ? "Cancel" : "No"; } }
		public bool IsNegativeVisible { get { return Buttons != ButtonType.Ok; } }
	}
}