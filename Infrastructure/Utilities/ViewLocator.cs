namespace Infrastructure.Utilities
{
	public class ViewLocator
	{
		public static TInterface GetSharedInstance<TInterface>() where TInterface : class
		{
			return Helpers.GetSharedInstance<TInterface>();
		}
	}
}
