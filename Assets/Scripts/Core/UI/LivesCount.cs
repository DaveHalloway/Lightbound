using TMPro;
using UnityEngine;
using System.Collections;

public class LivesCount : MonoBehaviour
{
    #region Variables
    [SerializeField] TextMeshProUGUI tmpText;
    public static int livesAmount = 3;

    public static LivesCount instance;

    static bool isInvulnerable = false;
    static float invulnerabilityTime = 1.0f;
    static float blinkInterval = 0.15f;

    [SerializeField] SpriteRenderer playerSprite;
    #endregion

    #region Unity Methods
    void Awake()
    {
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

    IEnumerator InvulnerabilityRoutine()
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

        // Blink effect
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
            // if no sprite found, just wait the invulnerability time
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }
    #endregion
}
