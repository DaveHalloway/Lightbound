using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    #region Variables
    public float patrolSpeed = 2f;
    public float patrolRange = 5f;

    public float chaseSpeed = 4f;
    public float chaseRange = 5f;

    public float attackRange = 1.2f;

    public Transform player;

    EnemyAttack enemyAttack;
    Vector2 startPoint;
    bool movingRight = true;
    bool isAttacking = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        startPoint = transform.position;
        enemyAttack = GetComponent<EnemyAttack>();
    }

    void Update()
    {
        if (player == null) return;
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Within attack range!");
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
        transform.Translate(Vector2.right * moveDirection * patrolSpeed * Time.deltaTime);

        if(movingRight && transform.position.x >= startPoint.x + patrolRange)
        {
            movingRight = false;
            Flip();
        }
        else if(!movingRight && transform.position.x <= startPoint.x - patrolRange)
        {
            movingRight = true;
            Flip();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * chaseSpeed * Time.deltaTime);

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
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(startPoint.x - patrolRange, startPoint.y), new Vector2(startPoint.x + patrolRange, startPoint.y));
    }
    #endregion
}
