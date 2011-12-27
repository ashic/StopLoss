using System;
using System.Collections.Generic;

namespace Tests
{
    public class Bus
    {
        public static List<object> Messages = new List<object>();
        static readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public static void Send(object message)
        {
            Messages.Add(message);

            if (_handlers.ContainsKey(message.GetType()))
                _handlers[message.GetType()](message);
        }

        public static void Clear()
        {
            Messages.Clear();
            _handlers.Clear();
        }

        public static void RegisterHandler<T>(Action<T> handler)
        {
            _handlers[typeof (T)] = x=>handler((T)x);
        }
    }
}