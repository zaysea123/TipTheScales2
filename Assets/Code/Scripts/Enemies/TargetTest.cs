using UnityEngine;

public class Target : MonoBehaviour
{
    public float health;

    public void takeDamage(float amount)
    {
        health -= amount;

        if(health <= 0 )
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
