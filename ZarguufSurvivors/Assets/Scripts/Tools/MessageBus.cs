using System;
using System.Collections.Generic;


public static class MessageBus
{

    private static readonly Dictionary<Type, Delegate> subscribers = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);
        if (subscribers.ContainsKey(eventType))
        {
            subscribers[eventType] = Delegate.Combine(subscribers[eventType], listener);
        }
        else
        {
            subscribers[eventType] = listener;
        }
    }

    public static void Unsubscribe<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);
        if (subscribers.ContainsKey(eventType))
        {
            subscribers[eventType] = Delegate.Remove(subscribers[eventType], listener);
            if (subscribers[eventType] == null)
            {
                subscribers.Remove(eventType);
            }
        }
    }

    public static void Publish<T>(T eventToPublish) where T : IEvent
    {
        Type eventType = typeof(T);
        if (subscribers.ContainsKey(eventType))
        {
            var eventListeners = subscribers[eventType] as Action<T>;

            eventListeners?.Invoke(eventToPublish);
        }
    }
}

public interface IEvent { }
