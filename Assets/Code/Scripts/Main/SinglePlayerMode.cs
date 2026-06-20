using UnityEngine;
using System.Collections;

public class SinglePlayerMode : MonoBehaviour
{
    private bool IsMultiplayer;
    public int demonModifier = 0;
    public float demonPointInterval = 2f;
    public float roomSwitchInterval = 10f;
    private int activeRoomIndex = -1;

    private void Start()
    {
        IsMultiplayer = GameMode.IsMultiplayer;
        if (!IsMultiplayer)
        {
            PickNewRoom();
            StartCoroutine(AddPoint());
            StartCoroutine(RoomSwitchTimer());
        }
    }

    private void PickNewRoom()
    {
        var rooms = GameState.Instance.RoomCombatPointsList;
        if (rooms.Count <= 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, rooms.Count);
        }
        while (newIndex == activeRoomIndex && rooms.Count > 1);

        activeRoomIndex = newIndex;
    }

    private IEnumerator AddPoint()
    {
        while (true)
        {
            yield return new WaitForSeconds(demonPointInterval);

            var rooms = GameState.Instance.RoomCombatPointsList;
            if (activeRoomIndex >= 0 && activeRoomIndex < rooms.Count)
            {
                CombatPoints room = rooms[activeRoomIndex];
                room.demonPoints += demonModifier;
                rooms[activeRoomIndex] = room;
            }
        }
    }
    private IEnumerator RoomSwitchTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(roomSwitchInterval);
            PickNewRoom();
        }
       
    }
}
