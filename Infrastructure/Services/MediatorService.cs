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

		public MediatorToken Subscribe<TEvent>(TEvent eventType, Action<object> onTriggered)
		{
			if(onTriggered == null)
			{
				return null;
			}

			MediatorToken token = null;
			if(!SubscriptionTokens.ContainsKey(eventType))
			{
				token = new MediatorToken();
				RegisteredEvents.TryAdd(token, onTriggered);
				SubscriptionTokens.TryAdd(eventType, new ConcurrentList<MediatorToken>() { token });
			}
			else
			{
				token = new MediatorToken();
				RegisteredEvents[token] = onTriggered;
				SubscriptionTokens[eventType].SafeAdd(token);
			}
			return token;
		}

		public bool Unsubscribe<TEvent>(TEvent eventType, MediatorToken token)
		{
			if(token == null)
			{
				return false;
			}

			if(SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType].Contains(token))
			{
				var killedIt = SubscriptionTokens[eventType].SafeRemove(token);
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
		public void NotifyColleagues<TEvent>(TEvent eventType, object argument)
		{
			// find all tokens for this event
			// fire off the Action<> for each one in succession
			if(SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType] != null)
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
		}

		/// <summary>
		/// Colleagues are notified in the order that they subscribed, asynchronously
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="argument"></param>
		public void NotifyColleaguesAsync<TEvent>(TEvent eventType, object argument)
		{
			Task.Factory.StartNew(() => NotifyColleagues(eventType, argument));
		}
	}
}
