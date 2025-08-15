using UnityEngine;

public class Experience : MonoBehaviour
{
    public int CurrentExp = 0;
    public int MaxExp = 100; 
    public HMEBar HmeBar;

    private Health PlayerHealth;
    private Ability PlayerMana;
    private PlayerMovement PlayerEnergy;

    private void Awake()
    {
        PlayerHealth = GetComponent<Health>();
        PlayerMana = GetComponent<Ability>();
        PlayerEnergy = GetComponent<PlayerMovement>();

    }

    private void Start()
    {
        if (HmeBar != null)
        {
            HmeBar.SetMaxExp(MaxExp);
            HmeBar.SetExp(CurrentExp);
        }
    }

    public void GainExp(int amount)
    {
        CurrentExp += amount;

        if (CurrentExp >= MaxExp)
        {
            RestorePlayer();
            CurrentExp -= MaxExp; 
        }

        if (HmeBar != null)
        {
            HmeBar.SetExp(CurrentExp);
        }

        Debug.Log("Gained EXP: " + amount + ", Total EXP: " + CurrentExp);
    }

    private void RestorePlayer()
    {
        if (PlayerHealth != null)
        {
            PlayerHealth.RestoreFull();
        }

        if (PlayerMana != null)
        {
            PlayerMana.RestoreFullMana();
        }

        if (PlayerEnergy != null)
        {
            PlayerEnergy.RestoreFullEnergy();
        }
    }
}
