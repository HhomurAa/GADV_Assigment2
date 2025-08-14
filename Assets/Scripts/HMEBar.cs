using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HMEBar : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider ManaSlider;
    public Slider EnergySlider;
    public Slider FireballCooldownSlider;

    private float MaxCooldownTime;

    public void SetMaxHealth(int health)
    {
        HealthSlider.maxValue = health;
        HealthSlider.value = health;
    }
    public void SetHealth(int health)
    {
        HealthSlider.value = health;
    }

    public void SetMaxMana(int mana)
    {
        ManaSlider.maxValue = mana;
        ManaSlider.value = mana;
    }
    public void SetMana(int mana)
    {
        ManaSlider.value = mana;
    }

    public void SetMaxEnergy(int energy)
    {
        EnergySlider.maxValue = energy;
        EnergySlider.value = energy;
    }
    public void SetEnergy(int energy)
    {
        EnergySlider.value = energy;
    }

    public void SetFireBallCooldown(float Cooldown)
    {
        MaxCooldownTime = Cooldown;

        if (FireballCooldownSlider != null)
        {
            FireballCooldownSlider.wholeNumbers = false;
            FireballCooldownSlider.maxValue = Cooldown;
            FireballCooldownSlider.minValue = 0f;
            FireballCooldownSlider.value = Cooldown; //start full
        }
    }

    public void UpdateFireballCooldown(float Remaining)
    {
        if (FireballCooldownSlider != null)
        {
            FireballCooldownSlider.value = Remaining;
        }
    }
}
