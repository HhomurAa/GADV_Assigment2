using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AbilityData")]
public class NewMonoBehaviourScript : ScriptableObject
{
    public string AbilityName;
    public float Cooldown;
    public int Damage;
    public float Range;
    public float ManaCost;
}