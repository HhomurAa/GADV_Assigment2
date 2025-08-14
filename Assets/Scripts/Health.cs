using System.Diagnostics;
using UnityEngine;

public class Health : MonoBehaviour
{
    //makes health variable visible and editable
    [SerializeField] int health = 100;
    private int MAX_HEALTH = 100;

    [SerializeField] private HMEBar hmeBar;

    public int CurrentHealth => health;

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
        StackTrace stackTrace = new StackTrace(true); // true = capture file info
        UnityEngine.Debug.Log($"Damage({amount}) called on {gameObject.name}\nStack trace:\n{stackTrace}");

        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        this.health -= amount;
        health = Mathf.Clamp(health, 0, MAX_HEALTH);

        //flash red if this obhect is an enemy
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.FlashOnHit();
        }

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
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
