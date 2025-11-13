using UnityEngine;
using System.Collections;

public class WorldShiftManager : MonoBehaviour
{
    public static bool isDay = true;

    [Header("References")]
    [SerializeField] private GameObject dayObjectsParent;
    [SerializeField] private GameObject nightObjectsParent;

    [Header("Player References")]
    [SerializeField] private PlayerMovement dayPlayer;
    [SerializeField] private PlayerMovement nightPlayer;

    [Header("Visuals")]
    public Color dayAmbientColor = Color.white;
    public Color nightAmbientColor = new Color(0.1f, 0.1f, 0.3f);
    public float transitionSpeed = 2f;

    [Header("Restrictions")]
    public bool canShift = false;

    private PlayerMovement activePlayer;
    private PlayerMovement inactivePlayer;
    private bool isTransitioning = false;

    private void Start()
    {
        activePlayer = isDay ? dayPlayer : nightPlayer;
        inactivePlayer = isDay ? nightPlayer : dayPlayer;

        // Initialize both players but only enable the active one
        activePlayer.gameObject.SetActive(true);
        inactivePlayer.gameObject.SetActive(false);

        UpdateWorldObjects();
        RenderSettings.ambientLight = isDay ? dayAmbientColor : nightAmbientColor;
    }

    private void Update()
    {
        // World shift input
        if (Input.GetKeyDown(KeyCode.Tab) && canShift && !isTransitioning)
        {
            StartCoroutine(SmoothWorldShift());
        }

        // Smooth ambient light transition
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

    private IEnumerator SmoothWorldShift()
    {
        if (activePlayer == null || inactivePlayer == null) yield break;

        isTransitioning = true;

        // Copy position, velocity, state, and facing direction
        activePlayer.CopyStateTo(inactivePlayer);

        // Freeze player input while transitioning
        activePlayer.enabled = false;
        inactivePlayer.enabled = false;

        // Activate the other player
        inactivePlayer.gameObject.SetActive(true);

        // Optional: short delay to avoid visual popping
        yield return null;

        // Swap references
        var temp = activePlayer;
        activePlayer = inactivePlayer;
        inactivePlayer = temp;

        isDay = !isDay;

        // Update world objects
        UpdateWorldObjects();

        // Notify any listeners (like camera)
        WorldShiftEvents.Invoke(isDay);

        // Re-enable player input
        activePlayer.enabled = true;

        // Hide inactive player
        inactivePlayer.gameObject.SetActive(false);

        isTransitioning = false;
    }

    private void UpdateWorldObjects()
    {
        if (dayObjectsParent != null)
            dayObjectsParent.SetActive(isDay);
        if (nightObjectsParent != null)
            nightObjectsParent.SetActive(!isDay);
    }
}
