using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject AttackArea = default; //attack area
    private AttackArea AttackAreaScript;
    private bool Attacking = false;
    private float TimeToAttack = 0.25f;
    private float Timer = 0f;

    public Animator animator; //for the attack animation

    public int AttackEnergyCost = 5;

    private PlayerMovement playerMovement;
    private HMEBar hmeBar;



    //for other script to check
    public bool IsAttacking => Attacking;

    void Start()
    {
        AttackArea = transform.GetChild(0).gameObject;
        AttackArea.SetActive(false); //makes sure the attack area is disabled at the start

        AttackAreaScript = AttackArea.GetComponent<AttackArea>();

        playerMovement = GetComponent<PlayerMovement>();
        hmeBar = FindFirstObjectByType<HMEBar>();
    }

    void Update()
    {
        //accpt input only if not currently attacking
        //left click to attack
        if (!Attacking && Input.GetMouseButtonDown(0))
        {
            if (playerMovement != null && playerMovement.CurrentEnergy >= AttackEnergyCost)
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
        if (playerMovement != null)
        {
            playerMovement.UseEnergy(AttackEnergyCost);
        }

        if (AttackAreaScript != null)
        {
            AttackAreaScript.ResetDamageFlag();
        }

        Attacking = true;
        AttackArea.SetActive(Attacking);
        //attack animation
        animator.SetTrigger("Attack");
    }
}
