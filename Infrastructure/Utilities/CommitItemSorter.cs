using System.Collections;
using System.ComponentModel;
using Infrastructure.Interfaces;

namespace Infrastructure.Utilities
{
	public class CommitItemSorter : IComparer
	{
		public CommitItemSorter()
		{
			Direction = ListSortDirection.Ascending;
		}

		public ListSortDirection Direction { get; set; }

		public int Compare(object x, object y)
		{
			var commitItemX = x as ICommitItem;
			var commitItemY = y as ICommitItem;
			if(commitItemX != null && commitItemY != null)
			{
				return (-1)*commitItemX.Revision.CompareTo(commitItemY.Revision);
			}
			return 0;
		}
	}
}