using UnityEngine;

public class WorldShiftManager : MonoBehaviour
{
    public static bool isDay = true;

    [Header("References")]
    [SerializeField] private GameObject dayObjectsParent;
    [SerializeField] private GameObject nightObjectsParent;

    [Header("Player Prefabs (Separate Day/Night)")]
    [SerializeField] private GameObject dayPlayerPrefab;
    [SerializeField] private GameObject nightPlayerPrefab;

    [Header("Optional Visuals")]
    public Color dayAmbientColor = Color.white;
    public Color nightAmbientColor = new Color(0.1f, 0.1f, 0.3f);
    public float transitionSpeed = 1.5f;

    [Header("Restrictions")]
    public bool canShift = false; // <� new flag!

    void Start()
    {
        UpdateWorldState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && canShift)
        {
            ToggleWorldState();
        }

        // Smoothly blend ambient light color
        RenderSettings.ambientLight = Color.Lerp(
            RenderSettings.ambientLight,
            isDay ? dayAmbientColor : nightAmbientColor,
            Time.deltaTime * transitionSpeed
        );
    }

    public void SetCanShift(bool value)
    {
        canShift = value;
    }

    void ToggleWorldState()
    {
        // Determine current and target player prefabs
        GameObject fromPlayer = isDay ? dayPlayerPrefab : nightPlayerPrefab;
        GameObject toPlayer = isDay ? nightPlayerPrefab : dayPlayerPrefab;

        if (fromPlayer != null && toPlayer != null)
        {
            // Copy world position & rotation
            toPlayer.transform.position = fromPlayer.transform.position;
            toPlayer.transform.rotation = fromPlayer.transform.rotation;

            // --- NEW: Copy sprite facing direction ---
            SpriteRenderer fromRenderer = fromPlayer.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer toRenderer = toPlayer.GetComponentInChildren<SpriteRenderer>();
            if (fromRenderer != null && toRenderer != null)
            {
                toRenderer.flipX = fromRenderer.flipX;
            }

            // Copy Rigidbody2D movement if they have physics
            Rigidbody2D fromRb = fromPlayer.GetComponentInChildren<Rigidbody2D>();
            Rigidbody2D toRb = toPlayer.GetComponentInChildren<Rigidbody2D>();
            if (fromRb && toRb)
            {
                toRb.linearVelocity = fromRb.linearVelocity;
                toRb.angularVelocity = fromRb.angularVelocity;
            }
        }

        // Switch world state
        isDay = !isDay;
        Debug.Log("Switched to " + (isDay ? "Day" : "Night"));

        UpdateWorldState();
        WorldShiftEvents.Invoke(isDay);
    }

    void UpdateWorldState()
    {
        // Toggle environment
        if (dayObjectsParent != null)
            dayObjectsParent.SetActive(isDay);
        if (nightObjectsParent != null)
            nightObjectsParent.SetActive(!isDay);

        // Toggle player prefabs
        if (dayPlayerPrefab != null)
            dayPlayerPrefab.SetActive(isDay);
        if (nightPlayerPrefab != null)
            nightPlayerPrefab.SetActive(!isDay);
    }
}
