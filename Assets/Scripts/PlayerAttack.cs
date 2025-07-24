using UnityEngine;
using UnityEngine.Analytics;

public class PlayerAttack : MonoBehaviour
{
    private GameObject AttackArea = default; //attack area
    private bool Attacking = false;
    private float TimeToAttack = 0.25f;
    private float Timer = 0f;

    public Animator animator; //for the attack animation

    void Start()
    {
        AttackArea = transform.GetChild(0).gameObject;
        AttackArea.SetActive(false); //makes sure the attack area is disabled at the start
    }

    void Update()
    {
        //accpt input only if not currently attacking
        //left click to attack
        if (!Attacking && Input.GetMouseButtonDown(0))
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
        //attack animation
        animator.SetTrigger("Attack");
    }
}
