using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float Speed = 5f;
    public int Damage = 5;
    public float ExplosionRadius = 3f;
    public GameObject ExplosionPrefab;

    void Update()
    {
        //moves the fireball forward
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    //when hits an enemy
    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void Explode()
    {
        //explosion effect
        if (ExplosionPrefab != null)
        {
            GameObject Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(Explosion, 0.5f); //destroy explosion after delay
        }

        //find all colliders in explosion 
        Collider[] Hits = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (Collider hit in Hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                Health EnemyHealth = enemy.GetComponent<Health>();
                if (EnemyHealth != null)
                {
                    EnemyHealth.Damage(Damage);
                }
            }
        }

        Destroy(gameObject);

    }

}
      