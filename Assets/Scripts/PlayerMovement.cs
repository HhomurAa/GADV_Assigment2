using Unity.VisualScripting;
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

    private Rigidbody2D RigidBody;

    public bool IsCastingLaser { get; private set; } = false;

    public float InteractRange = 2f;

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
    }

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
        EnergyRegen();

        //stop moving when attacking
        if ((playerAttack != null && playerAttack.IsAttacking) || IsCastingLaser)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        if (!IsDashing)
        {
            MoveDirection = Vector2.zero;

            //movement input
            if (Input.GetKey(KeyCode.W)) MoveDirection += Vector2.up;
            if (Input.GetKey(KeyCode.S)) MoveDirection += Vector2.down;
            if (Input.GetKey(KeyCode.A)) MoveDirection += Vector2.left;
            if (Input.GetKey(KeyCode.D)) MoveDirection += Vector2.right;
            MoveDirection = MoveDirection.normalized;

            //dash input
            if (Input.GetKeyDown(KeyCode.Q) && MoveDirection != Vector2.zero && CurrentEnergy >= DashEnergyCost)
            {
                CurrentEnergy -= DashEnergyCost;
                UpdateEnergyUI();
                StartDash();
            }

            //interact input
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        //rotate when character turns
        if (MoveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(MoveDirection.y, MoveDirection.x) * Mathf.Rad2Deg + 90f;
            Player.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void FixedUpdate()
    {
        if (IsDashing)
        {
            RigidBody.linearVelocity = MoveDirection * DashSpeed;
            DashTimer += Time.fixedDeltaTime;
            if (DashTimer >= DashDuration)
            {
                IsDashing = false;
                DashTimer = 0f;
            }
        }
        else if (MoveDirection != Vector2.zero) //only moves when wasd is pressed
        {
            RigidBody.linearVelocity = MoveDirection * MoveSpeed;
        }

        else
        {
            //stop when no input
            RigidBody.linearVelocity = Vector2.zero;
        }
    }

    private void EnergyRegen()
    {
        if (RegenDelayTimer > 0f)
        {
            RegenDelayTimer -= Time.deltaTime;
        }

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
                        CurrentEnergy = MaxEnergy;

                    EnergyAccumulator -= EnergyToAdd;
                    UpdateEnergyUI();
                }
            }
        }
    }

    private void StartDash()
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
            RegenDelayTimer = EnergyRegenDelay; //reset regen timer
            UpdateEnergyUI();
        }
    }

    void UpdateEnergyUI()
    {
        hmeBar.SetEnergy(CurrentEnergy);
    }

    public void RestoreFullEnergy()
    {
        CurrentEnergy = MaxEnergy;
        EnergyAccumulator = 0f;
        UpdateEnergyUI();
    }

    public void SetCastingLaser(bool value)
    {
        IsCastingLaser = value;
    }

    private void Interact()
    {
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, InteractRange);
        foreach (var Hit in Hits)
        {
            Chest chest = Hit.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Open();
                break; 
            }
        }
    }
}