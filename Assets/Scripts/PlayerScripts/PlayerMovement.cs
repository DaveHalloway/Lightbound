using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator anim;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    public bool isGrounded;

    bool jumpRequested; // boolean to check if the player has requested a jump
    float movement; // horizontal movement input
    #endregion

    #region Unity Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Get references to components
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get horizontal input
        movement = Input.GetAxisRaw("Horizontal");

        UpdateSpriteDirection();

        // Sets jump request if space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

        // Handle jump if requested
        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        UpdateAnimations();
    }

    //// FixedUpdate is called at a fixed interval and is independent of frame rate
    //private void FixedUpdate()
    //{
        
    //}
    #endregion

    #region Custom Methods
    // Updates the sprite direction based on movement input
    private void UpdateSpriteDirection()
    {
        // Flip sprite based on movement direction
        if (movement > 0f) 
        {
            sprite.flipX = true;
        }
        else if (movement < 0f)
        {
            sprite.flipX = false;
        }
    }

    // Handles the jump action

    void Jump()
    {
        // Only jump if the player is grounded
        if (!isGrounded)
        {
            return;
        }
        // Apply jump force
        Debug.Log("Player Jumped");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void UpdateAnimations()
    {
        // Update running animation
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); // run speed
            //anim.SetBool("isGrounded", isGrounded); // jump/land
        }
    }

    public bool IsFacingRight()
    {
        return sprite.flipX; // true if facing right
    }

    public bool CanAttack()
    {
        return isGrounded && movement == 0f; // can attack if grounded and not moving
    }
    #endregion
}
