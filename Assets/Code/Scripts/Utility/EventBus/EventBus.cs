using System.Collections.Generic;
using UnityEngine;

public static class EventBus<T> where T : IEvent
{
    private static readonly HashSet<IEventBinding<T>> bindings = new();
    private static readonly List<IEventBinding<T>> bindingSnapshot = new();

    public static void Register(EventBinding<T> binding) => bindings.Add(binding);
    public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

    // The @ character makes the compiler recognize the code element as an identifier rather than a C# keyword
    public static void Raise(T @event)
    {
        bindingSnapshot.Clear();

        bindingSnapshot.AddRange(bindings);

        foreach (var binding in bindingSnapshot)
        {
            binding.OnEvent?.Invoke(@event);
            binding.OnEventNoArgs?.Invoke();
        }
    }

    static void Clear()
    {
        Debug.Log($"Clearing {typeof(T).Name} bindings");
        bindings.Clear();
        bindingSnapshot.Clear();
    }
}