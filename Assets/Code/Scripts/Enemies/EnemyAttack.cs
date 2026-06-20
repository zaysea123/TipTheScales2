using AudioSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] Material attackIndicator;
    [SerializeField] Renderer rend;
    EnemyFSM fsm;
    EnemyStats stats;
    NavMeshAgent agent;
    private bool isAttacking = false;
    public string attackSound = "";
    private void Start()
    {
        fsm = GetComponent<EnemyFSM>();
        stats = fsm.stats;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (fsm.currentState == EnemyFSM.State.Attack && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }
    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        //Debug.Log(stats.enemyName + " winds up attack...");
        rend.material = attackIndicator;
        Vector3 directionToPLayer = (fsm.player.position - transform.position).normalized;
        yield return new WaitForSeconds(stats.anticipationTime);

        if(attackSound != "")
            SoundManager.instance.CreateSound().WithSoundData(attackSound).WithPosition(transform.position).WithrandomPitch().Play();

        if (stats.useLunge)
        {
            agent.enabled = false;
            float lungeDuration = 0.15f;
            float timer = 0f;
            while (timer < lungeDuration)
            {
                transform.position += directionToPLayer * stats.lungeForce * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            agent.enabled = true;

            float distanceAfterLunge = Vector3.Distance(transform.position, fsm.player.position);
            if (distanceAfterLunge <= stats.damageRange)
            {
                Health health = fsm.player.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.TakeDamage(stats.attackDamage);
                    Debug.Log(stats.enemyName + " lunge hit! Distance was: " + distanceAfterLunge);
                }
            }
            else
            {
                Debug.Log(stats.enemyName + " lunge missed! Distance was: " + distanceAfterLunge);
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, fsm.player.position);
            if (distanceToPlayer <= stats.attackRange)
            {
                Health health = fsm.player.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.TakeDamage(stats.attackDamage);
                }
                else
                {
                    Debug.LogError("Health component NOT found on " + fsm.player.name);
                }
            }
        }
            

        fsm.lastAttackTime = Time.time;
        fsm.currentState = EnemyFSM.State.Recover;
        isAttacking = false;
        //Debug.Log(stats.enemyName + " attack sequence COMPLETED");
    }
}
