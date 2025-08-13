using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private AttackArea AttackAreaScript;
    private bool Attacking = false;
    private float TimeToAttack = 0.25f;
    private float Timer = 0f;

    public Animator animator; //for the attack animation
    public int AttackEnergyCost = 5;

    private PlayerMovement PlayerMovement;
    private HMEBar hmeBar;

    //for other script to check
    public bool IsAttacking => Attacking;

    void Start()
    {
        AttackAreaScript = transform.GetChild(0).GetComponent<AttackArea>();

        PlayerMovement = GetComponent<PlayerMovement>();
        hmeBar = FindFirstObjectByType<HMEBar>();
    }

    void Update()
    {
        //accpt input only if not currently attacking
        //left click to attack
        if (!Attacking && Input.GetMouseButtonDown(0))
        {
            if (PlayerMovement != null && PlayerMovement.CurrentEnergy >= AttackEnergyCost)
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
            }
        }
    }

    private void Attack()
    {
        if (PlayerMovement != null)
        {
            PlayerMovement.UseEnergy(AttackEnergyCost);
        }

        if (AttackAreaScript != null)
        {
            AttackAreaScript.ResetHits();
            AttackAreaScript.DealDamage();
        }

        Attacking = true;
        Timer = 0f;

        //attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
}

