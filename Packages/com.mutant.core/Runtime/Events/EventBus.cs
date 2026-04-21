using System;
using System.Collections.Generic;
using Mutant.Core.Diagnostics;

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

			CoreRecorder.Record("EventBus", $"Subscribe<{type.Name}>");
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

			CoreRecorder.Record("EventBus", $"Unsubscribe<{type.Name}>");
		}

		public static void Publish<T>(T evt)
		{
			Type type = typeof(T);

			if (Handlers.TryGetValue(type, out Delegate del) && del is Action<T> callback)
			{
				callback.Invoke(evt);
				CoreRecorder.Record("EventBus", $"Publish<{type.Name}>");
			}
		}

		public static void Clear()
		{
			Handlers.Clear();
			CoreRecorder.Record("EventBus", "Clear");
		}
	}
}
