using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private int DamageAmount = 5;
    //keeps track of the enemies thats been hit
    private HashSet<Enemy> HitEnemies = new HashSet<Enemy>();

    private void OnEnable()
    {
        HitEnemies.Clear(); //reset on activation
    }

    public void ResetHits()
    {
        HitEnemies.Clear(); //reset hits when attack area activates
    }

    public void DealDamage()
    {
        //check all colliders in the attack area
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2);
        foreach (Collider2D Hit in Hits)
        {
            Enemy enemy = Hit.GetComponentInParent<Enemy>();
            if (enemy != null && !HitEnemies.Contains(enemy))
            {
                Health EnemyHealth = enemy.GetComponent<Health>();
                if (EnemyHealth != null)
                {
                    EnemyHealth.Damage(DamageAmount);
                    HitEnemies.Add(enemy); //mark enemy as hit
                }
            }
        }
    }
}

    