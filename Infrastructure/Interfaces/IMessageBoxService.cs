namespace Infrastructure.Interfaces
{
	public interface IMessageBoxService
	{
		void ShowError(string message, string caption = null);
		void ShowInfo(string message, string caption = null);
		bool? ShowOkCancel(string message, string caption = null);
		bool? ShowYesNo(string message, string caption = null);
	}
}
