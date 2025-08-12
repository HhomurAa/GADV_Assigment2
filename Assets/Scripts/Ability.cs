using UnityEngine;

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

    private float CooldownTimer = 0f;
   
    void Update()
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= Time.deltaTime;
        }

        //cast ability
        if (Input.GetKeyDown(KeyCode.R))
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
    }

    private void ActivateFireball()
    {
        if (FireballPrefab != null && FirePoint != null)
        {
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
