using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoomEntranceTrigger : MonoBehaviour
{
    [SerializeField]
    private int roomId = -1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Execute();
    }

    [ContextMenu("Execute Trigger")]
    private void Execute()
    {
        if (roomId == -1)
            return;

        EventBus<ChangeRoomStateEvent>.Raise(new ChangeRoomStateEvent
        {
            RoomId = roomId,
            IsPlayerInRoom = true
        });

        Debug.Log($"Player has entered room {roomId}!");
    }
}
