namespace Infrastructure.Utilities
{
	// Architecture prompt: Why even have this class?
	public class ViewModelLocator
	{
		public static TInterface GetSharedViewModel<TInterface>() where TInterface : class
		{
			return Container.GetSharedInstance<TInterface>();
		}
	}
}
