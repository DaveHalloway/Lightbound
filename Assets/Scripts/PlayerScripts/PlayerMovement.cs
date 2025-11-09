using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    public bool isGrounded;

    [Header("Double Jump Settings")]
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private float doubleJumpCooldown = 0.25f;
    [SerializeField] private int maxJumps = 2;
    private int jumpCount;
    private bool canDoubleJump = true;
    private float lastJumpTime;

    [Header("Sliding Settings")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 5f;
    [SerializeField] private float maxSlideSpeed = 8f;
    [SerializeField] private float slideAcceleration = 20f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckOffset = 1f;

    private bool isSliding;
    private bool slideInput;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;

    [Header("Slide Jump Settings")]
    [SerializeField] private float slideJumpRetentionTime = 0.3f;
    private float slideJumpTimer = 0f;
    private float retainedHorizontalSpeed = 0f;
    private bool isSlideJumping = false;

    private bool jumpRequested;
    private float movement;

    // ?? GROUND POUND VARIABLES
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
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");
        UpdateSpriteDirection();

        // ?? GROUND POUND INPUT
        if (Input.GetKeyDown(groundPoundKey))
        {
            TryGroundPound();
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        // Slide input
        slideInput = Input.GetKey(KeyCode.F);

        // Check slope
        CheckSlope();

        // Handle jump
        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        // Determine if should start or stop sliding
        if (slideInput && isGrounded && slopeDownAngle > maxSlopeAngle)
        {
            if (!isSliding && !isSlideJumping)
                isSliding = true;
        }
        else if (!slideInput || slopeDownAngle <= maxSlopeAngle || !isGrounded)
        {
            StopSlide();
        }

        // Apply horizontal input if not sliding or during slide-jump
        if (!isSliding && !isSlideJumping && !isGroundPounding)
        {
            rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);
        }

        // Reset jump count when grounded
        if (isGrounded && !jumpRequested)
        {
            jumpCount = 0;
            canDoubleJump = true;

            // ?? If we were ground pounding, stop once we hit ground
            if (isGroundPounding)
                StopGroundPound();
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // Slide movement with smooth acceleration
        if (isSliding && !isSlideJumping)
        {
            Slide();
        }

        // Slide-jump horizontal momentum
        if (isSlideJumping)
        {
            rb.linearVelocity = new Vector2(retainedHorizontalSpeed, rb.linearVelocity.y);
            slideJumpTimer -= Time.fixedDeltaTime;
            if (slideJumpTimer <= 0f)
                isSlideJumping = false;
        }

        // ?? Ground pound downward force
        if (isGroundPounding)
        {
            rb.linearVelocity = new Vector2(0, groundPoundSpeed);
        }
    }
    #endregion

    #region Custom Methods
    private void UpdateSpriteDirection()
    {
        if (movement > 0f)
            sprite.flipX = true;
        else if (movement < 0f)
            sprite.flipX = false;
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime < doubleJumpCooldown)
            return;

        if (isGroundPounding)
            return; // can't jump while pounding

        // Jumping from slide
        if (isSliding)
        {
            retainedHorizontalSpeed = rb.linearVelocity.x;
            slideJumpTimer = slideJumpRetentionTime;
            isSlideJumping = true;
            StopSlide();
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount = 1;
            canDoubleJump = true;
            lastJumpTime = Time.time;
        }
        else if (canDoubleJump && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            jumpCount++;
            canDoubleJump = false;
            lastJumpTime = Time.time;
        }
    }

    private void CheckSlope()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, groundCheckOffset, 0f);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

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
        // Smooth slide acceleration along slope
        Vector2 slideDirection = slopeNormalPerp.normalized;
        Vector2 targetVelocity = slideDirection * maxSlideSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, slideAcceleration * Time.fixedDeltaTime);
    }

    private void StopSlide()
    {
        if (isSliding)
        {
            isSliding = false;
        }
    }

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("GroundPound", isGroundPounding); // optional animator trigger
        }
    }

    // ?? GROUND POUND LOGIC
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

        // make player invulnerable while pounding
        LivesCount.SetInvulnerable(true);

        rb.linearVelocity = Vector2.zero; // reset movement before slamming
        rb.gravityScale = 1f; // ensure gravity is normal

        if (anim != null)
            anim.SetTrigger("GroundPound"); // optional animation trigger
    }

    private void StopGroundPound()
    {
        isGroundPounding = false;
        LivesCount.SetInvulnerable(false); // no longer invulnerable
        Debug.Log("Ground Pound finished");
    }

    public bool IsFacingRight()
    {
        return sprite.flipX;
    }

    public bool CanAttack()
    {
        return isGrounded && movement == 0f;
    }
    #endregion
}
