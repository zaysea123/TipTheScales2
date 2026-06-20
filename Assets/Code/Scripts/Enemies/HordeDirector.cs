using UnityEngine;
using Utilities;

public class HordeDirector : MonoBehaviour
{
    public int id = 0;

    [SerializeField] private EnemySpawner enemySpawner;

    public bool isPlayerInRoom = false;

    private EventBinding<ChangeRoomStateEvent> changeRoomStateEventBinding;

    private EventBinding<AllEnemiesDeadEvent> allEnemiesDeadEventBinding;

    private CombatPoints combatPoints;

    public static int TotalCurrentWave = 0;

    [SerializeField] private int currentWave = 0;
    public int CurrentWave
    {
        get
        {
            // Makes sure that all the rooms don't stray too far from each other in terms of wave count
            if (currentWave + maxWaveDeviation < TotalCurrentWave)
                currentWave = TotalCurrentWave - maxWaveDeviation;
            return currentWave;
        }
    }

    private readonly int maxWaveDeviation = 5;

    private float GameTime => GameState.Instance.GameTime;

    public int baseBudget = 10;

    private void Start()
    {
        combatPoints = GameState.Instance.RoomCombatPointsList[id];
    }

    private void OnEnable()
    {
        changeRoomStateEventBinding = new EventBinding<ChangeRoomStateEvent>(ChangeRoomState);
        EventBus<ChangeRoomStateEvent>.Register(changeRoomStateEventBinding);

        allEnemiesDeadEventBinding = new EventBinding<AllEnemiesDeadEvent>(TryStartWave);
        EventBus<AllEnemiesDeadEvent>.Register(allEnemiesDeadEventBinding);
    }

    private void OnDisable()
    {
        EventBus<ChangeRoomStateEvent>.Deregister(changeRoomStateEventBinding);
        EventBus<AllEnemiesDeadEvent>.Deregister(allEnemiesDeadEventBinding);
    }

    private void ChangeRoomState(ChangeRoomStateEvent e)
    {
        if (id != e.RoomId)
            return;

        isPlayerInRoom = e.IsPlayerInRoom;

        if (isPlayerInRoom)
        {
            // Re-alert enemies that are still walking home
            EnemyFSM[] enemies = FindObjectsByType<EnemyFSM>(FindObjectsSortMode.None);
            foreach (EnemyFSM enemy in enemies)
            {
                if (enemy.roomId == id)
                    enemy.ResumeChase();
            }
            //StartWave();
        }
        else
        {
            EnemyFSM[] enemies = FindObjectsByType<EnemyFSM>(FindObjectsSortMode.None);
            foreach (EnemyFSM enemy in enemies)
            {
                if (enemy.roomId == id)
                    enemy.GoHome();
            }
        }
    }

    [ContextMenu("Start Wave")]
    private void StartWave()
    {
        if (!isPlayerInRoom || !enemySpawner.IsWaveOver())
            return;

        currentWave++;
        TotalCurrentWave++;

        Debug.Log($"Current wave: {CurrentWave}; Total current wave: {TotalCurrentWave}");

        int budget = CalculateBudget();

        StartCoroutine(enemySpawner.SpawnWave(budget, CurrentWave));
    }

    public void TryStartWave(AllEnemiesDeadEvent e)
    {
        StartWave();
    }

    private int CalculateBudget()
    {
        int timeScaling = Mathf.FloorToInt(GameTime / 60f);

        int waveScaling = CurrentWave * 2;

        int scoreDifference = Mathf.Abs(combatPoints.demonPoints - combatPoints.angelPoints);

        return baseBudget
               + timeScaling
               + waveScaling
               + scoreDifference;
    }
}
