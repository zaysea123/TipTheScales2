using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoomEntranceTrigger : MonoBehaviour
{
    [SerializeField] private int roomId = -1;
    [SerializeField] private Transform enterDestination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (enterDestination != null)
            TeleportPlayer(other, enterDestination);

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
            IsPlayerInRoom = true,
        });

        GameState.Instance.CurrentRoomId = roomId;

        //Debug.Log($"Player has entered room {roomId}!");
    }

    private void TeleportPlayer (Collider player, Transform destination )
    {
        StyleMeter.Instance.ResumeDecay();

        if (!player.TryGetComponent<Rigidbody>(out var rb))
            rb = player.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            rb.position = destination.position;
            rb.linearVelocity = Vector3.zero;
        }

        else
        {
            player.transform.position = destination.position;
        }
    }
}
