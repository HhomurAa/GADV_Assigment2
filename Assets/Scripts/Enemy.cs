using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int Damage = 5;
    [SerializeField]
    private float Speed = 5f;
    [SerializeField]
    private float DetectionRange = 5f;
    [SerializeField]
    private float StopDistance = 2.5f;
    [SerializeField]
    private GameObject AttackIndicatorPrefab;
    [SerializeField]
    private EnemyData Data;
    [SerializeField]
    private Transform AttackArea;

    private GameObject Player;
    private bool IsAttacking = false;
    private GameObject CurrentIndicator;
    private ConeFill coneFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        SetEnemyValues();
    }

    // Update is called once per frame
    void Update()
    {
        //exits when there is no player
        if (Player == null) return;

        float DistanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        //calls MoveTowardsPlayer when in detection range and stops a certain distance before the player
        if (DistanceToPlayer <= DetectionRange && DistanceToPlayer > StopDistance && !IsAttacking)
        {
            MoveTowardsPlayer();
        }

        //attack if within distance
        if (DistanceToPlayer <= StopDistance)
        {
            if (!IsAttacking)
            {
                StartAttackIndicator();
            }
        }
        else
        {
            ResetAttack();
        }

        //track indicator during attack
        if (IsAttacking)
        {
            TrackAttackIndicator();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        //moves the enemy towards the player
        transform.position += (Vector3)(direction * Speed * Time.deltaTime);
    }

    private void SetEnemyValues()
    {
        GetComponent<Health>().SetHealth(Data.Hp, Data.Hp);
        Damage = Data.Damage;
        Speed = Data.Speed;

    }

    private void StartAttackIndicator()
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

    private void TrackAttackIndicator()
    {
        if (CurrentIndicator == null || Player == null) return;

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

    private void ExecuteAttack()
    {
        if (Player != null && Vector2.Distance(transform.position, Player.transform.position) <= StopDistance)
        {
            Health PlayerHealth = Player.GetComponent<Health>();
            if (PlayerHealth != null)
            {
                PlayerHealth.Damage(Damage);
            }
        }

        //reset and repeat attack if still in range
        if (coneFill != null)
        {
            coneFill.ResetFill();
            coneFill.StartFilling(); //refill and reattack automatically
        }
    }

    private void ResetAttack()
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
        if (collider.CompareTag("Player"))
        {
            Health health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.Damage(Damage);
            }
        }
    }
}
