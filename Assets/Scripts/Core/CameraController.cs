using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    [SerializeField] private Transform dayPlayer;
    [SerializeField] private Transform nightPlayer;

    [Header("Camera Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Look Ahead")]
    [SerializeField] private float aheadDistance = 2f;
    [SerializeField] private float aheadSpeed = 3f;

    private Transform currentTarget;
    private float lookAheadX;
    private float lastTargetX;

    private void Start()
    {
        // Automatically assign target based on current world state
        currentTarget = WorldShiftManager.isDay ? dayPlayer : nightPlayer;
        lastTargetX = currentTarget != null ? currentTarget.position.x : 0f;

        // Subscribe to world state changes
        WorldShiftEvents.OnWorldShift += OnWorldShift;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        WorldShiftEvents.OnWorldShift -= OnWorldShift;
    }

    private void FixedUpdate()
    {
        if (currentTarget == null) return;

        // --- Look-ahead calculation ---
        float deltaX = currentTarget.position.x - lastTargetX;
        lastTargetX = currentTarget.position.x;

        // Smoothly move look-ahead based on direction of motion
        float targetLookAhead = Mathf.Sign(deltaX) * aheadDistance;
        lookAheadX = Mathf.Lerp(lookAheadX, targetLookAhead, Time.deltaTime * aheadSpeed);

        // --- Calculate new camera position ---
        Vector3 desiredPos = new Vector3(
            currentTarget.position.x + lookAheadX + offset.x,
            currentTarget.position.y + offset.y,
            transform.position.z
        );

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
    }

    private void OnWorldShift(bool isDay)
    {
        // Switch camera target when world changes
        currentTarget = isDay ? dayPlayer : nightPlayer;

        if (currentTarget == null)
        {
            Debug.LogWarning("CameraController: Target player not assigned for current state!");
            return;
        }

        // Snap camera immediately to prevent a visible jump
        Vector3 snapPos = new Vector3(
            currentTarget.position.x + offset.x,
            currentTarget.position.y + offset.y,
            transform.position.z
        );
        transform.position = snapPos;

        lastTargetX = currentTarget.position.x;

        Debug.Log($"Camera now following: {currentTarget.name}");
    }
}
