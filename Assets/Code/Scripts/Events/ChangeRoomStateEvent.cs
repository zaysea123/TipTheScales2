public struct ChangeRoomStateEvent : IEvent
{
    public int RoomId;

    public bool IsPlayerInRoom;
}
