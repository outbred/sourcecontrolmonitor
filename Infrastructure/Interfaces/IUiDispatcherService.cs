using System;
using System.Windows;

namespace Infrastructure.Interfaces
{
	public interface IUiDispatcherService
	{
		void Invoke(Action toInvoke);
		void InvokeAsync(Action toInvoke);
		Window CurrentApplication { get; }
	}
}