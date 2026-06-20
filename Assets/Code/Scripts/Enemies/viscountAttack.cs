using UnityEngine;

public class ViscountAttack : MonoBehaviour
{
    [SerializeField] Material attackIndicator;
    [SerializeField] Renderer rend;
    EnemyFSM fsm;
    EnemyStats stats;
    float lastFireTime;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    private void Start()
    {
        fsm = GetComponent<EnemyFSM>();
        stats = fsm.stats;
    }
    private void Update()
    {
        if (fsm.currentState != EnemyFSM.State.Attack) return;
        if (Time.time < fsm.lastKiteEndTime + 0.3f) return;
        if (Time.time < lastFireTime + stats.attackCooldown) return;
        Fire();
        lastFireTime = Time.time;
    
    }

    private void Fire()
    {
        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Initialize(fsm.player, stats.homingStrength, stats.projectileSpeed, stats.attackDamage);
    }
}
