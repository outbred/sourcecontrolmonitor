using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	public static class UiDispatcherLocator
	{
		public static IUiDispatcherService GetSharedDispatcher()
		{
			return Container.GetSharedInstance<IUiDispatcherService>();
		}
	}
}
