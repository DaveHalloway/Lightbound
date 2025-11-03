using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody2D rb;
    SpriteRenderer sprite;

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
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    private void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

        // Handle jump if requested
        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }
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

    private void Jump()
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
    #endregion
}
