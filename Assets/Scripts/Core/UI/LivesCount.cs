using TMPro;
using UnityEngine;
using System.Collections;

public class LivesCount : MonoBehaviour
{
    #region Variables
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tmpText;
    public static int livesAmount = 3;

    public static LivesCount instance;

    [Header("Invulnerability")]
    private static bool isInvulnerable = false;
    private static float invulnerabilityTime = 1.0f;
    private static float blinkInterval = 0.15f;

    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Day/Night Followers")]
    [SerializeField] private SpriteRenderer dayFollower;
    [SerializeField] private SpriteRenderer nightFollower;

    private bool isNightMode = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        UpdateTextUI();
        UpdateFollowerSprites();
    }

    private void OnEnable()
    {
        WorldShiftEvents.OnWorldShift += HandleWorldShift;
    }

    private void OnDisable()
    {
        WorldShiftEvents.OnWorldShift -= HandleWorldShift;
    }
    #endregion

    #region Text & Lives
    public static void UpdateTextUI()
    {
        if (instance != null && instance.tmpText != null)
        {
            instance.tmpText.text = "x" + livesAmount;
        }
        else
        {
            Debug.LogWarning("LivesCount instance or tmpText is missing!");
        }
    }

    public static void LoseLife()
    {
        if (isInvulnerable) return;

        livesAmount--;
        UpdateTextUI();
        Debug.Log("Life lost! Remaining: " + livesAmount);

        if (instance != null)
            instance.StartCoroutine(instance.InvulnerabilityRoutine());

        if (livesAmount <= 0)
        {
            Debug.Log("Game Over!");
            // TODO: Game-over logic
        }
    }

    public static void GainLife()
    {
        livesAmount++;
        UpdateTextUI();
    }
    #endregion

    #region Invulnerability
    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsed = 0f;

        if (playerSprite == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerSprite = playerObj.GetComponent<SpriteRenderer>();
        }

        if (playerSprite != null)
        {
            while (elapsed < invulnerabilityTime)
            {
                playerSprite.enabled = !playerSprite.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }
            playerSprite.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }

    public static void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
        if (instance != null && instance.playerSprite != null && value)
            instance.playerSprite.enabled = true;
    }

    public static bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    #endregion

    #region Day/Night Handling
    private void HandleWorldShift(bool isDay)
    {
        // Convert to night mode for the sprite switch
        SetWorldState(!isDay);
    }

    public void SetWorldState(bool isNight)
    {
        isNightMode = isNight;
        UpdateFollowerSprites();
    }

    private void UpdateFollowerSprites()
    {
        if (dayFollower != null)
            dayFollower.enabled = !isNightMode;
        if (nightFollower != null)
            nightFollower.enabled = isNightMode;
    }
    #endregion
}
