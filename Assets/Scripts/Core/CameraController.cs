using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    [SerializeField] Transform dayPlayer;
    [SerializeField] Transform nightPlayer;

    [Header("Camera Follow Settings")]
    [SerializeField] float followSpeed = 5f;
    [SerializeField] Vector2 offset = Vector2.zero;

    [Header("Look Ahead")]
    [SerializeField] float aheadDistance = 2f;
    [SerializeField] float aheadSpeed = 3f;

    Transform currentTarget;
    float lookAheadX;

    void Start()
    {
        currentTarget = WorldShiftManager.isDay ? dayPlayer : nightPlayer;

        // Subscribe to day/night change event
        WorldShiftEvents.OnWorldShift += OnWorldShift;
    }

    void OnDestroy()
    {
        // Always unsubscribe
        WorldShiftEvents.OnWorldShift -= OnWorldShift;
    }

    void FixedUpdate()
    {
        if (currentTarget == null) return;

        float direction = Mathf.Sign(currentTarget.localScale.x);
        float targetLookAhead = aheadDistance * direction;
        lookAheadX = Mathf.Lerp(lookAheadX, targetLookAhead, Time.deltaTime * aheadSpeed);

        Vector3 targetPosition = new Vector3(
            currentTarget.position.x + lookAheadX + offset.x,
            currentTarget.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }

    void OnWorldShift(bool isDay)
    {
        currentTarget = isDay ? dayPlayer : nightPlayer;
        Debug.Log($"Camera now following: {currentTarget.name}");
    }
}