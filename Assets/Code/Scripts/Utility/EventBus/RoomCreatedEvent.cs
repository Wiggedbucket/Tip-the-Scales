using System.Collections.Generic;
using UnityEngine;

public struct RoomCreatedEvent : IEvent
{
    public int ID;
    public List<Vector3> EnemeySpawnPoints;
}

