using UnityEngine;
using System;
using AudioSystem;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;

    public bool isEnemy = false;
    
    private float currentHealth;

    private bool isDead = false;
    
    public event Action OnDeath;

    public string hitSound = "";
    public string deathSound = "";

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        
        currentHealth -= amount;

        if(hitSound != "")
			      SoundManager.instance.CreateSound().WithSoundData(hitSound).WithPosition(transform.position).WithrandomPitch().Play();

		    //Debug.Log(gameObject.name + " took " + amount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            OnDeath?.Invoke();
            if(deathSound != "")
				        SoundManager.instance.CreateSound().WithSoundData(deathSound).WithPosition(transform.position).WithrandomPitch().Play();
		    }

        EventBus<TookDamageEvent>.Raise(new TookDamageEvent()
        {
            IsEnemy = isEnemy,
            Died = isDead,
        });
        if (isEnemy == false)
        {
            EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
            {
                Amount = -(amount * 0.5f),
                Reason = "Got Hit",
                TextColor = Color.red,
            });
        }
    }

    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
}
