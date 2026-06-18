using UnityEngine;

public class EnemyDiedEvent : IEvent
{
    public GameObject EnemyObject;
    public int Points;
}
