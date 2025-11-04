using UnityEngine;

public class CoinCollect : MonoBehaviour
{

    #region Unity Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreCounter.AddCoin();
            Destroy(gameObject);
        }
    }
    #endregion
}
