using System;

internal interface IEventBinding<T>
{
    int Priority { get; }

    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    public int Priority { get; }

    Action<T> OnEvent = _ => { };
    Action OnEventNoArgs = () => { };

    Action<T> IEventBinding<T>.OnEvent
    {
        get => OnEvent;
        set => OnEvent = value;
    }

    Action IEventBinding<T>.OnEventNoArgs
    {
        get => OnEventNoArgs;
        set => OnEventNoArgs = value;
    }

    public EventBinding(Action<T> onEvent, int priority = 0)
    {
        this.OnEvent = onEvent;
        this.Priority = priority;
    }

    public EventBinding(Action onEventNoArgs, int priority = 0)
    {
        this.OnEventNoArgs = onEventNoArgs;
        this.Priority = priority;
    }

    public void Add(Action onEvent) => OnEventNoArgs += onEvent;
    public void Remove(Action onEvent) => OnEventNoArgs -= onEvent;

    public void Add(Action<T> onEvent) => OnEvent += onEvent;
    public void Remove(Action<T> onEvent) => OnEvent -= onEvent;
}