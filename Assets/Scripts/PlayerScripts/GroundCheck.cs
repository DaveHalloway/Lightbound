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
        //float distance = 0.05f;
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.down;
        LayerMask layerMask = LayerMask.GetMask("Ground");
        LayerMask layerMask1 = LayerMask.GetMask("Platform");

        // Perform the raycast
        RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(0.3f, 0.05f), 0f, direction, 0.05f, layerMask);
        RaycastHit2D hit1 = Physics2D.BoxCast(origin, new Vector2(0.3f, 0.05f), 0f, direction, 0.05f, layerMask1);

        // If the raycast hits a collider on the Ground layer, the player is grounded
        if (hit.collider != null || hit1.collider != null)
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