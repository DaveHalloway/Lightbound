using TMPro;
using UnityEngine;

public class LivesCount : MonoBehaviour
{
    #region Variables
    [SerializeField] TextMeshProUGUI tmpText;
    public static int livesAmount = 3;

    static LivesCount instance;
    #endregion

    #region Unity Methods
    void Awake()
    {
        instance = this;
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();
        UpdateTextUI();
    }
    #endregion

    #region Custom Methods
    public static void UpdateTextUI()
    {
        if (instance != null && instance.tmpText != null)
            instance.tmpText.text = "x" + livesAmount;
    }

    public static void LoseLife()
    {
        livesAmount--;
        UpdateTextUI();

        if (livesAmount <= 0)
        {
            // Handle game over logic here
            Debug.Log("Game Over!");
        }
    }

    public static void GainLife()
    {
        livesAmount++;
        UpdateTextUI();
    }


    #endregion
}
