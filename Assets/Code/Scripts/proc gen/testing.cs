using UnityEngine;

public class testing : MonoBehaviour
{
    public bool hastested = false;
    void Start()
    {
        if (hastested)
            return;

        Room room = GetComponent<Room>();
        if (room != null)
        {
            room.Create(Vector3.zero);
            hastested = true;
        }
    }
}
