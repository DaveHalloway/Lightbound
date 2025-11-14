using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LivesCount : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tmpText;
    public static int livesAmount = 3;

    public static LivesCount Instance;

    [Header("Invulnerability")]
    private static bool isInvulnerable = false;
    private static float invulnerabilityTime = 1.0f;
    private static float blinkInterval = 0.15f;

    [Header("Player Sprites")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer dayFollower;
    [SerializeField] private SpriteRenderer nightFollower;

    private bool isNightMode = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        UpdateTextUI();
        UpdateFollowerSprites();
    }

    private void OnEnable()
    {
        WorldShiftEvents.OnWorldShift += HandleWorldShift;
        CheckpointManager.OnRespawned += HandleRespawn;
    }

    private void OnDisable()
    {
        WorldShiftEvents.OnWorldShift -= HandleWorldShift;
        CheckpointManager.OnRespawned -= HandleRespawn;
    }

    private void HandleRespawn()
    {
        UpdateFollowerSprites();
        UpdateTextUI();
    }

    public static void UpdateTextUI()
    {
        if (Instance != null && Instance.tmpText != null)
            Instance.tmpText.text = "x" + livesAmount;
    }

    public static void LoseLife()
    {
        if (isInvulnerable) return;

        livesAmount--;
        UpdateTextUI();
        Debug.Log("Life lost! Remaining: " + livesAmount);

        if (Instance != null)
            Instance.StartCoroutine(Instance.InvulnerabilityRoutine());

        if (livesAmount <= 0)
            ResetLivesAndReloadScene();
    }

    private static void ResetLivesAndReloadScene()
    {
        livesAmount = 3;

        WorldShiftEvents.Invoke(true); // Reset to day mode

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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

    private void HandleWorldShift(bool isDay)
    {
        isNightMode = !isDay;
        UpdateFollowerSprites();
    }

    private void UpdateFollowerSprites()
    {
        if (dayFollower != null)
            dayFollower.enabled = !isNightMode;
        if (nightFollower != null)
            nightFollower.enabled = isNightMode;
    }

    public static void GainLife()
    {
        livesAmount++;
        UpdateTextUI();
    }
}
