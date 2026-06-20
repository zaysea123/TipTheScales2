using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [Header("Passive Generation")]
    public int passiveThreshold = 80;
    public float passiveInterval = 5f;
    public int passivePointsPerInterval = 2;
    private float passiveTimer = 0f;

    [Header("Scale Values")]
    [Range(-1f, 1f)]
    public float Scale = 0f;

    [Range(-1f, 1f)]
    public float ScaleTreshold = -0.8f;

    [Header("Perma Death")]
    public bool InPermaDeathRange => Scale <= ScaleTreshold;

    public bool MatchEnded = false;

    [Header("Combat Points")]
    public CombatPoints GlobalCombatPoints
    {
        get
        {
            CombatPoints total = new();

            foreach (CombatPoints room in RoomCombatPointsList)
            {
                total.angelPoints += room.angelPoints;
                total.demonPoints += room.demonPoints;
            }

            return total;
        }
    }
    public List<CombatPoints> RoomCombatPointsList;

    [Header("Variables")]
    public float GameTime = 0f;

    public bool IsPaused = false;

    public int CurrentRoomId = -1;

    public EventBinding<EnemyDiedEvent> enemyDiedBinding;

    #region Singleton Setup
    public static GameState Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            // Registers the first valid instance before the rest of the scene startup flow.
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    private void OnEnable()
    {
        enemyDiedBinding = new EventBinding<EnemyDiedEvent>(EnemyDeath);
        EventBus<EnemyDiedEvent>.Register(enemyDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<EnemyDiedEvent>.Deregister(enemyDiedBinding);
    }

    private void Update()
    {
        GameTime += Time.deltaTime;

        if (!GameMode.IsMultiplayer)
            CalculateScale();

        // TODO: Make sure scale is gathered from the server in multiplayer

        TickPassiveGeneration();

        if (!GameMode.IsMultiplayer && Scale >= 1f)
        {
            MatchEndMenu.Instance.OpenMenu(true);
            MatchEnded = true;
            EventBus<PlayerWonEvent>.Raise(new PlayerWonEvent());
        }
    }

    private void TickPassiveGeneration()
    {
        passiveTimer += Time.deltaTime;

        if (passiveTimer < passiveInterval) return;
        passiveTimer = 0f;

        for (int i = 0; i < RoomCombatPointsList.Count; i++)
        {
            CombatPoints room = RoomCombatPointsList[i];
            int difference = room.angelPoints - room.demonPoints;

            if (difference >= passiveThreshold)
                room.angelPoints += passivePointsPerInterval;
            else if (difference <= -passiveThreshold)
                room.demonPoints += passivePointsPerInterval;

            RoomCombatPointsList[i] = room;
        }
    }

    private void CalculateScale()
    {
        float pointsLight = GlobalCombatPoints.angelPoints;
        float pointsDark = GlobalCombatPoints.demonPoints;

        float rawScore = (pointsLight - pointsDark) / 100f;

        float clamped = Mathf.Clamp(rawScore, -1f, 1f);
        float newScore = (float)System.Math.Round(clamped, 2);

        Scale = newScore;
    }

    private void EnemyDeath(EnemyDiedEvent e)
    {
        //Debug.Log("GameState received enemy death event");
        if(e.RoomID < 0 || e.RoomID >= RoomCombatPointsList.Count)
        {
            return;
        }

        CombatPoints room = RoomCombatPointsList[e.RoomID];
        room.angelPoints += e.Points;
        RoomCombatPointsList[e.RoomID] = room;
    }

    [ContextMenu("Toggle Paused State")]
    public void TogglePauseGame()
    {
        IsPaused = !IsPaused;

        EventBus<PauseGameStateChangedEvent>.Raise(new PauseGameStateChangedEvent()
        {
            IsPaused = IsPaused,
        });

        if (!GameMode.IsMultiplayer)
            Time.timeScale = IsPaused ? 0f : 1f;
    }

    public float GetHazardIntensity(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= RoomCombatPointsList.Count)
            return 0f;

        CombatPoints room = RoomCombatPointsList[roomIndex];

        float total = room.angelPoints + room.demonPoints;
        if (total <= 0f)
            return 0f;

        float ratio = (float)(room.demonPoints - room.angelPoints) / total;

        return Mathf.Clamp(ratio * 0.5f + 0.5f, 0f, 1f);
    }
}

[System.Serializable]
public struct CombatPoints
{
    public int angelPoints;
    public int demonPoints;
}