using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Laserbeam : MonoBehaviour
{
    public int DamagePerSecond = 3;
    public float RotationOffset = 0f;

    private Ability ability;
    private bool IsFiring = false;
    private BoxCollider2D LaserCollider;
    private Transform FirePoint;

    private Dictionary<GameObject, float> DamageTracker = new Dictionary<GameObject, float>();

    private void Awake()
    {
        LaserCollider = GetComponent<BoxCollider2D>();
        if (LaserCollider != null)
        {
            LaserCollider.enabled = false;
        }
    }
    public void StartLaser(Ability AbilityReference, bool HoldToFire = false)
    {
        ability = AbilityReference;
        if (ability != null)
        {
            FirePoint = ability.GetFirePoint();
        }
        else
        {
            FirePoint = null;
        }

        if (!IsFiring)
        {
            StartCoroutine(FireLaser(HoldToFire));
        }
    }

    private IEnumerator FireLaser(bool HoldToFire)
    {
        IsFiring = true;

        //enable laser collider
        if (LaserCollider != null)
        {
            LaserCollider.enabled = true;
        }

        while (HoldToFire && ability != null)
        {
            //keep rotation in sync with firepoint
            if (FirePoint != null)
            {
                transform.SetPositionAndRotation(FirePoint.position, FirePoint.rotation * Quaternion.Euler(0f, 0f, RotationOffset - 90f));
            }

            //consume mana over time
            if (!ability.ConsumeMana(ability.GetLaserbeamManaCost() * Time.deltaTime))
            {
                break;
            }

            //find all colliders in the laser
            if (LaserCollider != null)
            {
                Vector2 BoxCenter = transform.TransformPoint(LaserCollider.offset);
                Vector2 BoxSize = new Vector2(Mathf.Abs(LaserCollider.size.x * transform.lossyScale.x), Mathf.Abs(LaserCollider.size.y * transform.lossyScale.y));

                //detect enemies
                Collider2D[] Hits = Physics2D.OverlapBoxAll(BoxCenter, BoxSize, transform.eulerAngles.z);
                Debug.Log($"Laser detected {Hits.Length} colliders"); // check how many colliders are in the box

                //track which enemies took damage that second
                HashSet<GameObject> DamagedEnemies = new HashSet<GameObject>();

                foreach (var Hit in Hits)
                {
                    Debug.Log($"Hit object: {Hit.gameObject.name}"); // see the names of detected objects

                    if (Hit.gameObject == ability.gameObject)
                    {
                        continue;
                    }

                    Enemy enemy = Hit.GetComponentInParent<Enemy>();
                    if (enemy == null)
                    {
                        continue;
                    }

                    GameObject EnemyObject = enemy.gameObject;

                    //avoid duplicate damage in same frame
                    if (DamagedEnemies.Contains(EnemyObject))
                    {
                        continue;
                    }
                    DamagedEnemies.Add(EnemyObject);

                    Health health = enemy.GetComponent<Health>();
                    if (health == null)
                    {
                        continue;
                    }

                    //initialize tracker if not present
                    if (!DamageTracker.ContainsKey(EnemyObject))
                    {
                        DamageTracker[EnemyObject] = 0f;
                    }

                    // Add fractional damage
                    DamageTracker[EnemyObject] += DamagePerSecond * Time.deltaTime;

                    //apply whole damage points
                    if (DamageTracker[EnemyObject] >= 1f)
                    {
                        int DamageToApply = Mathf.FloorToInt(DamageTracker[EnemyObject]);
                        DamageTracker[EnemyObject] -= DamageToApply;
                        health.Damage(DamageToApply);
                    }
                }
            }
            yield return null;
        }

        //disable laser
        if (LaserCollider != null)
        {
            LaserCollider.enabled = false;
        }

        //unlock movement when laser stoos
        PlayerMovement playerMovement = ability.GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.SetCastingLaser(false);
        {
            IsFiring = false;
        }

        //destroy laser after
        Destroy(gameObject);
    }
}