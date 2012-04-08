using System.Collections.Generic;

namespace Infrastructure.Controls
{
	/// <summary>
	/// Exposes some thread safe methods for adding/removing items
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConcurrentList<T> : List<T>
	{
		static readonly object Locker = new object();

		public void SafeAdd(T item)
		{
			lock(Locker)
			{
				this.Add(item);
			}
		}

		public bool SafeRemove(T item)
		{
			if(this.Contains(item))
			{
				lock(Locker)
				{
					this.Remove(item);
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
