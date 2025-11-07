using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TextChange : MonoBehaviour
{
    #region Variables
    [SerializeField] TextMeshProUGUI tmpText;
    [SerializeField] string newText;
    [SerializeField] string originalText = "";
    [SerializeField] float fadeDuration = 0.5f;

    [SerializeField] string playerTag = "Player";

    bool triggered = false;
    Coroutine fadeCoroutine;
    #endregion

    #region 

    private void Awake()
    {
        // Store the starting text if not set manually
        if (tmpText != null && string.IsNullOrEmpty(originalText))
            originalText = tmpText.text;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag(playerTag))
        {
            triggered = true;
            StartFade(newText);
        }
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag(playerTag))
    //    {
    //        StartFade(newText);
    //        triggered = false;
    //    }
    //}

    #endregion

    #region Custom Methods
    private void StartFade(string targetText)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeText(targetText));
    }

    private IEnumerator FadeText(string newText)
    {
        float t = 0f;

        // --- Step 1: Fade Out ---
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        // Change the text after fade out
        SetText(newText);

        // --- Step 2: Fade In ---
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);
    }

    // Helper to set alpha
    private void SetAlpha(float alpha)
    {
        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
    }

    // Helper to set text
    private void SetText(string text)
    {
        if (tmpText != null) tmpText.text = text;
    }
    #endregion
}
