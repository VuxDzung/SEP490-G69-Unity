namespace SEP490G69
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class EventManager : MonoBehaviour, IGameContext
    {

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Subscribe an event's object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }
            _subscribers[type].Add(callback);
        }

        /// <summary>
        /// Unsubscribe an event's object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(callback);
            }
        }

        /// <summary>
        /// Trigger the event's source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        public void Publish<T>(T evt) where T : IEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    ((Action<T>)callback)?.Invoke(evt);
                }
            }
        }

        public void ClearAll()
        {
            _subscribers.Clear();
        }

        public void SetManager(ContextManager manager)
        {
            
        }
    }
}