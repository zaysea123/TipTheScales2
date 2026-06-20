using UnityEngine;
using UnityEngine.AI;

public class TestDummyEnemy : MonoBehaviour
{
    private NavMeshAgent agent;

    private EventBinding<KillAllEnemiesEvent> killAllEnemiesEventBinding;

    public int roomId;

    private void OnEnable()
    {
        killAllEnemiesEventBinding = new EventBinding<KillAllEnemiesEvent>(Die);
        EventBus<KillAllEnemiesEvent>.Register(killAllEnemiesEventBinding);
    }

    private void OnDisable()
    {
        EventBus<KillAllEnemiesEvent>.Deregister(killAllEnemiesEventBinding);
    }

    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(Vector3.zero);
    }

    private void Die(KillAllEnemiesEvent killAllEnemiesEvent)
    {
        if (killAllEnemiesEvent.RoomId != roomId)
            return;

        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent()
        {
            EnemyObject = gameObject
        });
        Destroy(gameObject);
    }
}
