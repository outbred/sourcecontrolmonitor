using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Infrastructure.Controls;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
	public class MediatorService : IMediatorService
	{
		/// <summary>
		/// Precisely one action per token
		/// </summary>
		private static readonly ConcurrentDictionary<MediatorToken, Action<object>> RegisteredEvents = new ConcurrentDictionary<MediatorToken, Action<object>>();

		/// <summary>
		/// Possibly multiple tokens for one event
		/// </summary>
		private static readonly ConcurrentDictionary<object, ConcurrentList<MediatorToken>> SubscriptionTokens = new ConcurrentDictionary<object, ConcurrentList<MediatorToken>>();

		/// <summary>
		/// To handle non-Enum events, one type to multiple events
		/// </summary>
		private static readonly ConcurrentDictionary<Type, ConcurrentList<MediatorToken>> TypesToTokens = new ConcurrentDictionary<Type, ConcurrentList<MediatorToken>>();

		/// <summary>
		/// Returns a token for a subscribed event
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <param name="onTriggered"></param>
		/// <param name="eventType">If null, registers the action with the Type</param>
		/// <returns></returns>
		public MediatorToken Subscribe<TEvent>(Action<object> onTriggered, TEvent eventType = null) where TEvent : class
		{
			if(onTriggered == null)
			{
				return null;
			}

			MediatorToken token = null;
			var type = typeof(TEvent);

			if(TypesToTokens.ContainsKey(type))
			{
				token = new MediatorToken();
				RegisteredEvents[token] = onTriggered;
				TypesToTokens[type].SafeAdd(token);
			}
			else if(eventType != null && SubscriptionTokens.ContainsKey(eventType))
			{
				token = new MediatorToken();
				RegisteredEvents[token] = onTriggered;
				SubscriptionTokens[eventType].SafeAdd(token);
			}
			else if(eventType != null)
			{
				token = new MediatorToken();
				RegisteredEvents.TryAdd(token, onTriggered);
				SubscriptionTokens.TryAdd(eventType, new ConcurrentList<MediatorToken>() { token });
			}
			else
			{
				token = new MediatorToken();
				RegisteredEvents.TryAdd(token, onTriggered);
				TypesToTokens.TryAdd(type, new ConcurrentList<MediatorToken>() { token });
			}
			return token;
		}

		public bool Unsubscribe<TEvent>(TEvent eventType, MediatorToken token)
		{
			if(token == null)
			{
				return false;
			}

			var type = typeof(TEvent);
			if(SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType].Contains(token))
			{
				var killedIt = SubscriptionTokens[eventType].SafeRemove(token);
				Action<object> someAction;
				killedIt = killedIt && RegisteredEvents.TryRemove(token, out someAction);
				return killedIt;
			}
			else if(TypesToTokens.ContainsKey(type) && TypesToTokens[type].Contains(token))
			{
				var killedIt = TypesToTokens[type].SafeRemove(token);
				Action<object> someAction;
				killedIt = killedIt && RegisteredEvents.TryRemove(token, out someAction);
				return killedIt;
			}
			return false;
		}

		/// <summary>
		/// Colleagues are notified in the order that they subscribed
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="argument"></param>
		public void NotifyColleagues<TEvent>(object argument, TEvent eventType = null) where TEvent : class
		{
			// find all tokens for this event
			// fire off the Action<> for each one in succession
			var type = typeof(TEvent);
			if(eventType != null && SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType] != null)
			{
				var tokens = SubscriptionTokens[eventType];
				tokens.ForEach(t =>
				{
					if(RegisteredEvents.ContainsKey(t))
					{
						RegisteredEvents[t](argument);
					}
				});
			}
			else if(TypesToTokens.ContainsKey(type) && TypesToTokens[type] != null)
			{
				var tokens = TypesToTokens[type];
				tokens.ForEach(t =>
				{
					if(RegisteredEvents.ContainsKey(t))
					{
						RegisteredEvents[t](argument);
					}
				});
			}
		}

		/// <summary>
		/// Colleagues are notified in the order that they subscribed, asynchronously
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="argument"></param>
		public void NotifyColleaguesAsync<TEvent>(object argument, TEvent eventType = null) where TEvent : class
		{
			Task.Factory.StartNew(() => NotifyColleagues(argument, eventType));
		}
	}
}
