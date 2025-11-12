using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] Vector2 boxSize = new Vector2(0.3f, 0.05f);
    [SerializeField] float castDistance = 0.1f;
    [SerializeField] LayerMask groundLayers;

    public bool isGrounded { get; private set; }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void CheckGround()
    {
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, castDistance, groundLayers);
        isGrounded = hit.collider != null;

        // Debug
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector2.down * castDistance, rayColor);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position - Vector3.up * castDistance, boxSize);
    }
}
