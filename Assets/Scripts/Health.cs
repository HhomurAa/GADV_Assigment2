using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 100;
    private int MAX_HEALTH = 100;
    [SerializeField] private HMEBar hmeBar;

    private SpriteRenderer spriteRenderer;
    private Color OriginalColor;

    public int CurrentHealth => health;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            OriginalColor = spriteRenderer.color;
        }
    }

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
        //when the player is parrying, no damage is taken
        Parry parry = GetComponent<Parry>();
        if (parry != null && parry.IsParrying)
        {
            UnityEngine.Debug.Log($"{gameObject.name} is parrying! No damage taken.");
            return;
        }

        //find how much damage done to what enemy
        StackTrace stackTrace = new StackTrace(true);
        UnityEngine.Debug.Log($"Damage({amount}) called on {gameObject.name}\nStack trace:\n{stackTrace}");

        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        health -= amount;
        health = Mathf.Clamp(health, 0, MAX_HEALTH);

        //flash red for enemy or player
        if (spriteRenderer != null)
        {
            FlashOnHit();
        }

        //update health bar
        if (hmeBar != null)
        {
            hmeBar.SetHealth(health);
        }

        if(health <= 0)
        {
            HandleDeath();
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

    public void FlashOnHit()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        //makes user flash red
        spriteRenderer.color = Color.red;
        //wait before continuing
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = OriginalColor;
    }
    public void RestoreFull()
    {
        health = MAX_HEALTH;
        if (hmeBar != null)
        {
            hmeBar.SetHealth(health);
        }
    }

    private void HandleDeath()
    {
        //if object is enemy let Enemy class handle death
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Die();
            return;
        }

        //fallback for non-enemy objects
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}