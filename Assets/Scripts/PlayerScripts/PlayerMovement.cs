using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private float movement;

    [Header("Double Jump Settings")]
    [SerializeField] private int maxJumps = 2; // double jump
    private int jumpCount = 0;
    private bool jumpRequested = false;
    [SerializeField] private float jumpCooldown = 0.1f;
    private float lastJumpTime;

    [Header("Ground Check")]
    public bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

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
    #endregion

    #region Unity Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");
        UpdateSpriteDirection();

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        // Slide input
        slideInput = Input.GetKey(KeyCode.F);

        // Ground pound input
        if (Input.GetKeyDown(groundPoundKey))
            TryGroundPound();

        CheckSlope();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        HandleSliding();
        ResetJumpOnGround();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        if (!isSliding && !isSlideJumping && !isGroundPounding)
            rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

        // Slide movement
        if (isSliding && !isSlideJumping)
            Slide();

        // Slide jump momentum
        if (isSlideJumping)
        {
            rb.linearVelocity = new Vector2(retainedHorizontalSpeed, rb.linearVelocity.y);
            slideJumpTimer -= Time.fixedDeltaTime;
            if (slideJumpTimer <= 0f)
                isSlideJumping = false;
        }

        // Ground pound downward velocity
        if (isGroundPounding)
            rb.linearVelocity = new Vector2(0, groundPoundSpeed);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
    }
    #endregion

    #region Custom Methods
    private void UpdateSpriteDirection()
    {
        if (movement > 0)
            sprite.flipX = true; // facing right
        else if (movement < 0)
            sprite.flipX = false; // facing left
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime < jumpCooldown || isGroundPounding)
            return;

        // Slide jump
        if (isSliding)
        {
            retainedHorizontalSpeed = rb.linearVelocity.x;
            slideJumpTimer = slideJumpRetentionTime;
            isSlideJumping = true;
            isSliding = false;
        }

        if (isGrounded)
            jumpCount = 0; // Reset jumps on ground

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
        else if (!slideInput || slopeDownAngle <= maxSlopeAngle || !isGrounded)
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

            Debug.DrawRay(hit.point, hit.normal, Color.red);
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
        }
        else
        {
            slopeDownAngle = 0f;
        }

        Debug.DrawRay(checkPos, Vector2.down * slopeCheckDistance, Color.green);
    }

    private void Slide()
    {
        Vector2 slideDirection = slopeNormalPerp.normalized;
        Vector2 targetVelocity = slideDirection * maxSlideSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, slideAcceleration * Time.fixedDeltaTime);
    }

    private void ResetJumpOnGround()
    {
        if (isGrounded && !isGroundPounding)
            jumpCount = 0;
    }

    private void TryGroundPound()
    {
        if (isGrounded || isSliding)
            return;

        if (Time.time - lastGroundPoundTime < groundPoundCooldown)
            return;

        StartGroundPound();
    }

    private void StartGroundPound()
    {
        isGroundPounding = true;
        lastGroundPoundTime = Time.time;
        LivesCount.SetInvulnerable(true);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;

        if (anim != null)
            anim.SetTrigger("GroundPound");
    }

    private void StopGroundPound()
    {
        isGroundPounding = false;
        LivesCount.SetInvulnerable(false);
    }

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("GroundPound", isGroundPounding);
        }
    }

    public bool IsFacingRight()
    {
        return sprite.flipX;
    }
    #endregion

    #region World Shift Support
    public void CopyStateTo(PlayerMovement target)
    {
        if (target == null)
            return;

        // Try to assign missing components safely
        if (target.rb == null)
            target.rb = target.GetComponent<Rigidbody2D>();
        if (target.sprite == null)
            target.sprite = target.GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        // Copy transform and motion
        target.transform.position = transform.position;
        if (rb != null && target.rb != null)
            target.rb.linearVelocity = rb.linearVelocity;

        // Copy facing direction
        if (sprite != null && target.sprite != null)
            target.sprite.flipX = sprite.flipX;

        // Copy general state
        target.isGrounded = isGrounded;
        target.jumpCount = jumpCount;

        // Copy optional movement states
        target.isSliding = isSliding;
        target.isSlideJumping = isSlideJumping;
        target.retainedHorizontalSpeed = retainedHorizontalSpeed;
        target.slideJumpTimer = slideJumpTimer;
        target.isGroundPounding = isGroundPounding;
    }
    #endregion

}
