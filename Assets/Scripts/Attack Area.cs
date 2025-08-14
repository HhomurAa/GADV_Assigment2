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
        Collider2D AttackCollider = GetComponent<Collider2D>();
        if (AttackCollider == null)
        {
            return;
        }

        //get all overlapping colliders inside this attack area collider
        ContactFilter2D Filter = new ContactFilter2D();
        Filter.useTriggers = true; // allow trigger colliders
        Collider2D[] Results = new Collider2D[20];
        int hitCount = AttackCollider.Overlap(Filter, Results);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = Results[i];
            if (hit == null) continue;

            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null && !HitEnemies.Contains(enemy))
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Damage(DamageAmount);
                    HitEnemies.Add(enemy); //prevent double damage
                }
            }
        }
    }
}