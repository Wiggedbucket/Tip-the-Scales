using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<GameObject> hazards = new(){ null, };

    private EventBinding<RoomCreatedEvent> roomCreatedEventBinding;

    void OnEnable()
    {
        roomCreatedEventBinding = new EventBinding<RoomCreatedEvent>(PlaceHazards);
        EventBus<RoomCreatedEvent>.Register(roomCreatedEventBinding);
    }
    private void OnDisable()
    {
        EventBus<RoomCreatedEvent>.Deregister(roomCreatedEventBinding);
    }

    public void PlaceHazards(RoomCreatedEvent roomCreatedEvent)
    {
        foreach (Transform spawnPoint in roomCreatedEvent.HazardSpawnPoints)
        {
            int hazardIndex = Random.Range(0, hazards.Count);
            GameObject hazardPrefab = hazards[hazardIndex];
            if (hazardPrefab != null)
            {
                GameObject hazard = Instantiate(hazardPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
                hazard.transform.localScale = hazardPrefab.transform.localScale/10;
            }
        }
    }
}
