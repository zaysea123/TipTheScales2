using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed;
    float damage;
    float homingStrength;
    Transform target;

    public void Initialize(Transform playerTransform, float projectileHoming, float projectileSpeed, float projectileDamage)
    {
        target = playerTransform;
        speed = projectileSpeed;
        damage = projectileDamage;
        homingStrength = projectileHoming;
    }
    public void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, directionToTarget, homingStrength * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Destroy(gameObject, 1.8f);
    }

}
