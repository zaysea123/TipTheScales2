using System.Collections.Generic;
using UnityEngine;

public struct RoomCreatedEvent : IEvent
{
    public int ID;
    public List<Transform> EnemySpawnPoints;
    public List<Transform> HazardSpawnPoints;
}
