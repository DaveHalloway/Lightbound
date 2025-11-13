using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
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
    private bool isStunned = false;

    private Rigidbody2D rb;

    private void Start()
    {
        startPoint = transform.position;
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null || isStunned) return;
        if (isAttacking) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
            StartCoroutine(AttackSequence());
        else if (dist <= chaseRange)
            ChasePlayer();
        else
            Patrol();

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("IsAttacking", isAttacking);
            anim.SetBool("IsStunned", isStunned);
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        if (enemyAttack != null)
            enemyAttack.Attack(player);
        yield return new WaitForSeconds(enemyAttack.attackCooldown);
        isAttacking = false;
    }

    void Patrol()
    {
        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

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
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * chaseSpeed, rb.linearVelocity.y);

        if (dir.x > 0 && !movingRight)
        {
            movingRight = true;
            Flip();
        }
        else if (dir.x < 0 && movingRight)
        {
            movingRight = false;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? 1 : -1);
        transform.localScale = scale;
    }

    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        if (isStunned) return;
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        if (anim != null)
            anim.SetBool("IsStunned", true);
        yield return new WaitForSeconds(duration);
        if (anim != null)
            anim.SetBool("IsStunned", false);
        isStunned = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(transform.position.x - patrolRange, transform.position.y),
                        new Vector2(transform.position.x + patrolRange, transform.position.y));
    }
}
