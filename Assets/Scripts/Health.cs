using UnityEngine;

public class Health : MonoBehaviour
{
    //makes health variable visible and editable
    [SerializeField] private int health = 100;
    private int MAX_HEALTH = 100;

    [SerializeField] private HMEBar hmeBar;

    private void Start()
    {
        if (hmeBar != null)
        {
            hmeBar.SetMaxHealth(MAX_HEALTH);
            hmeBar.SetHealth(health);
        }
    }

    public void Damage(int amount)
    {
        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        this.health -= amount;
        health = Mathf.Clamp(health, 0, MAX_HEALTH);

        //update health bar
        if (hmeBar != null)
        {
            hmeBar.SetHealth(health);
        }

        if(health <= 0)
        {
            Die();
        }
    }

    //set health of enemy
    public void SetHealth(int MaxHealth, int Health)
    {
        this.MAX_HEALTH = MaxHealth;
        this.health = Health;

        if (hmeBar != null)
        {
            hmeBar.SetMaxHealth(MaxHealth);
            hmeBar.SetHealth(health);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
