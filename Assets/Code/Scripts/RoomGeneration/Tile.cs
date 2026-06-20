using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Hazards")]
    public List<GameObject> hazards = new() { null };

    [Range(0f, 1f)]
    [SerializeField] private float baseHazardChance = 0.5f;

    private EventBinding<RoomCreatedEvent> roomCreatedEventBinding;

    void OnEnable()
    {
        roomCreatedEventBinding = new EventBinding<RoomCreatedEvent>(PlaceHazards);
        EventBus<RoomCreatedEvent>.Register(roomCreatedEventBinding);
    }

    private void OnDisable()
    {
        EventBus<RoomCreatedEvent>.Deregister(roomCreatedEventBinding);
    }

    public void PlaceHazards(RoomCreatedEvent e)
    {
        foreach (Transform spawnPoint in e.HazardSpawnPoints)
        {
            if (Random.value > GetFinalHazardChance(e.ID))
                continue;

            int hazardIndex = Random.Range(0, hazards.Count);
            GameObject hazardPrefab = hazards[hazardIndex];

            if (hazardPrefab == null)
                continue;

            GameObject hazard = Instantiate(hazardPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            hazard.transform.localScale = hazardPrefab.transform.localScale / 10;
        }
    }

    private float GetFinalHazardChance(int roomIndex)
    {
        float intensity = GameState.Instance.GetHazardIntensity(roomIndex);
        return Mathf.Clamp01(baseHazardChance + intensity * 0.5f);
    }
}
