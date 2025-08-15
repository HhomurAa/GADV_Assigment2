using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int Damage = 5;
    [SerializeField]
    protected float Speed = 5f;
    [SerializeField]
    protected float DetectionRange = 5f;
    [SerializeField]
    protected float StopDistance = 2.5f;

    [SerializeField]
    protected GameObject AttackIndicatorPrefab;
    [SerializeField]
    protected EnemyData Data;
    [SerializeField]
    protected Transform AttackArea;

    [SerializeField] protected int ExpReward = 25; 


    protected GameObject Player;
    protected  bool IsAttacking = false;
    protected  GameObject CurrentIndicator;
    protected  ConeFill coneFill;
    protected  Rigidbody2D RigidBody;

    private const int DamageAmount = 5;

    protected virtual void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        RigidBody = GetComponent<Rigidbody2D>();

        SetEnemyValues();
    }

    protected virtual void Update()
    {
        //exits when there is no player
        if (Player == null) return;

        float DistanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        //attack if within distance
        if (!IsAttacking && DistanceToPlayer <= StopDistance)
        {
            if (!IsAttacking)
            {
                StartAttackIndicator();
            }
        }
        else if (!IsAttacking && DistanceToPlayer > DetectionRange)
        {
            //only resets if player is far away and not attacking
            ResetAttack();
        }

        //track indicator during attack
        if (IsAttacking)
        {
            TrackAttackIndicator();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (Player == null) return;

        float DistanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        //calls MoveTowardsPlayer when in detection range and stops a certain distance before the player
        if (DistanceToPlayer <= DetectionRange && DistanceToPlayer > StopDistance)
        {
            MoveTowardsPlayer();
        }

        else
        {
            //stop movement if outside detection or too close
            RigidBody.linearVelocity = Vector2.zero;
        }
    }

    //moves the enemy towards the player
    protected virtual void MoveTowardsPlayer()
    {
        if (Player == null) return;

        Vector2 Direction = (Player.transform.position - transform.position).normalized;
        Vector2 TargetVelocity = Direction * Speed;

        // Only update velocity along movement direction
        RigidBody.linearVelocity = TargetVelocity;
    }

    protected virtual void SetEnemyValues()
    {
        Health HealthComponent = GetComponent<Health>();
        if (HealthComponent != null)
        {
            HealthComponent.SetHealth(Data.Hp, Data.Hp);
        }

        Damage = Data.Damage;
        Speed = Data.Speed;
    }

    protected virtual void StartAttackIndicator()
    {
        IsAttacking = true;

        //attack warning indicator
        if (AttackIndicatorPrefab != null)
        {
            //calculate direction to player
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

            float DistanceFromEnemy = 2.5f;//distance where the indicator to appears
            Vector3 SpawnOffset = direction * DistanceFromEnemy;
            //instantiate at correct world position
            Vector3 SpawnPosition = transform.position + SpawnOffset;

            //spawn indicator
            CurrentIndicator = Instantiate(AttackIndicatorPrefab, SpawnPosition, Quaternion.Euler(0, 0, angle));

            //set up conefill
            coneFill = CurrentIndicator.GetComponent<ConeFill>();

            if (coneFill != null)
            {
                coneFill.SetAttackRange(StopDistance);
                coneFill.OnFillComplete = ExecuteAttack;
                coneFill.StartFilling();
            }

            //rotate and position attack area
            if (AttackArea != null)
            {
                AttackArea.position = SpawnPosition;
                AttackArea.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    protected virtual void TrackAttackIndicator()
    {
        if (CurrentIndicator == null || Player == null || this == null) return;

        //recalculate
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

        float DistanceFromEnemy = 0.5f;
        Vector3 TrackedOffset = direction * DistanceFromEnemy;
        Vector3 TrackedPosition = transform.position + TrackedOffset;

        //update indicator
        CurrentIndicator.transform.position = TrackedPosition;
        CurrentIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

        //update attack area
        if (AttackArea != null)
        {
            AttackArea.position = TrackedPosition;
            AttackArea.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void ExecuteAttack()
    {
        //damage player if in range
        if (Player != null && Vector2.Distance(transform.position, Player.transform.position) <= StopDistance)
        {
            Health PlayerHealth = Player.GetComponent<Health>();
            if (PlayerHealth != null)
            {
                PlayerHealth.Damage(Damage);
            }
        }

        //check if player is in range to attack
        if (Player != null && Vector2.Distance(transform.position, Player.transform.position) <= StopDistance)
        {
            coneFill.ResetFill();
            coneFill.StartFilling(); //refill and reattack automatically
        }

        else
        {
            ResetAttack(); //if player leaves attack range
        }
    }

    protected virtual void ResetAttack()
    {
        IsAttacking = false;

        //prevents error in case there is nothing
        if (CurrentIndicator != null)
        {
            Destroy(CurrentIndicator);
            CurrentIndicator = null;
            coneFill = null;
        }

        if (AttackArea != null)
        {
            AttackArea.transform.localPosition = Vector3.zero;
            AttackArea.transform.rotation = Quaternion.identity;
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //damage player if hits them
        if (collider.CompareTag("Player"))
        {
            Health health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.Damage(DamageAmount);
            }
        }
    }

    public virtual void Die()
    {
        StopAllCoroutines();
        //prevent update from running
        this.enabled = false;

        if (Player != null)
        {
            Experience PlayerExp = Player.GetComponent<Experience>();
            if (PlayerExp != null)
            {
                PlayerExp.GainExp(ExpReward); //give exp
            }
        }
        Destroy(gameObject);
    }
}