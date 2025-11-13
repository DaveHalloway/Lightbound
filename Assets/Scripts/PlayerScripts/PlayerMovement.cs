using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private float movement;

    [Header("Double Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    private int jumpCount = 0;
    private bool jumpRequested = false;
    [SerializeField] private float jumpCooldown = 0.1f;
    private float lastJumpTime;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;
    public bool isGrounded;
    private bool wasGrounded;

    [Header("Sliding Settings")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 5f;
    [SerializeField] private float maxSlideSpeed = 8f;
    [SerializeField] private float slideAcceleration = 20f;
    [SerializeField] private LayerMask slopeLayer;
    [SerializeField] private float groundCheckOffset = 0.1f;

    private bool isSliding;
    private bool slideInput;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;

    [Header("Slide Jump Settings")]
    [SerializeField] private float slideJumpRetentionTime = 0.3f;
    private float slideJumpTimer = 0f;
    private float retainedHorizontalSpeed = 0f;
    private bool isSlideJumping = false;

    [Header("Ground Pound Settings")]
    [SerializeField] private float groundPoundSpeed = -25f;
    [SerializeField] private float groundPoundCooldown = 1f;
    [SerializeField] private KeyCode groundPoundKey = KeyCode.S;
    private bool isGroundPounding = false;
    private float lastGroundPoundTime;

    [Header("Ground Pound Impact Settings")]
    [SerializeField] private float impactRadius = 3f;
    [SerializeField] private int impactDamage = 1;
    [SerializeField] private float impactKnockback = 10f;
    [SerializeField] private float stunDuration = 1.0f;
    [SerializeField] private LayerMask enemyLayers;

    public bool IsFacingRight => !sprite.flipX;

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");
        UpdateSpriteDirection();

        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        slideInput = Input.GetKey(KeyCode.F);

        if (Input.GetKeyDown(groundPoundKey))
            TryGroundPound();

        CheckSlope();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        HandleSliding();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);

        if (isGroundPounding && isGrounded && !wasGrounded)
            StopGroundPound();

        if (!isSliding && !isSlideJumping && !isGroundPounding)
            rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

        if (isSliding && !isSlideJumping)
            Slide();

        if (isSlideJumping)
        {
            rb.linearVelocity = new Vector2(retainedHorizontalSpeed, rb.linearVelocity.y);
            slideJumpTimer -= Time.fixedDeltaTime;
            if (slideJumpTimer <= 0f)
                isSlideJumping = false;
        }

        if (isGroundPounding)
            rb.linearVelocity = new Vector2(0, groundPoundSpeed);

        if (isGrounded && !isGroundPounding)
            jumpCount = 0;
    }

    public void InitializeComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void UpdateSpriteDirection()
    {
        if (movement > 0)
            sprite.flipX = true;
        else if (movement < 0)
            sprite.flipX = false;
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime < jumpCooldown || isGroundPounding) return;

        if (isSliding)
        {
            retainedHorizontalSpeed = rb.linearVelocity.x;
            slideJumpTimer = slideJumpRetentionTime;
            isSlideJumping = true;
            isSliding = false;
        }

        if (isGrounded) jumpCount = 0;

        if (jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            lastJumpTime = Time.time;
            isGrounded = false;
        }
    }

    private void HandleSliding()
    {
        if (slideInput && isGrounded && slopeDownAngle > maxSlopeAngle)
        {
            if (!isSliding && !isSlideJumping)
                isSliding = true;
        }
        else
        {
            isSliding = false;
        }
    }

    private void CheckSlope()
    {
        Vector2 checkPos = (Vector2)transform.position - new Vector2(0f, groundCheckOffset);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, slopeLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            if (slopeNormalPerp.y > 0)
                slopeNormalPerp *= -1f;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
        else
        {
            slopeDownAngle = 0f;
        }
    }

    private void Slide()
    {
        Vector2 slideDirection = slopeNormalPerp.normalized;
        Vector2 targetVelocity = slideDirection * maxSlideSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, slideAcceleration * Time.fixedDeltaTime);
    }

    private void TryGroundPound()
    {
        if (isGrounded || isSliding) return;
        if (Time.time - lastGroundPoundTime < groundPoundCooldown) return;
        StartGroundPound();
    }

    private void StartGroundPound()
    {
        isGroundPounding = true;
        lastGroundPoundTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;
        if (anim != null)
            anim.SetTrigger("GroundPound");
    }

    private void StopGroundPound()
    {
        isGroundPounding = false;
        rb.linearVelocity = Vector2.zero;
        if (anim != null)
            anim.SetTrigger("GroundPoundLand");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, impactRadius, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(impactDamage);

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 dir = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(dir * impactKnockback, ForceMode2D.Impulse);
            }

            EnemyMovement move = enemy.GetComponent<EnemyMovement>();
            if (move != null)
                move.Stun(stunDuration);
        }
    }

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("GroundPound", isGroundPounding);
            anim.SetBool("Grounded", isGrounded);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }

    // Copy state to another player safely
    public void CopyStateTo(PlayerMovement other)
    {
        if (other == null) return;

        // Ensure components are initialized
        other.InitializeComponents();
        this.InitializeComponents();

        other.rb.linearVelocity = this.rb.linearVelocity; // Fixed Unity 2D
        other.transform.position = this.transform.position;
        other.jumpCount = this.jumpCount;
        other.isGrounded = this.isGrounded;
        other.isSliding = this.isSliding;
        other.slideJumpTimer = this.slideJumpTimer;

        // Copy sprite direction safely
        if (this.sprite != null && other.sprite != null)
            other.sprite.flipX = this.sprite.flipX;
    }


    public void SetFacing(bool facingRight)
    {
        if (sprite != null)
            sprite.flipX = !facingRight; // matches your IsFacingRight logic
    }


}
