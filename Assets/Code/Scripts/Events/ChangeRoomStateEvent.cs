public class ChangeRoomStateEvent : IEvent
{
    public int RoomId;

    public bool IsPlayerInRoom;
}
