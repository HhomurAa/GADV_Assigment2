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
    private float StopDistance = 2f;

    [SerializeField]
    private EnemyData Data;

    private GameObject Player;
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
        if (DistanceToPlayer <= DetectionRange && DistanceToPlayer > StopDistance)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        //moves the enemy towards the player
        transform.position += (Vector3)(direction * Speed * Time.deltaTime);
    }

    //when attack hits player
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //checls if there is a health script attacked
            if (collider.GetComponent<Health>() != null)
            {
                collider.GetComponent<Health>().Damage(Damage);
                this.GetComponent<Health>().Damage(1000);
            }
        }
    }

    private void SetEnemyValues()
    {
        GetComponent<Health>().SetHealth(Data.Hp, Data.Hp);
        Damage = Data.Damage;
        Speed = Data.Speed;

    }
}