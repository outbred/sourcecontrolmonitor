﻿using System;

namespace Infrastructure.Interfaces
{

	public interface IMediatorService
	{
		/// <summary>
		/// Subscribes to a certain ViewModelEvent
		/// </summary>
		/// <param name="eventType">Which event to be subscribed to</param>
		/// <param name="onTriggered">Action to be performed when event is triggered - strong reference</param>
		/// <returns>Subscription token (for unsubscribing)</returns>
		MediatorToken Subscribe<TEvent>(Action<object> onTriggered, TEvent eventType = null) where TEvent : class;

		/// <summary>
		/// Allows a consumer to unsubscribe from events by passing in the type and token
		/// </summary>
		/// <param name="eventType">Which event to be unsubscribed from</param>
		/// <param name="token"></param>
		/// <returns>True if able to unsubscribe</returns>
		bool Unsubscribe<TEvent>(TEvent eventType, MediatorToken token);

		/// <summary>
		/// Triggers the event with an payload
		/// </summary>
		/// <param name="eventType">Which event to publish</param>
		/// <param name="argument"></param>
		void NotifyColleagues<TEvent>(object argument, TEvent eventType = null) where TEvent : class;

		/// <summary>
		/// Triggers the event with payload async from the calling thread
		/// </summary>
		/// <param name="eventType">Which event to publish asynchronously</param>
		/// <param name="argument"></param>
		void NotifyColleaguesAsync<TEvent>(object argument, TEvent eventType = null) where TEvent : class;
	}

	/// <summary>
	/// Simple reference mechanism for keeping track of event subscriptions
	/// </summary>
	public class MediatorToken { }
}
