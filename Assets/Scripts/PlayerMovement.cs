using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class PlayerMovement : MonoBehaviour
{
    // how fast the player moves
    public float MoveSpeed = 1f;
    public float DashSpeed = 5f;
    public float DashDuration = 0.2f;
    public Transform Player;

    public int MaxEnergy = 50;
    public int CurrentEnergy = 50;
    public float EnergyRegenRate = 2f;
    private float EnergyAccumulator = 0f;
    public float EnergyRegenDelay = 0.5f;
    private float RegenDelayTimer = 0f;

    public int DashEnergyCost = 5;

    private Vector2 MoveDirection;
    private PlayerAttack playerAttack;
    private HMEBar hmeBar;

    private bool IsDashing = false;
    private float DashTimer = 0f;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        hmeBar = FindFirstObjectByType<HMEBar>();
        if (hmeBar != null)
        {
            hmeBar.SetMaxEnergy(MaxEnergy);
            hmeBar.SetEnergy(CurrentEnergy);
        }
    }

    void Update()
    {
        if (RegenDelayTimer > 0f)
        {
            RegenDelayTimer -= Time.deltaTime;
        }
        //regen energy when not atttacking or dashing
        if (!IsDashing && (playerAttack == null || !playerAttack.IsAttacking) && RegenDelayTimer <= 0f)
        {
            EnergyAccumulator += EnergyRegenRate * Time.deltaTime;

            if (CurrentEnergy < MaxEnergy)
            {
                int EnergyToAdd = Mathf.FloorToInt(EnergyAccumulator);
                if (EnergyToAdd > 0)
                {
                    CurrentEnergy += EnergyToAdd;

                    if (CurrentEnergy > MaxEnergy)
                    {
                        CurrentEnergy = MaxEnergy;
                    }

                    EnergyAccumulator -= EnergyToAdd;

                    UpdateEnergyUI();
                }
            }
        } 
        //stop moving when attacking
        if (playerAttack != null && playerAttack.IsAttacking)
        {
            return;
        }

        if (IsDashing)
        {
            transform.position += (Vector3)(MoveDirection * DashSpeed * Time.deltaTime);
            DashTimer +=  Time.deltaTime;
            if (DashTimer >= DashDuration)
            {
                IsDashing = false;
                DashTimer = 0f;
            }
            return;
        }

        MoveDirection = Vector2.zero; //reset at start of frame
        //movement input
        if (Input.GetKey(KeyCode.W)) MoveDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) MoveDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A)) MoveDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) MoveDirection += Vector2.right;

        //dash input
        if (Input.GetKeyDown(KeyCode.Q) && !IsDashing && MoveDirection != Vector2.zero)
        {
            if (CurrentEnergy >= DashEnergyCost)
            {
                CurrentEnergy -= DashEnergyCost;
                UpdateEnergyUI();
                StartDash();
            }
        }

        //apply rotation when character turns
        if (MoveDirection != Vector2.zero)
        {
            MoveDirection = MoveDirection.normalized;

            //move the entire object
            transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);

            //rotate just the sprite 
            float angle = Mathf.Atan2(MoveDirection.y, MoveDirection.x) * Mathf.Rad2Deg + 90f;
            if (Player != null)
                Player.localRotation = Quaternion.Euler(0, 0, angle); // use localRotation to avoid global shift
        }
    }
    
    void StartDash()
    {
        IsDashing = true;
        DashTimer = 0f;
    }

    public void UseEnergy(int amount)
    {
        CurrentEnergy -= amount;
        if (CurrentEnergy < 0) CurrentEnergy = 0;
        {
            EnergyAccumulator = 0f;

            //reset regen timer
            RegenDelayTimer = EnergyRegenDelay;

            UpdateEnergyUI();
        }
    }

    void UpdateEnergyUI()
    {
        hmeBar.SetEnergy(CurrentEnergy);
    }
}