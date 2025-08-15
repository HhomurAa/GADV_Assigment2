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

    [Header("Interaction Settings")]
    [SerializeField] private float PickupRange = 2f; //range to detect ability drops

    private void Start()
    {
        CurrentMana = MaxMana;
        if (hmeBar == null)
        {
            hmeBar = FindFirstObjectByType<HMEBar>();
        }

        if (hmeBar != null)
        {
            hmeBar.SetMaxMana(MaxMana);
            hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
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
        HandleCooldown();
        HandleManaRegen();
        HandleAbilityCasting();
        HandleAbilityPickup();
    }

    private void HandleCooldown()
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer < 0)
            {
                CooldownTimer = 0;
            }
        }

        if (hmeBar != null)
        {
            hmeBar.UpdateFireballCooldown(CooldownTimer);
        }
    }

    private void HandleManaRegen()
    {
        //regen when mana is not full
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
    }

    private void HandleAbilityCasting()
    {
        if (abilityType == AbilityType.Laserbeam)
        {
            //hold down left click to fire laser
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartLaser();
            }

            //stop laser
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

    private void HandleAbilityPickup()
    {
        //change ability when press C over drop item
        if (Input.GetKeyDown(KeyCode.C))
        {
            //get all colliders in within range
            Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, PickupRange);
            foreach (Collider2D Hit in Hits)
            {
                //check if collider has drop data component
                AbilityDropData DropData = Hit.GetComponent<AbilityDropData>();
                if (DropData != null)
                {
                    SwitchAbility(DropData.abilityType); //switch ability
                    Destroy(Hit.gameObject);
                    Debug.Log("Switched to " + DropData.abilityType);
                    break;
                }
            }
        }
    }

    private void SwitchAbility(AbilityType NewAbility)
    {
        abilityType = NewAbility;
        CooldownTimer = 0f;

        if (hmeBar != null)
        {
            hmeBar.SetFireBallCooldown(Cooldown);
            hmeBar.UpdateFireballCooldown(CooldownTimer);

            //update icon
            hmeBar.SetAbilityIcon(NewAbility);
        }
    }

    public void ActivateAbility()
    {
        if (CooldownTimer > 0) return;

        switch (abilityType)
        {
            case AbilityType.Fireball:
                if (CurrentMana >= FireballManaCost)
                {
                    CurrentMana -= FireballManaCost;
                    if (hmeBar != null) hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
                    ActivateFireball();
                    CooldownTimer = Cooldown;
                }
                break;

            case AbilityType.Laserbeam:
                if (CurrentMana > 0 && LaserbeamPrefab != null)
                {
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

    private void ActivateFireball()
    {
        if (FireballPrefab != null && FirePoint != null)
        {
            Instantiate(FireballPrefab, FirePoint.position, FirePoint.rotation);
        }
    }

    public bool ConsumeMana(float amount)
    {
        if (CurrentMana < amount) return false;

        CurrentMana -= amount;
        if (hmeBar != null)
        {
            hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
        }
        return true;
    }

    public Transform GetFirePoint() => FirePoint;
    public int GetLaserbeamManaCost() => LaserbeanManaCost;

    private void StartLaser()
    {
        if (CurrentMana <= 0 || LaserbeamPrefab == null || ActiveLaser != null) return;

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

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null) playerMovement.SetCastingLaser(false);
    }

    public void RestoreFullMana()
    {
        CurrentMana = MaxMana;
        if (hmeBar != null)
        {
            hmeBar.SetMana(Mathf.RoundToInt(CurrentMana));
        }
    }
}
