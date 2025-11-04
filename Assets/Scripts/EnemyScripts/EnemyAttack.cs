using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    #region Variables
    public float attackCooldown = 1.5f;
    public int damage = 10;
    public float knockbackForce = 5f;

    float lastAttackTime = 0f;
    #endregion

    #region Custom Methods
    public void Attack(Transform player)
    {
        Debug.Log("Attack() called!");
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Enemy attacks and removes a life!");

            LivesCount.LoseLife();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
    #endregion
}
