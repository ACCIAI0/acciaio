using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Acciaio
{
    ///<summary>
    ///The PubSubBoard, as that unexpectedly funny sounding name suggests, is the
    ///main Hub which event publishers and events subscribers refer to. Here, subscribers
    ///can subscribe for certain events by specifying their name and - if any - their
    ///arguments type. Publishers can trigger events by name and let the Hub notify all
    ///those which subscribed to it. This functionality is NOT threadsafe.
    ///</summary>
    public sealed class PubSubBoard
    {
        private static readonly Type VOID_TYPE = typeof(void);

        private readonly Dictionary<string, List<object>> _subscribersByType = new();

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
                throw new ArgumentNullException("Cannot accept null subscriptions");
            if (!_subscribersByType.TryGetValue(key, out List<object> subs))
            {
                subs = new List<object>();
                _subscribersByType.Add(key, subs);
            }
            subs.Add(subscriptionAsObject);
        }

        private bool Unsubscribe(string key, object subscriptionAsObject)
        {
            if (subscriptionAsObject == null)
                throw new ArgumentNullException("Cannot unsubscribe null subscriptions");
            if (!_subscribersByType.TryGetValue(key, out List<object> subs))
                return false;
            return subs.Remove(subscriptionAsObject);
        }

        private List<object> RetrieveSubs(string key) 
        {
            if (!_subscribersByType.ContainsKey(key))
                return new List<object>();
            return _subscribersByType[key];
        }

        ///<summary>
        ///Subscribes to the event of name eventName with the given callback.
        ///</summary>
        public void Subscribe<T>(string eventName, Action<T> subscription) =>
                Subscribe(BuildKey(eventName, typeof(T)), (object)subscription);

        ///<summary>
        ///Subscribes to the event of name eventName with the given callback with no arguments.
        ///</summary>
        public void Subscribe(string eventName, Action subscription) =>
                Subscribe(BuildKey(eventName, VOID_TYPE), (object)subscription);

        ///<summary>
        ///Removes a subscription to the event of name eventName.
        ///</summary>
        public bool Unsubscribe<T>(string eventName, Action<T> subscription) =>
                Unsubscribe(BuildKey(eventName, typeof(T)), (object)subscription);

        ///<summary>
        ///Removes a subscription to the event of name eventName.
        ///</summary>
        public void Unsubscribe(string eventName, Action subscription) =>
                Unsubscribe(BuildKey(eventName, VOID_TYPE), (object)subscription);

        ///<summary>
        ///Triggers the event of name eventName, thus calling sequentially all subscribed callbacks.
        ///</summary>
        public void Trigger<T>(string eventName, T args) => RetrieveSubs(BuildKey(eventName, typeof(T)))
                .Cast<Action<T>>()
                .ToList()
                .ForEach(sub => sub(args));

        ///<summary>
        ///Triggers the event of name eventName, thus calling sequentially all subscribed callbacks with no arguments.
        ///</summary>
        public void Trigger(string eventName) => RetrieveSubs(BuildKey(eventName, VOID_TYPE))
                .Cast<Action>()
                .ToList()
                .ForEach(sub => sub());
    }
}