using UnityEngine;

public struct InputActionValueChangedEvent : IEvent
{
    public string ActionName;
    public Vector2 Value;
}