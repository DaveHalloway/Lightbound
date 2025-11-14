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

    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    private int jumpCount = 0;
    private bool jumpRequested = false;
    [SerializeField] private float jumpCooldown = 0.1f;
    private float lastJumpTime;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;
    private bool isGrounded;
    private bool wasGrounded;

    [Header("Sliding Settings")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 5f;
    [SerializeField] private float slideAcceleration = 20f;
    [SerializeField] private float maxSlideSpeed = 8f;
    [SerializeField] private LayerMask slopeLayer;

    private bool isSliding = false;
    private bool slidePressed = false;
    private float slopeAngle;
    private Vector2 slopeNormalPerp;

    [Header("Slide Jump Settings")]
    [SerializeField] private float slideJumpRetentionTime = 0.25f;
    private float slideJumpTimer = 0f;
    private float retainedHorizontalSpeed = 0f;
    private bool isSlideJumping = false;

    [Header("Ground Pound Settings")]
    [SerializeField] private float groundPoundSpeed = -25f;
    [SerializeField] private float groundPoundCooldown = 1f;
    [SerializeField] private KeyCode groundPoundKey = KeyCode.S;

    private bool isGroundPounding = false;
    private float lastGroundPoundTime = 0f;

    [Header("Natural Gravity")]
    [SerializeField] private float normalGravity = 2f; // always restored after pound

    [Header("Freeze During World Shift")]
    public bool movementFrozen = false;

    public bool IsFacingRight => !sprite.flipX;

    private void Start()
    {
        InitializeComponents();
        rb.gravityScale = normalGravity;
    }

    public void InitializeComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (movementFrozen)
        {
            movement = 0f;
            return;
        }

        movement = Input.GetAxisRaw("Horizontal");
        slidePressed = Input.GetKey(KeyCode.F);

        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        if (Input.GetKeyDown(groundPoundKey))
            TryGroundPound();

        CheckSlope();
        UpdateSpriteDirection();

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

        if (movementFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // If landing from ground pound
        if (isGroundPounding && isGrounded && !wasGrounded)
            StopGroundPound();

        // Normal move
        if (!isSliding && !isSlideJumping && !isGroundPounding)
            rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

        // Sliding
        if (isSliding && !isSlideJumping)
        {
            Vector2 targetVel = slopeNormalPerp * maxSlideSpeed;
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, slideAcceleration * Time.fixedDeltaTime);
        }

        // Slide jump movement retention
        if (isSlideJumping)
        {
            rb.linearVelocity = new Vector2(retainedHorizontalSpeed, rb.linearVelocity.y);
            slideJumpTimer -= Time.fixedDeltaTime;
            if (slideJumpTimer <= 0f)
                isSlideJumping = false;
        }

        // Ground pound fall
        if (isGroundPounding)
            rb.linearVelocity = new Vector2(0, groundPoundSpeed);

        // Restore jump count on landing
        if (isGrounded && !isGroundPounding)
            jumpCount = 0;
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
        if (Time.time - lastJumpTime < jumpCooldown || isGroundPounding)
            return;

        if (isSliding)
        {
            retainedHorizontalSpeed = rb.linearVelocity.x;
            slideJumpTimer = slideJumpRetentionTime;
            isSlideJumping = true;
            isSliding = false;
        }

        if (isGrounded)
            jumpCount = 0;

        if (jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            lastJumpTime = Time.time;
            isGrounded = false;
        }
    }

    private void CheckSlope()
    {
        Vector2 checkPos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, slopeLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            if (slopeNormalPerp.y > 0)
                slopeNormalPerp *= -1f;

            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
        else
        {
            slopeAngle = 0f;
        }
    }

    private void HandleSliding()
    {
        bool steepEnough = slopeAngle > maxSlopeAngle;

        // Player controlled slide ONLY
        if (slidePressed && steepEnough && isGrounded && !isSlideJumping)
            isSliding = true;
        else
            isSliding = false;
    }

    private void TryGroundPound()
    {
        if (isGrounded || isSliding || movementFrozen) return;
        if (Time.time - lastGroundPoundTime < groundPoundCooldown) return;

        StartGroundPound();
    }

    private void StartGroundPound()
    {
        isGroundPounding = true;
        lastGroundPoundTime = Time.time;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = normalGravity; // always reset to clean gravity

        if (anim != null)
            anim.SetTrigger("GroundPound");
    }

    private void StopGroundPound()
    {
        isGroundPounding = false;
        rb.gravityScale = normalGravity;
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger("GroundPoundLand");
    }

    private void UpdateAnimations()
    {
        if (!anim) return;

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("GroundPound", isGroundPounding);
        anim.SetBool("Sliding", isSliding);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    // Called by WorldShiftManager
    public void FreezeMovement(bool value)
    {
        movementFrozen = value;
        if (value)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // For day/night switching
    public void CopyStateTo(PlayerMovement other)
    {
        if (other == null) return;

        other.InitializeComponents();
        this.InitializeComponents();

        other.transform.position = transform.position;
        other.rb.linearVelocity = rb.linearVelocity;

        other.sprite.flipX = sprite.flipX;

        other.jumpCount = jumpCount;
        other.isSliding = isSliding;
        other.isGroundPounding = false;
    }

    public void SetFacing(bool facingRight)
    {
        sprite.flipX = !facingRight;
    }
}
