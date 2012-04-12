using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	// Architecture prompt: Why even have this class?
	public class MediatorLocator
	{
		public static IMediatorService GetSharedMediator()
		{
			return Container.GetSharedInstance<IMediatorService>();
		}
	}
}
