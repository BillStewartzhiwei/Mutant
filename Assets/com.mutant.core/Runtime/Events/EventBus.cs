using System;
using System.Collections.Generic;

namespace Mutant.Core.Events
{
	public static class EventBus
	{
		private static readonly Dictionary<Type, Delegate> Handlers = new();

		public static void Subscribe<T>(Action<T> handler)
		{
			if (handler == null)
				return;

			Type type = typeof(T);

			if (Handlers.TryGetValue(type, out Delegate existing))
				Handlers[type] = Delegate.Combine(existing, handler);
			else
				Handlers[type] = handler;
		}

		public static void Unsubscribe<T>(Action<T> handler)
		{
			if (handler == null)
				return;

			Type type = typeof(T);

			if (!Handlers.TryGetValue(type, out Delegate existing))
				return;

			Delegate current = Delegate.Remove(existing, handler);

			if (current == null)
				Handlers.Remove(type);
			else
				Handlers[type] = current;
		}

		public static void Publish<T>(T evt)
		{
			Type type = typeof(T);

			if (Handlers.TryGetValue(type, out Delegate del) && del is Action<T> callback)
				callback.Invoke(evt);
		}

		public static void Clear()
		{
			Handlers.Clear();
		}
	}
}
