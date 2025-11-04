using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables
    [SerializeField] Transform player;  // The player transform to follow

    [SerializeField] float followSpeed = 5f;  // How fast the camera follows
    [SerializeField] Vector2 offset = Vector2.zero; // Optional offset for fine-tuning

    [SerializeField] float aheadDistance = 2f; // How far ahead camera looks
    [SerializeField] float aheadSpeed = 3f;    // How quickly the lookahead reacts

    float lookAheadX;
    #endregion

    #region Unity Methods
    void FixedUpdate()
    {
        if (player == null) return; // Safety check

        // Calculate horizontal look-ahead based on facing direction
        float targetLookAhead = aheadDistance * Mathf.Sign(player.localScale.x);
        lookAheadX = Mathf.Lerp(lookAheadX, targetLookAhead, Time.deltaTime * aheadSpeed);

        // Target position includes both X and Y from player
        Vector3 targetPosition = new Vector3(
            player.position.x + lookAheadX + offset.x,
            player.position.y + offset.y,
            transform.position.z
        );

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }
    #endregion
}