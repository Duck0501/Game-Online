using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public float speed = 5f;
    public float jumpSpeed = 10f;
    public float gravity = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private ChatManager chatManager;

    [Networked, OnChangedRender (nameof(OnHealthChanged))]
    public float NetworkedHealth { get; set; }
    private int Health { get; set; } = 100;
    public Slider healthSlider;

    [Networked] public Vector3 NetworkedVelocity { get; set; }
    [Networked, OnChangedRender(nameof(OnWalkingChanged))] public bool NetworkedWalking { get; set; }
    [Networked, OnChangedRender(nameof(OnIdleChanged))] public bool NetworkedIdle { get; set; }
    [Networked, OnChangedRender(nameof(OnJumpingChanged))] public bool NetworkedJumping { get; set; }
    [Networked, OnChangedRender(nameof(OnAttackingChanged))] public bool NetworkedAttacking { get; set; }
    [Networked] private float ShootDelayTimer { get; set; }

    private Vector3 moveDirection = Vector3.zero;
    private float attackCooldown = 0f;
    private const float ATTACK_DURATION = 2.5f;
    private const float SHOOT_DELAY = 1.31f;

    public override void Spawned()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        chatManager = ChatManager.Instance;
        NetworkedAttacking = false;
        ShootDelayTimer = 0f;

        NetworkedHealth = Health;

        if (healthSlider != null)
        {
            healthSlider.maxValue = Health;
            healthSlider.value = NetworkedHealth;
        }


    }

    private void OnWalkingChanged()
    {
        animator.SetBool("walking", NetworkedWalking);
    }

    private void OnIdleChanged()
    {
        animator.SetBool("idle", NetworkedIdle);
    }

    private void OnJumpingChanged()
    {
        animator.SetBool("jumping", NetworkedJumping);
    }

    private void OnAttackingChanged()
    {
        if (NetworkedAttacking)
        {
            animator.SetTrigger("attack");
        }
    }

    private void OnHealthChanged()
    {
        if (healthSlider != null)
        {
            healthSlider.value = NetworkedHealth;
        }
    }

[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
public void RPC_TakeDamage(float damage)
{
    if (!Object.HasStateAuthority) return;

    NetworkedHealth = Mathf.Max(0, NetworkedHealth - damage);
}

public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (chatManager.chat.activeSelf)
        {
            NetworkedWalking = false;
            NetworkedIdle = true;
            NetworkedJumping = false;
            return;
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical).normalized;

        if (attackCooldown > 0)
        {
            attackCooldown -= Runner.DeltaTime;
            if (attackCooldown <= 0)
            {
                NetworkedAttacking = false;
                ShootDelayTimer = 0f;
                NetworkedIdle = controller.isGrounded && !NetworkedJumping && !NetworkedWalking;
            }
        }

        if (ShootDelayTimer > 0)
        {
            ShootDelayTimer -= Runner.DeltaTime;
            if (ShootDelayTimer <= 0 && NetworkedAttacking)
            {
                if (bulletPrefab != null && firePoint != null)
                {
                    Runner.Spawn(bulletPrefab, firePoint.position, firePoint.rotation);
                }
                else
                {
                    Debug.LogWarning("Bullet Prefab or Fire Point not assigned!");
                }
            }
        }

        if (controller.isGrounded)
        {
            moveDirection = move * speed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
                NetworkedJumping = true;
                NetworkedIdle = false;
            }
            else if (moveDirection.y <= 0)
            {
                NetworkedJumping = false;
            }

            if (Input.GetKeyDown(KeyCode.F) && attackCooldown <= 0 && !NetworkedJumping)
            {
                NetworkedAttacking = true;
                attackCooldown = ATTACK_DURATION;
                ShootDelayTimer = SHOOT_DELAY;
                NetworkedIdle = false;
                NetworkedWalking = false;
            }
        }
        else
        {
            moveDirection.x = move.x * speed;
            moveDirection.z = move.z * speed;
        }

        moveDirection.y -= gravity * Runner.DeltaTime;

        controller.Move(moveDirection * Runner.DeltaTime);

        if (move.magnitude > 0 && controller.isGrounded && !NetworkedAttacking)
        {
            NetworkedWalking = true;
            NetworkedIdle = false;
            transform.rotation = Quaternion.LookRotation(move);
        }
        else
        {
            NetworkedWalking = false;
            NetworkedIdle = !NetworkedJumping && !NetworkedAttacking && controller.isGrounded;
        }
    }
}