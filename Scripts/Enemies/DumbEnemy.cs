using UnityEngine;
using System.Collections;

public class DumbEnemy : Enemy
{
    private int baseSpeed;

    private bool canMoveOn = true;
    private bool moving = false;

    private Vector3 dir;
    private Rigidbody2D rb;

    [SerializeField]
    private EnemyAnimator eAnim;

    [SerializeField]
    private float stallTime;
    [SerializeField]
    private float moveTime;
    private void Awake()
    {
        baseSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            rb.MovePosition(transform.position + speed * Time.deltaTime * dir);
        }
        if (canMoveOn)
        {
            canMoveOn = false;
            StartCoroutine(ControlCo());
        }
    }
    private IEnumerator ControlCo()
    {
        IdleMoving();
        if (stallTime != 0)
        {
            eAnim.currentState = EnemyAnimator.EnemyState.Look;
            yield return new WaitForSeconds(stallTime);
        }
        speed = baseSpeed;

        //if (Random.Range(0, 4) >= 1) // 3 in 4 chance to move
        //{
        //IdleMoving();
        //}
        eAnim.currentState = EnemyAnimator.EnemyState.Move;
        moving = true;
        yield return new WaitForSeconds(moveTime);
        moving = false;
        canMoveOn = true;
    }

    private void IdleMoving()
    {
        dir.x = 0;
        dir.y = 0;

    TryAgain:
        dir.x = Random.Range(-1, 2);
        dir.y = Random.Range(-1, 2);
        if (dir.x == 0 && dir.y == 0) { goto TryAgain; }

        if (dir.x != 0 && dir.y != 0)
        {
            int xOrY = Random.Range(0, 2); // 0 or 1
            if (xOrY == 0) { dir.y = 0; } else { dir.x = 0; } // Only x or only y
        }
        eAnim.x = (int)dir.x;
        eAnim.y = (int)dir.y;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Sword"))
        {
            return;
        }
        else if (dir.x != 0 || dir.y != 0)
        {
            dir.x *= -1;
            dir.y *= -1;
            eAnim.x *= -1;
            eAnim.y *= -1;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().Health(attackDamage);
        }
    }
}
