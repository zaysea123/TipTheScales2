using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int width = 5;
    public int height = 5;
    public float tileSize = 6f;
    private List<Transform> enemySpawnPoints = new();
    private List<Transform> hazardSpawnPoints = new();
    private int roomID;

    public void SetID(int id)
    {
        roomID = id;
    }

    public void Create(Vector3 position)
    {
        // remove any existing tiles before creating new ones
        Debug.Log($"Creating room {roomID} at position {position}!");
        ClearTiles();

        bool hasValidPrefab = roomPrefabs != null && roomPrefabs.Length > 0 && roomPrefabs[0] != null;
        if (!hasValidPrefab)
            return;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpawnRoom(x - 2f, y - 2f, position);
            }
        }

        EventBus<RoomCreatedEvent>.Raise(new RoomCreatedEvent
        {
            ID = roomID,
            EnemySpawnPoints = enemySpawnPoints,
            HazardSpawnPoints = hazardSpawnPoints,
        });
    }

    public void ClearTiles()
    {
        // clear spawn lists
        enemySpawnPoints.Clear();
        hazardSpawnPoints.Clear();

        // destroy child tile instances
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
            {
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }
        }
    }


    private void SpawnRoom(float coord_x, float coord_y, Vector3 room_position)
    {
        int prefabIndex = Random.Range(0, roomPrefabs.Length);
        GameObject prefab = roomPrefabs[prefabIndex];

        Vector3 spawnLocation = room_position + new Vector3(coord_x * tileSize, 0f, coord_y * tileSize);
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, transform);
        instance.name = "Tile_" + coord_x + "_" + coord_y;
        instance.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);

        Transform enemyRoot = instance.transform.Find("EnemySpawnPoints");
        if (enemyRoot != null)
        {
            foreach (Transform spawnPoint in enemyRoot)
            {
                enemySpawnPoints.Add(spawnPoint);
            }
        }

        Transform hazardRoot = instance.transform.Find("HazardSpawnPoints");
        if (hazardRoot != null)
        {
            foreach (Transform spawnPoint in hazardRoot)
            {
                hazardSpawnPoints.Add(spawnPoint);
            }
        }
    }
}
