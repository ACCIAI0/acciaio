using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Acciaio
{
    public delegate void RefAction<T>(ref T param);

    ///<summary>
    ///The PubSubBoard, as that unexpectedly funny sounding name suggests, is the
    ///main Hub which event publishers and events subscribers refer to. Here, subscribers
    ///can subscribe for certain events by specifying their name and - if any - their
    ///arguments type. Publishers can trigger events by name and let the Hub notify all
    ///those which subscribed to it. This functionality is NOT threadsafe.
    ///</summary>
    public sealed class PubSubBoard
    {
        private static readonly Type VoidType = typeof(void);
        private static readonly List<object> EmptySubs = new();

        private readonly Dictionary<string, List<object>> _subscribersByType = new();
        private readonly Dictionary<string, List<object>> _refSubscribersByType = new();

        private string BuildKey(string eventName, Type type)
        {
            var builder = new StringBuilder("<")
                    .Append(eventName)
                    .Append(">__")
                    .Append(type.FullName);
            return builder.ToString();
        }

        private void Subscribe(string key, object subscriptionAsObject)
        {
            if (subscriptionAsObject == null)
                throw new ArgumentNullException(nameof(subscriptionAsObject), "Cannot accept null subscriptions");
            if (!_subscribersByType.TryGetValue(key, out List<object> subs))
            {
                subs = new List<object>();
                _subscribersByType.Add(key, subs);
            }
            subs.Add(subscriptionAsObject);
        }

        private void RefSubscribe(string key, object subscriptionAsObject)
        {
            if (subscriptionAsObject == null)
                throw new ArgumentNullException(nameof(subscriptionAsObject), "Cannot accept null subscriptions");
            if (!_refSubscribersByType.TryGetValue(key, out List<object> subs))
            {
                subs = new List<object>();
                _refSubscribersByType.Add(key, subs);
            }
            subs.Add(subscriptionAsObject);
        }

        private bool Unsubscribe(string key, object subscriptionAsObject)
        {
            if (subscriptionAsObject == null)
                throw new ArgumentNullException(nameof(subscriptionAsObject), "Cannot unsubscribe null subscriptions");
            if (!_subscribersByType.TryGetValue(key, out List<object> subs))
                return false;
            return subs.Remove(subscriptionAsObject);
        }

        private bool RefUnsubscribe(string key, object subscriptionAsObject)
        {
            if (subscriptionAsObject == null)
                throw new ArgumentNullException(nameof(subscriptionAsObject), "Cannot unsubscribe null subscriptions");
            if (!_refSubscribersByType.TryGetValue(key, out List<object> subs))
                return false;
            return subs.Remove(subscriptionAsObject);
        }

        private List<object> RetrieveSubs(string key, bool isRef) 
        {
            var dict = isRef ? _refSubscribersByType : _subscribersByType;
            if (!dict.ContainsKey(key))
                return EmptySubs;
            return dict[key];
        }

        ///<summary>
        ///Subscribes to the event of name eventName with the given callback.
        ///</summary>
        public void Subscribe<T>(string eventName, Action<T> subscription) 
            => Subscribe(BuildKey(eventName, typeof(T)), (object)subscription);

        ///<summary>
        ///Subscribes to the event of name eventName with the given callback.
        ///Parameters passed down to callbacks subscribed this way are passed by reference.
        ///</summary>
        public void Subscribe<T>(string eventName, RefAction<T> subscription) 
            => RefSubscribe(BuildKey(eventName, typeof(T)), subscription);

        ///<summary>
        ///Subscribes to the event of name eventName with the given callback with no arguments.
        ///</summary>
        public void Subscribe(string eventName, Action subscription) 
            => Subscribe(BuildKey(eventName, VoidType), (object)subscription);

        ///<summary>
        ///Removes a subscription to the event of name eventName.
        ///</summary>
        public bool Unsubscribe<T>(string eventName, Action<T> subscription) 
            => Unsubscribe(BuildKey(eventName, typeof(T)), (object)subscription);

        ///<summary>
        ///Removes a ref subscription to the event of name eventName.
        ///</summary>
        public bool Unsubscribe<T>(string eventName, RefAction<T> subscription) 
            => RefUnsubscribe(BuildKey(eventName, typeof(T)), subscription);

        ///<summary>
        ///Removes a subscription to the event of name eventName.
        ///</summary>
        public void Unsubscribe(string eventName, Action subscription) 
            => Unsubscribe(BuildKey(eventName, VoidType), (object)subscription);
        
        ///<summary>
        ///Triggers the event of name eventName, thus calling sequentially all subscribed callbacks with no arguments.
        ///</summary>
        public void Trigger(string eventName)
        {
            var subs = RetrieveSubs(BuildKey(eventName, VoidType), false)
                .Cast<Action>()
                .ToList();
            foreach (var sub in subs) sub();
        }

        ///<summary>
        ///Triggers the event of name eventName, thus calling sequentially all subscribed callbacks.
        ///</summary>
        public void Trigger<T>(string eventName, T args) 
        {
            var subs = RetrieveSubs(BuildKey(eventName, typeof(T)), false)
                    .Cast<Action<T>>()
                    .ToList();
            foreach (var sub in subs) sub(args);
        }

        ///<summary>
        ///Triggers the event of name eventName, thus calling sequentially all subscribed callbacks.
        ///Parameters passed down to callbacks called this way are passed by reference.
        ///</summary>
        public void Trigger<T>(string eventName, ref T args)
        {
            var subs = RetrieveSubs(BuildKey(eventName, typeof(T)), true)
                .Cast<RefAction<T>>()
                .ToList();
            foreach (var sub in subs) sub(ref args);
        }
    }
}