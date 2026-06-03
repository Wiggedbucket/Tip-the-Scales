using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public void create(Vector3 position)
    {
        transform.position = position;
        for (int i = 0; i < 25; i++)
        {
            GameObject prefab = roomPrefabs[1];
            // GameObject prefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            Vector3 spawnLocation = new Vector3(position.x+(i%5), position.y, position.z+(i/5));
            Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        }
    }
}
