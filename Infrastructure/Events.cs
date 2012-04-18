using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
	public class ShowPopupEvent { }
	public class DeleteRepositoryEvent { }
	public class BeginBusyEvent { }
	public class EndBusyEvent { }
	public class EditRepositoryEvent { }
	//public class ShowChildWindowEvent { }
	public class HideChildWindowEvent { }
	public class AddRepositoryEvent { }
	public class RefreshRepositoryHistoriesEvent { }
	public class RepositorySelectedEvent { }
	public class CommitsPublishedEvent { }
	public class ApplicationHiddenEvent { }
	public class ApplicationRestoredEvent { }
	/// <summary>
	/// Used just by the NotifyBalloonViewModel...for now
	/// </summary>
	public class ShowApplicationEvent { }
}
