using UnityEngine;

public class WorldShiftManager : MonoBehaviour
{
    public static bool isDay = true;

    [Header("References")]
    public GameObject dayObjectsParent;
    public GameObject nightObjectsParent;

    [Header("Optional Visuals")]
    public Color dayAmbientColor = Color.white;
    public Color nightAmbientColor = new Color(0.1f, 0.1f, 0.3f);
    public float transitionSpeed = 1.5f;

    void Start()
    {
        UpdateWorldState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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

    void ToggleWorldState()
    {
        isDay = !isDay;
        Debug.Log("Switched to " + (isDay ? "Day" : "Night"));
        UpdateWorldState();

        // Notify everything listening (camera, enemies, etc.)
        WorldShiftEvents.Invoke(isDay);
    }

    void UpdateWorldState()
    {
        if (dayObjectsParent != null)
            dayObjectsParent.SetActive(isDay);

        if (nightObjectsParent != null)
            nightObjectsParent.SetActive(!isDay);
    }
}
