using UnityEngine;
using System.Collections.Generic;


public class Fireball : MonoBehaviour
{
    public float Speed = 5f;
    public int Damage = 5;
    public float ExplosionRadius = 3f;
    public float MaxTravelDistance = 10f;
    public GameObject ExplosionPrefab;

    private bool HasExploded = false;
    private Rigidbody2D RigidBody;
    private Vector3 StartPosition;

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        if (RigidBody != null )
        {
            RigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void Start()
    {
        StartPosition = transform.position;

        //ignore collision with the player
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            Collider2D PlayerCollider = Player.GetComponent<Collider2D>();
            Collider2D FireballCollider = GetComponent<Collider2D>();
            if (PlayerCollider != null && FireballCollider != null)
            {
                Physics2D.IgnoreCollision(FireballCollider, PlayerCollider);
            }
        }

        //set fireball velocity forward
        if (RigidBody != null)
        {
            RigidBody.linearVelocity = -transform.up * Speed;
        }
    }

    private void Update()
    {
        //check travel distance
        if (Vector3.Distance(StartPosition, transform.position) >= MaxTravelDistance)
        {
            Explode();
        }
    }

    //when hits an enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HasExploded)
        {
            return;
        }

        //ignore player
        if (other.CompareTag("Player"))
        {
            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            //omly explode once, damaging all enmeies nearby
            Explode();
            HasExploded = true; //prevent multiple explosions

        }
    }

    private void Explode()
    {
        if (HasExploded)
        {
            return;
        }
        HasExploded = true;

        //explosion effect
        if (ExplosionPrefab != null)
        {
            GameObject Explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(Explosion, 0.5f); //destroy explosion after delay
        }

        //track which enemies that took damage
        HashSet<GameObject> DamagedEnemies = new HashSet<GameObject>();
        //find all colliders in explosion 
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        foreach (Collider2D hit in Hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null && !DamagedEnemies.Contains(enemy.gameObject))
            {
                DamagedEnemies.Add(enemy.gameObject); //mark enemy as damaged

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