using UnityEngine;
using System.Collections;
using AudioSystem;

public class FirePillar : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerTick = 10f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem warningParticles;
    [SerializeField] private ParticleSystem activeParticles;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private float minActiveDuration = 5f;
    [SerializeField] private float maxActiveDuration = 10f;
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float maxInterval = 4f;

    [Header("Scale")]
    private float scaleMultiplier = 0f;
    private bool isActive = false;
    private float nextDamageTime;

    private void Start()
    {
        StartCoroutine(FireCycle());
    }

    private IEnumerator FireCycle()
    {
        yield return new WaitForSeconds(Random.Range(0f, maxInterval)); // Randomize initial delay to desync multiple pillars
        while (true)
        {
            //rest phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Stop();
            float currentInterval = Mathf.Lerp(maxInterval, minInterval, scaleMultiplier);
            yield return new WaitForSeconds(currentInterval);

            //warning phase
            if (warningParticles != null) warningParticles.Play();
            //Debug.Log(gameObject.name + " warning!");
            yield return new WaitForSeconds(warningDuration);

            //active phase
            if (warningParticles != null) warningParticles.Stop();
            if (activeParticles != null) activeParticles.Play();
            isActive = true;
            float currentActiveDuration = Mathf.Lerp(minActiveDuration, maxActiveDuration, scaleMultiplier);
            SoundManager.instance.CreateSound().WithSoundData("LavaPillar").WithPosition(transform.position).WithrandomPitch().Play();
            //Debug.Log(gameObject.name + " ACTIVE!");
            yield return new WaitForSeconds(currentActiveDuration);
            isActive = false;
            if (activeParticles != null) activeParticles.Stop();
            //Debug.Log(gameObject.name + " off.");

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isActive) return;
        if(!other.CompareTag("Player")) return;
        if(Time.time < nextDamageTime) return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerTick);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    public void SetScaleMultiplier(float normalizedscale)
    {
        scaleMultiplier = Mathf.Clamp01(normalizedscale);
    }
}
