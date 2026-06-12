using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;

    private List<Vector3> enemySpawnPoints = new List<Vector3>();
    private int roomID;

    public void Initialize(int id)
    {
        roomID = id;
    }

    public void Create(Vector3 position)
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
            }
        }

        EventBus<RoomCreatedEvent>.Raise(new RoomCreatedEvent
        {
            ID = roomID,
            EnemeySpawnPoints = enemySpawnPoints
        });
    }


    private void SpawnRoom(int x, int y, Vector3 position)
    {
        int prefabIndex = Random.Range(0, roomPrefabs.Length);
        GameObject prefab = roomPrefabs[prefabIndex];

        if (prefab.transform.Find("EnemySpawnPoints") != null)
        {
            foreach (Transform spawnPoint in prefab.transform.Find("EnemySpawnPoints"))
            {
                Vector3 worldSpawnPoint = position + new Vector3(x * tileSize, 0f, y * tileSize) + spawnPoint.localPosition;
                enemySpawnPoints.Add(worldSpawnPoint);
            }
        }

        tileSize = prefab.transform.localScale.x;
        Vector3 spawnLocation = position + new Vector3(x * tileSize, 0f, y * tileSize);
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        instance.name = "Tile_" + x + "_" + y;
    }
}
