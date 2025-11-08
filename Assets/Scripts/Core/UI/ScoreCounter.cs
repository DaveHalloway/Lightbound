using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    #region Variables
    [SerializeField] TextMeshProUGUI tmpText;
    public static int coinAmount;

    static ScoreCounter instance;
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
            instance.tmpText.text = "Coins: " + coinAmount;
    }

    public static void AddCoin(int amount = 1)
    {
        coinAmount += amount;
        if(coinAmount % 10 == 0)
        {
            LivesCount.GainLife();
            coinAmount = 0;
        }
        UpdateTextUI();
    }
    #endregion
}
