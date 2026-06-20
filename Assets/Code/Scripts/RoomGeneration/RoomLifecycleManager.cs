using UnityEngine;

public class RoomLifecycleManager : MonoBehaviour
{
    public int roomID;
    public GameObject roomGeneratorPrefab;
    private EventBinding<ChangeRoomStateEvent> changeBinding;

    private void OnEnable()
    {
        changeBinding = new EventBinding<ChangeRoomStateEvent>(OnChangeRoomState);
        EventBus<ChangeRoomStateEvent>.Register(changeBinding);
    }

    private void OnDisable()
    {
        if (changeBinding != null) {
            EventBus<ChangeRoomStateEvent>.Deregister(changeBinding);
        }
    }

    private void OnChangeRoomState(ChangeRoomStateEvent evt)
    {
        if (evt.RoomId != roomID)
            return;

        if (evt.IsPlayerInRoom)
        {
            Debug.Log($"Creating room {evt.RoomId}!");
            CreateRoom(evt.RoomId);
        }
        else
        {
            Debug.Log($"Removing room {evt.RoomId}!");
            RemoveRoom();
        }
    }

    public void CreateRoom(int id, Transform spawnSource = null)
    {
        Vector3 pos = spawnSource != null ? spawnSource.position : transform.position;

        var localRg = GetComponentInChildren<RoomManager>(true);
        if (localRg != null)
        {
            localRg.SetID(id);
            localRg.Create(pos);
        }
    }

    public void RemoveRoom()
    {
        var localRg = GetComponentInChildren<RoomManager>(true);
        if (localRg != null)
        {
            localRg.ClearTiles();
        }
    }
}
