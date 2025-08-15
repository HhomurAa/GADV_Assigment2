using UnityEngine;

public class ArcherEnemy : Enemy
{
    [Header("Archer Specific")]
    [SerializeField] private GameObject ArrowPrefab;
    [SerializeField] private Transform ShootPoint;
    [SerializeField] private float ShootCooldown = 2f;
    [SerializeField] private float ArrowSpeed = 10f;
    [SerializeField] private int ArrowDamage = 5;
    [SerializeField] private float MoveSpeed = 3f;

    private float ShootTimer = 0f;
    private Rigidbody2D rigidBody;
    private Vector2 MoveDirection;

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            rigidBody = gameObject.AddComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0f;
            rigidBody.freezeRotation = true;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (Player == null) return;

        ShootTimer += Time.deltaTime;
        MoveDirection = (Player.transform.position - transform.position).normalized;

        //check if player within range and shot cooldown
        if (Vector2.Distance(transform.position, Player.transform.position) <= DetectionRange && ShootTimer >= ShootCooldown)
        {
            ShootArrow();
            ShootTimer = 0f;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Player != null)
        {
            float distance = Vector2.Distance(transform.position, Player.transform.position);

            //only move towards player when in detection range
            if (distance <= DetectionRange && distance > StopDistance)
            {
                rigidBody.linearVelocity = MoveDirection * MoveSpeed;
            }

            else
            {
                rigidBody.linearVelocity = Vector2.zero;
            }
        }

        else
        {
            rigidBody.linearVelocity = Vector2.zero;
        }
    }

    private void ShootArrow()
    {
        Vector2 direction = (Player.transform.position - ShootPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject arrow = Instantiate(ArrowPrefab, ShootPoint.position, Quaternion.Euler(0f, 0f, angle + 45));

        Collider2D arrowCollider = arrow.GetComponent<Collider2D>();
        if (arrowCollider != null)
        {
            arrowCollider.isTrigger = true;

            // Ignore collisions with all colliders on this archer
            Collider2D[] archerColliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in archerColliders)
                Physics2D.IgnoreCollision(arrowCollider, col, true);
        }

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * ArrowSpeed;
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        ArrowHitHandler hitHandler = arrow.AddComponent<ArrowHitHandler>();
        hitHandler.Damage = ArrowDamage;
        hitHandler.OriginalArcherColliders = GetComponentsInChildren<Collider2D>();
        hitHandler.IsReflected = false;

        Destroy(arrow, 5f);
    }

    private class ArrowHitHandler : MonoBehaviour
    {
        public int Damage;
        public Collider2D[] OriginalArcherColliders;
        public bool IsReflected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;

            Parry parry = other.GetComponent<Parry>();
            if (parry != null && parry.IsParrying && !IsReflected)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    //reverse the arrow back at enemy
                    rb.linearVelocity = -rb.linearVelocity;
                    float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle + 45);
                    IsReflected = true;

                    //enable collisions with original archer after reflection
                    foreach (var col in OriginalArcherColliders)
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col, false);
                }
                return;
            }

            //prevent hitting original archer before reflection
            if (!IsReflected && OriginalArcherColliders != null)
            {
                foreach (var col in OriginalArcherColliders)
                {
                    if (other == col) return;
                }
            }

            if ((other.CompareTag("Player") || other.CompareTag("Enemy")))
            {
                Health health = other.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.Damage(Damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}