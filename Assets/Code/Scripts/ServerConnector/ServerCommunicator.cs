using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ServerCommunicator : MonoBehaviour
{
    private const int RequiredRooms = 3;

    [Header("Server Settings")]
    [SerializeField] private string url = "http://145.93.81.29:8080";
    [SerializeField] private string clientId = "unityClient1";
    [SerializeField] private float heartbeatInterval = 5f;

    private bool IsMultiplayer => GameMode.IsMultiplayer;

    private static GameData CreateEmptyGameData()
    {
        return new GameData
        {
            score = 0,
            objectives = RequiredRooms,
            obj1 = new Objective(),
            obj2 = new Objective(),
            obj3 = new Objective(),
        };
    }

    private static void CopyRoomToObjective(CombatPoints room, Objective objective)
    {
        objective.light = room.angelPoints;
        objective.dark = room.demonPoints;
    }

    private static void CopyObjectiveToRoom(Objective objective, CombatPoints room)
    {
        room.angelPoints = objective.light;
        room.demonPoints = objective.dark;
    }

    private IEnumerator Start()
    {
        if (!IsMultiplayer)
        {
            Debug.Log("Server off, Singleplayer On");
            yield break;
        }

        Debug.Log("Server on, Singleplayer Off");

        yield return SendResetRequest();
        StartCoroutine(HeartbeatLoop());

        yield return new WaitForSeconds(1f);

        yield return SendGameData(CreateEmptyGameData());
    }

    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return SendCurrentGameData();
            yield return new WaitForSeconds(heartbeatInterval);
        }
    }

    private IEnumerator SendCurrentGameData()
    {
        var rooms = GameState.Instance.RoomCombatPointsList;

        if (rooms == null || rooms.Count < RequiredRooms)
        {
            Debug.LogError("Not enough rooms for sync");
            yield break;
        }

        GameData data = CreateEmptyGameData();

        for (int i = 0; i < RequiredRooms; i++)
        {
            CopyRoomToObjective(rooms[i], GetObjective(data, i));
        }

        yield return SendGameData(data);
    }

    private Objective GetObjective(GameData data, int index)
    {
        return index switch
        {
            0 => data.obj1,
            1 => data.obj2,
            2 => data.obj3,
            _ => null,
        };
    }

    private IEnumerator SendResetRequest()
    {
        ResetRequest reset = new()
        {
            objectives = RequiredRooms
        };

        yield return SendRequest(
            $"{url}/reset/{clientId}",
            JsonUtility.ToJson(reset),
            response => Debug.Log("Reset Response: " + response)
        );
    }

    private IEnumerator SendGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        yield return SendRequest(
            $"{url}/{clientId}",
            json,
            response =>
            {
                Debug.Log("Server Response: " + response);

                GameData updated = JsonUtility.FromJson<GameData>(response);

                if (updated != null)
                {
                    Debug.Log("New Score: " + updated.score);
                    ApplyToGameStateData(updated);
                }
            }
        );
    }

    private IEnumerator SendRequest(string endpoint, string json, System.Action<string> onSuccess)
    {
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new(endpoint, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request Failed: " + request.error);
            yield break;
        }

        onSuccess?.Invoke(request.downloadHandler.text);
    }

    private void ApplyToGameStateData(GameData data)
    {
        var state = GameState.Instance;

        if (state == null)
        {
            Debug.LogError("GameState is null");
            return;
        }

        var rooms = state.RoomCombatPointsList;

        while (rooms.Count < RequiredRooms)
        {
            rooms.Add(new CombatPoints());
        }

        CopyObjectiveToRoom(data.obj1, rooms[0]);
        CopyObjectiveToRoom(data.obj2, rooms[1]);
        CopyObjectiveToRoom(data.obj3, rooms[2]);
    }
}

[System.Serializable]
public class Objective
{
    public int light;
    public int dark;
}

[System.Serializable]
public class GameData
{
    public float score;
    public int objectives;

    public Objective obj1;
    public Objective obj2;
    public Objective obj3;
}

[System.Serializable]
public class ResetRequest
{
    public int objectives;
}