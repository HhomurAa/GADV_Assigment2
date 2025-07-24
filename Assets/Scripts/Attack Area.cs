using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int Damage = 5;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //checks whether the collided area has a health component
        if(collider.GetComponent<Health>() != null)
        {
            Health health = collider.GetComponent<Health>();
            health.Damage(Damage);
        }
    }
}
