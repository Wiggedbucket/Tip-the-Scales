using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;
    private List<Vector3> enemySpawnPoints = new();
    private List<Vector3> hazardSpawnPoints = new();
    private int roomID;

    public void Initialize(int id)
    {
        roomID = id;
    }

    public void Create(Vector3 position)
    {
        bool hasValidPrefab = roomPrefabs != null && roomPrefabs.Length > 0 && roomPrefabs[0] != null;
        if (!hasValidPrefab) { return; }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnRoom(x, y, position);
            }
        }

        EventBus<RoomCreatedEvent>.Raise(new RoomCreatedEvent
        {
            ID = roomID,
            EnemySpawnPoints = enemySpawnPoints,
            HazardSpawnPoints = hazardSpawnPoints,
        });
    }


    private void SpawnRoom(int coord_x, int coord_y, Vector3 room_position)
    {
        int prefabIndex = Random.Range(0, roomPrefabs.Length);
        GameObject prefab = roomPrefabs[prefabIndex];

        if (prefab.transform.Find("EnemySpawnPoints") != null)
        {
            foreach (Transform spawnPoint in prefab.transform.Find("EnemySpawnPoints"))
            {
                Vector3 worldSpawnPoint = room_position + new Vector3(coord_x * tileSize, 0f, coord_y * tileSize) + spawnPoint.localPosition;
                enemySpawnPoints.Add(worldSpawnPoint);
            }
        }
        if (prefab.transform.Find("HazardSpawnPoints") != null)
        {
            foreach (Transform spawnPoint in prefab.transform.Find("HazardSpawnPoints"))
            {
                Vector3 worldSpawnPoint = room_position + new Vector3(coord_x * tileSize, 0f, coord_y * tileSize) + spawnPoint.localPosition;
                hazardSpawnPoints.Add(worldSpawnPoint);
            }
        }

        Vector3 spawnLocation = room_position + new Vector3(coord_x * tileSize, 0f, coord_y * tileSize);
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        instance.name = "Tile_" + coord_x + "_" + coord_y;
    }
}
