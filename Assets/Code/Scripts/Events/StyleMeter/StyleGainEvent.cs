using UnityEngine.UIElements;

public struct StyleGainEvent : IEvent
{
    public float Amount;
    public string Reason;
    public StyleColor TextColor;
}