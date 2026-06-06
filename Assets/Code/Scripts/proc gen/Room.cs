using UnityEngine;

public class Room : MonoBehaviour
{
    bool hasGenerated = false;
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;

    public void Create(Vector3 position)
    {
        if (hasGenerated)
            return;

        hasGenerated = true;
        transform.position = position;

        // Remove previous children (useful for repeated testing)
        for (int c = transform.childCount - 1; c >= 0; c--)
        {
            GameObject child = transform.GetChild(c).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        // Prepare a fallback cube prefab if none provided
        GameObject fallback = null;
        bool hasValidPrefab = roomPrefabs != null && roomPrefabs.Length > 0 && roomPrefabs[0] != null;
        if (!hasValidPrefab)
        {
            fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.transform.localScale = Vector3.one * tileSize;
            fallback.SetActive(false);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject prefab = hasValidPrefab ? roomPrefabs[0] : fallback;

                Vector3 spawnLocation = position + new Vector3(x * tileSize, 0f, y * tileSize);
                GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
                instance.name = "Tile_" + x + "_" + y;
                if (fallback != null)
                    instance.SetActive(true);

                Destroy(instance.GetComponent<Room>());
                Destroy(instance.GetComponent<testing>());
            }
        }

        if (fallback != null)
        {
            if (Application.isPlaying)
                Destroy(fallback);
            else
                DestroyImmediate(fallback);
        }
    }
}
