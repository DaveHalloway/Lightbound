using UnityEngine;

public class WorldShiftManager : MonoBehaviour
{
    public static bool isDay = true;

    [Header("References")]
    [SerializeField] private GameObject dayObjectsParent;
    [SerializeField] private GameObject nightObjectsParent;

    [Header("Player Prefabs (Separate Day/Night)")]
    [SerializeField] private PlayerMovement dayPlayerPrefab;
    [SerializeField] private PlayerMovement nightPlayerPrefab;

    [Header("Optional Visuals")]
    public Color dayAmbientColor = Color.white;
    public Color nightAmbientColor = new Color(0.1f, 0.1f, 0.3f);
    public float transitionSpeed = 1.5f;

    [Header("Restrictions")]
    public bool canShift = false;

    private void Start()
    {
        UpdateWorldState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && canShift)
        {
            ToggleWorldState();
        }

        // Smooth ambient color transition
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

    private void ToggleWorldState()
    {
        PlayerMovement fromPlayer = isDay ? dayPlayerPrefab : nightPlayerPrefab;
        PlayerMovement toPlayer = isDay ? nightPlayerPrefab : dayPlayerPrefab;

        if (fromPlayer != null && toPlayer != null)
        {
            // Copy full player state to the new prefab
            fromPlayer.CopyStateTo(toPlayer);
        }

        // Switch world state
        isDay = !isDay;
        Debug.Log("Switched to " + (isDay ? "Day" : "Night"));

        UpdateWorldState();
        WorldShiftEvents.Invoke(isDay);
    }

    private void UpdateWorldState()
    {
        // Toggle environment
        if (dayObjectsParent != null)
            dayObjectsParent.SetActive(isDay);
        if (nightObjectsParent != null)
            nightObjectsParent.SetActive(!isDay);

        // Toggle player prefabs
        if (dayPlayerPrefab != null)
            dayPlayerPrefab.gameObject.SetActive(isDay);
        if (nightPlayerPrefab != null)
            nightPlayerPrefab.gameObject.SetActive(!isDay);
    }
}
