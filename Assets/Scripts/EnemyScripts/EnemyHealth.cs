using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Optional Visuals")]
    public Animator animator;
    public GameObject deathEffect;
    public float deathDelay = 0.5f;

    void Start()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! HP: {currentHealth}");

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} has died.");
        if (animator != null)
            animator.SetTrigger("Die");
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject, deathDelay);
    }
}
