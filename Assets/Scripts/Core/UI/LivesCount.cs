using TMPro;
using UnityEngine;
using System.Collections;

public class LivesCount : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI tmpText;
    public static int livesAmount = 3;

    public static LivesCount instance;

    private static bool isInvulnerable = false;
    private static float invulnerabilityTime = 1.0f;
    private static float blinkInterval = 0.15f;

    [SerializeField] private SpriteRenderer playerSprite;
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
    }
    #endregion

    #region Custom Methods
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
        // Prevent losing life while invulnerable
        if (isInvulnerable) return;

        livesAmount--;
        UpdateTextUI();
        Debug.Log("Life lost! Remaining: " + livesAmount);

        if (instance != null)
            instance.StartCoroutine(instance.InvulnerabilityRoutine());

        if (livesAmount <= 0)
        {
            Debug.Log("Game Over!");
            // TODO: Add restart or game-over logic
        }
    }

    public static void GainLife()
    {
        livesAmount++;
        UpdateTextUI();
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsed = 0f;

        // If no sprite assigned, try to find one on the player tagged "Player"
        if (playerSprite == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerSprite = playerObj.GetComponent<SpriteRenderer>();
        }

        // Blink effect during invulnerability
        if (playerSprite != null)
        {
            while (elapsed < invulnerabilityTime)
            {
                playerSprite.enabled = !playerSprite.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            playerSprite.enabled = true; // make sure it's visible after blinking
        }
        else
        {
            // If no sprite found, just wait the invulnerability time
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }

    /// <summary>
    /// Enables or disables invulnerability manually.
    /// Used for things like ground pounds or power-ups.
    /// </summary>
    /// <param name="value">True = Invulnerable, False = Vulnerable</param>
    public static void SetInvulnerable(bool value)
    {
        isInvulnerable = value;

        // Ensure player sprite is visible when toggling manually
        if (instance != null && instance.playerSprite != null && value)
            instance.playerSprite.enabled = true;
    }

    /// <summary>
    /// Returns whether the player is currently invulnerable.
    /// </summary>
    public static bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    #endregion
}
