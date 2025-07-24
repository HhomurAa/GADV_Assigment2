using UnityEngine;

public class Health : MonoBehaviour
{
    //makes health variable visible and editable
    [SerializeField] private int health = 100;

    private int MAX_HEALTH = 100;
    public void Damage(int amount)
    {
        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        this.health -= amount;

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
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
