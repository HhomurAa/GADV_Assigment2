using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    public enum AbilityType
    {
        Fireball,
        Laserbeam
    }

    [SerializeField] private AbilityType abilityType = AbilityType.Fireball;
    [SerializeField] private float Cooldown = 3f;
    [SerializeField] private Transform FirePoint;

    [Header("Fireball")]
    [SerializeField] private GameObject FireballPrefab;
    [SerializeField] private int FireballManaCost = 10;

    [Header("Laserbeam")]
    [SerializeField] private Laserbeam LaserbeamScript;
    [SerializeField] private int LaserbeanManaCost = 10;
    public GameObject LaserbeamPrefab;

    [Header("Other")]
    [SerializeField] private int MaxMana = 100;
    [SerializeField] private float ManaRegen = 3f;

    private float CurrentMana;
    private float CooldownTimer = 0f;

    [SerializeField] private HMEBar hmeBar;
    private GameObject ActiveLaser;

    private void Start()
    {
        CurrentMana = MaxMana;
        //get HMEBar if not assigned
        if (hmeBar == null)
        {
            hmeBar = FindFirstObjectByType<HMEBar>();
        }

        if (hmeBar != null)
        {
            hmeBar.SetMaxMana(MaxMana);
            hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
            //instantiate slider at full cooldown
            hmeBar.SetFireBallCooldown(Cooldown);
            hmeBar.UpdateFireballCooldown(CooldownTimer);
        }

        if (LaserbeamScript != null)
        {
            LaserbeamScript.enabled = false;
        }
    }

    private void Update()
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= Time.deltaTime;
        }

        if (CooldownTimer < 0)
        {
            CooldownTimer = 0;
        }

        if (hmeBar != null)
        {
            hmeBar.UpdateFireballCooldown(CooldownTimer);
        }

        //mana regen
        if (CurrentMana < MaxMana)
        {
            CurrentMana += ManaRegen * Time.deltaTime;
            if (CurrentMana > MaxMana)
            {
                CurrentMana = MaxMana;
            }

            if (hmeBar != null)
                hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
        }

        //cast ability
        if (abilityType == AbilityType.Laserbeam)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartLaser();
            }
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                StopLaser();
            }
        }

        if (abilityType == AbilityType.Fireball && Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateAbility();
        }

    }

    public void ActivateAbility()
    {
        //wont activate if theres cooldown
        if (CooldownTimer > 0)
        {
            return;
        }

        //checks for which ability the player currently has
        switch (abilityType)
        {
            case AbilityType.Fireball:
            {
                if (CurrentMana >= FireballManaCost)
                {
                    CurrentMana -= FireballManaCost;
                    if (hmeBar != null) hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
                    ActivateFireball();
                    CooldownTimer = Cooldown;
                }
                break;
            }
            case AbilityType.Laserbeam:
            {
                if (CurrentMana > 0 && LaserbeamPrefab != null)
                {
                    //instantiate the laser
                    GameObject LaserObj = Instantiate(LaserbeamPrefab, FirePoint.position, FirePoint.rotation);

                    Laserbeam Laser = LaserObj.GetComponent<Laserbeam>();
                    if (Laser != null)
                    {
                        Laser.StartLaser(this);
                    }
                    CooldownTimer = Cooldown;
                }
                break;
            }
        }
    }
    private void ActivateFireball()
    {
        if (FireballPrefab != null && FirePoint != null)
        {
            //rotate the prefab to align with player direction
            Instantiate(FireballPrefab, FirePoint.position, FirePoint.rotation);
        }
    }

    public bool ConsumeMana(float Amount)
    {
        if (CurrentMana < Amount)
        {
            return false;
        }

        CurrentMana -= Amount;

        if (hmeBar != null)
        {
            hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
        }
        return true;
    }

    public Transform GetFirePoint()
    {
        return FirePoint;
    }

    public int GetLaserbeamManaCost()
    {
        return LaserbeanManaCost;
    }

    private void StartLaser()
    {
        if (CurrentMana <= 0 || LaserbeamPrefab == null || ActiveLaser != null)
            return;

        //lock movement
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.SetCastingLaser(true);   

        ActiveLaser = Instantiate(LaserbeamPrefab, FirePoint.position, FirePoint.rotation);
        Laserbeam Laser = ActiveLaser.GetComponent<Laserbeam>();
        if (Laser != null)
        {
            Laser.StartLaser(this, true); 
        }
    }

    private void StopLaser()
    {
        if (ActiveLaser != null)
        {
            Destroy(ActiveLaser);
            ActiveLaser = null;
        }

        //unlock movement
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.SetCastingLaser(false);
    }
}