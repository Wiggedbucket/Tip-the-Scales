using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;
    private List<Vector3> enemySpawnPoints = new();
    private int roomID;

    public void Initialize(int id)
    {
        roomID = id;
    }

    public async Task Create(Vector3 position)
    {
        bool hasValidPrefab = roomPrefabs != null && roomPrefabs.Length > 0 && roomPrefabs[0] != null;
        if (!hasValidPrefab)
        {
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnRoom(x, y, position);
                await Task.Delay(500);
            }
        }

        EventBus<RoomCreatedEvent>.Raise(new RoomCreatedEvent
        {
            ID = roomID,
            EnemySpawnPoints = enemySpawnPoints
        });
    }

    private void SpawnRoom(int x, int y, Vector3 position)
    {
        GameObject prefab = roomPrefabs[0];
        if (prefab.transform.Find("EnemySpawnPoints") != null)
        {
            foreach (Transform spawnPoint in prefab.transform.Find("EnemySpawnPoints"))
            {
                Vector3 worldSpawnPoint = position + new Vector3(x * tileSize * 6, 0f, y * tileSize * 6) + spawnPoint.localPosition;
                enemySpawnPoints.Add(worldSpawnPoint);
            }
        }
        tileSize = prefab.transform.localScale.x;
        Vector3 spawnLocation = position + new Vector3(x * tileSize * 6, 0f, y * tileSize * 6);
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        instance.name = "Tile_" + x + "_" + y;
    }
}
