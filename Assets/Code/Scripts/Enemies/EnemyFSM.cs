using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public int roomId = -1;

    [SerializeField] private Material basecolour;
    [SerializeField] private Renderer rend;

    public enum State { Idle, Recover, Chase, Attack, Kite, ReturnHome }

    public EnemyStats stats;
    public Transform player;

    public float lastKiteEndTime;
    public State currentState = State.Idle;

    private NavMeshAgent agent;

    public float lastAttackTime;
    private Health health;
    private float combatStoppingDistance;

    Vector3 HomePosition;

    public int points;

    private void Start()
    {
        HomePosition = transform.position;
        health = GetComponentInParent<Health>();
        health.OnDeath += HandleDeath;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene. Please ensure there is a GameObject tagged 'Player'.");
        }

        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.moveSpeed;

        if (stats.canKite)
            agent.stoppingDistance = stats.fireRange * 0.8f;
        else if (stats.useLunge)
            agent.stoppingDistance = stats.lungeRange * 0.8f;
        else
            agent.stoppingDistance = stats.attackRange * 0.8f;

        agent.stoppingDistance = combatStoppingDistance;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.ReturnHome:
                HandleReturnHome();
                break;
            case State.Idle:
                HandleIdle();
                break;
            case State.Recover:
                HandleRecover();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Attack:

                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                directionToPlayer.y = 0f;
                if (directionToPlayer != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);

                if (stats.canKite)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                    if (distanceToPlayer > stats.fireRange)
                    {
                        currentState = State.Chase;
                        break;
                    }

                    if (distanceToPlayer <= stats.kiteDistance)
                    {
                        currentState = State.Kite;
                    }
                }
                break;
            case State.Kite:
                HandleKite();
                break;
        }
    }


    private void HandleReturnHome()
    {
        agent.SetDestination(HomePosition);
        float distanceToHome = Vector3.Distance(transform.position, HomePosition);
        if (distanceToHome < 0.5f)
        {
            agent.SetDestination(transform.position);
            currentState = State.Idle;
        }
    }

    private void HandleIdle()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 100f)
        {
            currentState = State.Chase;
        }
    }

    private void HandleChase()
    {
        if (player == null)
            return;

        agent.SetDestination(player.position);
        float distance = Vector3.Distance(transform.position, player.position);

        float triggerRange = stats.canKite ? stats.fireRange :
                             (stats.useLunge ? stats.lungeRange : stats.attackRange);


        if (distance <= triggerRange)
        {
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;
            currentState = State.Attack;
        }
    }

    private void HandleRecover()
    {
        agent.velocity = Vector3.zero;
        if (Time.time >= lastAttackTime + stats.attackCooldown)
        {
            currentState = State.Chase;
        }
        rend.material = basecolour;
    }

    private void HandleKite()
    {
        agent.stoppingDistance = 0f;
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 kiteTarget = transform.position + directionAwayFromPlayer * 5f;
        agent.SetDestination(kiteTarget);
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance >= stats.kiteDistance * 2f)
        {
            agent.stoppingDistance = stats.fireRange * 0.8f;
            currentState = State.Attack;
            lastKiteEndTime = Time.time;

        }

    }

    private void HandleDeath()
    {
        //Debug.Log($"[EnemyFSM] DIED → RoomID: {roomId}, Points: {points}");
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent()
        {
            EnemyObject = gameObject,
            Points = points,
            RoomID = roomId,
        });
        EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
        {
            Amount = points,
            Reason = "Enemy Dead",
            TextColor = Color.lightYellow,
        });
        Destroy(gameObject);
    }

    public void GoHome()
    {
        agent.stoppingDistance = 0f;
        currentState = State.ReturnHome;
    }
    public void ResumeChase()
    {
        if (currentState == State.ReturnHome)
        {
            agent.stoppingDistance = combatStoppingDistance;
            currentState = State.Chase;
        }
    }
}
