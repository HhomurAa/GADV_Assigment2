using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HMEBar : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider ManaSlider;
    public Slider EnergySlider;
    public Slider FireballCooldownSlider;
    public Slider ParryCooldownSlider;
    public Slider ExpSlider;

    public Image AbilityIcon;
    public Sprite FireballSprite;
    public Sprite LaserbeamSprite;

    private float MaxCooldownTime;
    private int MaxExp = 100;

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

    public void SetMaxExp(int exp)
    {
        MaxExp = exp;
        if (ExpSlider != null)
        {
            ExpSlider.maxValue = exp;
            ExpSlider.value = 0; //start empty
        }
    }

    public void SetExp(int exp)
    {
        if (ExpSlider != null)
        {
            ExpSlider.value = exp;
        }
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

    public void SetParryCooldown(float Cooldown)
    {
        if (ParryCooldownSlider != null)
        {
            ParryCooldownSlider.maxValue = Cooldown;
            ParryCooldownSlider.minValue = 0f;
            ParryCooldownSlider.value = Cooldown; 
        }
    }

    public void UpdateParryCooldown(float Remaining)
    {
        if (ParryCooldownSlider != null)
        {
            ParryCooldownSlider.value = Remaining;

        }
    }

    public void SetAbilityIcon(Ability.AbilityType type)
    {
        if (AbilityIcon == null) return;

        //switch based on ability type
        switch (type)
        {
            case Ability.AbilityType.Fireball:
                AbilityIcon.sprite = FireballSprite;
                break;
            case Ability.AbilityType.Laserbeam:
                AbilityIcon.sprite = LaserbeamSprite;
                break;
        }
    }
}
