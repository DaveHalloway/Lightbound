using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    #region Variables
    PlayerMovement player;
    [SerializeField] Vector2 boxSize = new Vector2(0.3f, 0.05f);
    [SerializeField] float castDistance = 0.05f;
    [SerializeField] LayerMask groundLayers; // combine Ground + Platform in Inspector
    #endregion

    #region Unity Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
    }
    #endregion

    #region Custom Methods
    // Checks if the player is grounded, and sets the isGrounded variable accordingly in the PlayerMovement script
    void CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.down;

        // Perform a boxcast downward to check for ground
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, direction, castDistance, groundLayers);

        bool grounded = hit.collider != null;
        player.isGrounded = grounded;

        // Debug visualization
        Color rayColor = grounded ? Color.green : Color.red;
        Debug.DrawRay(origin, direction * castDistance, rayColor);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position - Vector3.up * castDistance, boxSize);
    }
    #endregion
}