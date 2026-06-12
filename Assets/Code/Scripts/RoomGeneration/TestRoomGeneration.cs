using UnityEngine;

public class TestRoomGeneration : MonoBehaviour
{
    public bool hastested = false;

    void Start()
    {
        if (hastested)
            return;

        RoomGenerator room = GetComponent<RoomGenerator>();
        if (room != null)
        {
            room.Create(Vector3.zero);
            hastested = true;
        }
    }
}
