using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObject/Enemy", order = 1)]

public class EnemyData : ScriptableObject
{
    public int Hp;
    public int Damage;
    public int Speed;
}
