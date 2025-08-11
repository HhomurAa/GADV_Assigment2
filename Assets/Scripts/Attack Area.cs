using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private int DamageAmount = 5;

    private bool HasDealtDamage = false;

    private void OnEnable()
    {
        HasDealtDamage = false; //resets when attack area activates
    }

    public void ResetDamageFlag()
    {
        HasDealtDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (HasDealtDamage)
        {
            return;
        }

        Enemy enemy = collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Health EnemyHealth = enemy.GetComponent<Health>();
            if (EnemyHealth != null)
            {
                EnemyHealth.Damage(DamageAmount); //apply damage
                HasDealtDamage = true;
            }
        }
    }
}