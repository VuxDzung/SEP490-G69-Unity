using System.Collections.Generic;
using UnityEngine;

namespace SEP490G69
{
    public class NarrativeEventDispatcher
    {
        private readonly Dictionary<string, INarrativeActionHandler> _handlers
            = new();

        public NarrativeEventDispatcher(IEnumerable<INarrativeActionHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                _handlers[handler.ActionId] = handler;
            }
        }

        public void Dispatch(DialogEvent ev)
        {
            if (_handlers.TryGetValue(ev.Action, out var handler))
            {
                handler.Execute(ev);
            }
            else
            {
                Debug.LogWarning($"No handler found for action: {ev.Action}");
            }
        }
    }
}