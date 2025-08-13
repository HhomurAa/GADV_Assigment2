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

    [Header("Laserbeam")]
    [SerializeField] private LineRenderer LaserLine; //draw a line for the laser
    [SerializeField] private float LaserDuration = 1f;

    [SerializeField] private HMEBar hmeBar;

    private float CooldownTimer = 0f;

    private void Start()
    {
        if (hmeBar != null)
        {
            //instantiate slider at full cooldown
            hmeBar.SetFireBallCooldown(Cooldown, Cooldown);
        }
    }
    void Update()
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer < 0f)
            {
                CooldownTimer = 0f;
            }

            if (hmeBar != null)
            {
                hmeBar.UpdateFireballCooldown(Cooldown - CooldownTimer);
            }
        }

        //cast ability
        if (Input.GetKeyDown(KeyCode.Alpha1))
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
                ActivateFireball();
                break;
            }
            case AbilityType.Laserbeam:
            {
                ActivateLaser();
                    break;
            }
        }
        CooldownTimer = Cooldown;
        if (hmeBar != null)
        {
            hmeBar.SetFireBallCooldown(CooldownTimer, Cooldown);
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

    private void ActivateLaser()
    {
        if (LaserLine != null)
        {
            StartCoroutine(FireLaser());
        }
    }

    private System.Collections.IEnumerator FireLaser()
    {
        LaserLine.enabled = true;
        //wait for laser to end
        yield return new WaitForSeconds(LaserDuration); 
        LaserLine.enabled = false;
    }
}