using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Background References")]
    [SerializeField] private GameObject dayBackground;
    [SerializeField] private GameObject nightBackground;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // Initialize with correct background based on current world state
        UpdateBackground(WorldShiftManager.isDay);

        // Listen for day/night changes
        WorldShiftEvents.OnWorldShift += UpdateBackground;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Keep the background following the camera (but static visually)
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z // keep fixed depth
        );
    }

    private void UpdateBackground(bool isDay)
    {
        if (dayBackground != null)
            dayBackground.SetActive(isDay);

        if (nightBackground != null)
            nightBackground.SetActive(!isDay);
    }

    private void OnDestroy()
    {
        WorldShiftEvents.OnWorldShift -= UpdateBackground;
    }
}
