using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    #region Variables
    GameObject player;
    #endregion

    #region Unity Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
    }
    #endregion

    #region Custom Methods
    // Checks if the player is grounded, and sets the isGrounded variable accordingly in the PlayerMovement script
    private void CheckGround()
    {
        float radius = 0.2f;
        float distance = 0.2f;
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.down;
        LayerMask layerMask = LayerMask.GetMask("Ground");

        // Perform the raycast
        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, distance, layerMask);

        // If the raycast hits a collider on the Ground layer, the player is grounded
        if (hit.collider != null)
        {
            player.GetComponent<PlayerMovement>().isGrounded = true;
        }
        else
        {
            player.GetComponent<PlayerMovement>().isGrounded = false;
        }
    }
    #endregion
}