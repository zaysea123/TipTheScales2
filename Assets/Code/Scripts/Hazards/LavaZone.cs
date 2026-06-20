using UnityEngine;

public class LavaZone : MonoBehaviour
{
    [SerializeField] private float damagePerTick = 10f;
    [SerializeField] private float damageInterval = 1f;
    private float nextDamageTime;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        nextDamageTime = 0f;
        Debug.Log("Player entered lava zone.");
    }

    private void OnTriggerStay(Collider other)
    { 
        if (!other.CompareTag("Player")) return;
        if (Time.time >= nextDamageTime)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damagePerTick);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player exited lava zone.");
    }
}
