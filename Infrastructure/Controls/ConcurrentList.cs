using System.Collections.Generic;

namespace Infrastructure.Controls
{
	/// <summary>
	/// Exposes some thread safe methods for adding/removing items
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ConcurrentList<T> : List<T>
	{
		readonly object _locker = new object();

		public void SafeAdd(T item)
		{
			lock(_locker)
			{
				this.Add(item);
			}
		}

		public bool SafeRemove(T item)
		{
			if(this.Contains(item))
			{
				lock(_locker)
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
