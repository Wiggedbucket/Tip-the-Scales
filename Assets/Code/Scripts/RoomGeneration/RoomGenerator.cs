using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;
    private List<Transform> enemySpawnPoints = new();
    private List<Transform> hazardSpawnPoints = new();
    private int roomID;

    public void Initialize(int id)
    {
        roomID = id;
    }

    void Start()
    {
        Create(transform.position);
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

        Vector3 spawnLocation = room_position + new Vector3(coord_x * tileSize, 0f, coord_y * tileSize);
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        instance.name = "Tile_" + coord_x + "_" + coord_y;
        instance.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);

        Transform enemyRoot = instance.transform.Find("EnemySpawnPoints");
        if (enemyRoot != null)
        {
            foreach (Transform spawnPoint in enemyRoot)
            {
                enemySpawnPoints.Add(spawnPoint);
            }
        }

        Transform hazardRoot = instance.transform.Find("HazardSpawnPoints");
        if (hazardRoot != null)
        {
            foreach (Transform spawnPoint in hazardRoot)
            {
                hazardSpawnPoints.Add(spawnPoint);
            }
        }
    }
}
