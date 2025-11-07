using UnityEngine;

public class Snowball : MonoBehaviour
{
    private Rigidbody2D rb;
    public int damage = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f; // Snowball flies straight
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Launch(Vector2 direction, float speed)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Snowball collided with '{collision.gameObject.name}' (tag='{collision.gameObject.tag}')");

        // Try to get EnemyHealth from the collider GameObject first,
        // then try parents (handles colliders on child objects).
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>() ?? collision.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Optional: destroy on any collision (walls, environment).
        Destroy(gameObject);
    }
}
