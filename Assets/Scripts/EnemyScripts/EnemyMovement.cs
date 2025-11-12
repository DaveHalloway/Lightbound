using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    #region Variables
    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float patrolRange = 5f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float chaseRange = 5f;

    [Header("Attack Settings")]
    public float attackRange = 1.2f;
    public Transform player;

    [Header("Animation")]
    public Animator anim;

    private EnemyAttack enemyAttack;
    private Vector2 startPoint;
    private bool movingRight = true;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    #endregion

    #region Unity Methods
    private void Start()
    {
        startPoint = transform.position;
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();

        // Ensure animator is assigned
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null) return;
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackSequence());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Update animation parameters
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("IsAttacking", isAttacking);
        }
    }
    #endregion

    #region Custom Methods
    IEnumerator AttackSequence()
    {
        isAttacking = true;
        enemyAttack.Attack(player);
        yield return new WaitForSeconds(enemyAttack.attackCooldown);
        isAttacking = false;
    }

    void Patrol()
    {
        float moveDirection = movingRight ? 1f : -1f;
        Vector2 velocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y);
        rb.linearVelocity = velocity;

        // Check patrol boundaries
        if (movingRight && transform.position.x >= startPoint.x + patrolRange)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && transform.position.x <= startPoint.x - patrolRange)
        {
            movingRight = true;
            Flip();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !movingRight)
        {
            movingRight = true;
            Flip();
        }
        else if (direction.x < 0 && movingRight)
        {
            movingRight = false;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(transform.position.x - patrolRange, transform.position.y),
                        new Vector2(transform.position.x + patrolRange, transform.position.y));
    }
    #endregion
}
