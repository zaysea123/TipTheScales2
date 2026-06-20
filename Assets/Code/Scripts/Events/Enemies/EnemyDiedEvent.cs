using UnityEngine;

public struct EnemyDiedEvent : IEvent
{
    public GameObject EnemyObject;
    public int Points;
    public int RoomID;
}
