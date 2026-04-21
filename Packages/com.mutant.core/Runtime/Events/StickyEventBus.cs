using System;
using System.Collections.Generic;

namespace Mutant.Core.Events
{
    public static class StickyEventBus
    {
        private sealed class Subscription
        {
            public Delegate Original;
            public Action<object> Callback;
        }

        private static readonly Dictionary<Type, object> StickyValues = new();
        private static readonly Dictionary<Type, List<Subscription>> Subscriptions = new();

        public static void Subscribe<T>(Action<T> handler, bool replayLatest = true)
        {
            if (handler == null)
                return;

            Type type = typeof(T);

            if (!Subscriptions.TryGetValue(type, out List<Subscription> list))
            {
                list = new List<Subscription>();
                Subscriptions[type] = list;
            }

            list.Add(new Subscription
            {
                Original = handler,
                Callback = payload =>
                {
                    if (payload is T cast)
                        handler.Invoke(cast);
                }
            });

            if (replayLatest && StickyValues.TryGetValue(type, out object latest) && latest is T replay)
                handler.Invoke(replay);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null)
                return;

            Type type = typeof(T);

            if (!Subscriptions.TryGetValue(type, out List<Subscription> list))
                return;

            list.RemoveAll(s => Equals(s.Original, handler));

            if (list.Count == 0)
                Subscriptions.Remove(type);
        }

        public static void PublishSticky<T>(T evt)
        {
            Type type = typeof(T);
            StickyValues[type] = evt;

            if (!Subscriptions.TryGetValue(type, out List<Subscription> list))
                return;

            List<Subscription> snapshot = new(list);

            foreach (Subscription sub in snapshot)
                sub.Callback?.Invoke(evt);
        }

        public static bool TryGetLatest<T>(out T value)
        {
            if (StickyValues.TryGetValue(typeof(T), out object latest) && latest is T cast)
            {
                value = cast;
                return true;
            }

            value = default;
            return false;
        }

        public static void Clear<T>()
        {
            Type type = typeof(T);
            StickyValues.Remove(type);
            Subscriptions.Remove(type);
        }

        public static void ClearAll()
        {
            StickyValues.Clear();
            Subscriptions.Clear();
        }
    }
}