using Infrastructure.Interfaces;
using Infrastructure.Utilities;

namespace Infrastructure.Views
{
	/// <summary>
	/// Interaction logic for PopUpView.xaml
	/// </summary>
	public partial class PopUpView : IPopUpView
	{
		public PopUpView()
		{
			InitializeComponent();
			this.DataContext = ViewModelLocator.GetSharedViewModel<IPopUpViewModel>();
		}
	}
}
