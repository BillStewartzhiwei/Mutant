using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mutant.Core.Events
{
    public static class EventBus
    {
        private sealed class Subscription
        {
            public Delegate Original;
            public Action<object> Callback;
            public bool Once;
        }

        private static readonly Dictionary<Type, List<Subscription>> Subscriptions = new();
        private static bool _logEnabled;

        public static void Configure(bool logEnabled)
        {
            _logEnabled = logEnabled;
        }

        public static void Subscribe<T>(Action<T> handler)
        {
            AddSubscription(handler, false);
        }

        public static void SubscribeOnce<T>(Action<T> handler)
        {
            AddSubscription(handler, true);
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

        public static bool HasSubscribers<T>()
        {
            return Subscriptions.TryGetValue(typeof(T), out List<Subscription> list) && list.Count > 0;
        }

        public static void Publish<T>(T evt)
        {
            Type type = typeof(T);

            if (!Subscriptions.TryGetValue(type, out List<Subscription> list) || list.Count == 0)
                return;

            List<Subscription> snapshot = new(list);
            List<Subscription> onceToRemove = null;

            foreach (Subscription sub in snapshot)
            {
                sub.Callback?.Invoke(evt);

                if (sub.Once)
                {
                    onceToRemove ??= new List<Subscription>();
                    onceToRemove.Add(sub);
                }
            }

            if (onceToRemove != null)
            {
                foreach (Subscription sub in onceToRemove)
                    list.Remove(sub);

                if (list.Count == 0)
                    Subscriptions.Remove(type);
            }

            Log($"Publish: {type.Name}");
        }

        public static void Clear<T>()
        {
            Subscriptions.Remove(typeof(T));
        }

        public static void ClearAll()
        {
            Subscriptions.Clear();
        }

        private static void AddSubscription<T>(Action<T> handler, bool once)
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
                Once = once,
                Callback = payload =>
                {
                    if (payload is T cast)
                        handler.Invoke(cast);
                }
            });
        }

        private static void Log(string message)
        {
            if (_logEnabled)
                Debug.Log("[EventBus] " + message);
        }
    }
}