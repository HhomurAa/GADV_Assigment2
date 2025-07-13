using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject AttackArea = default; //attack area
    private bool Attacking = false;
    private float TimeToAttack = 0.25f;
    private float Timer = 0f;

    void Start()
    {
        AttackArea = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        //left click to attack
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //how long the attack will last
        if (Attacking)
        {
            Timer += Time.deltaTime;

            if (Timer >= TimeToAttack)
            {
                Timer = 0;
                Attacking = false;
                AttackArea.SetActive(Attacking);
            }
        }
    }

    private void Attack()
    {
        Attacking = true;
        AttackArea.SetActive(Attacking);    
    }
}
