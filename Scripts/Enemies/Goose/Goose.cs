using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Thrown,
    Patrol,
    Guard,
    Attack
}


public class Goose : Enemy
{
    private int speedY;
    private int speedX;

    public int radius;

    private int radiussquared;

    private Vector3 difference;

    public EnemyState currentState;

    public bool pathfinding = false;

    private float currentTime;
    private RaycastHit2D hit;
    private LayerMask layermask = 1 << 7;

    //private void Update()
    //{
    //    if (speedY != 0 && Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
    //    {
    //        // Y animate
    //    }
    //    else if (speedX != 0)
    //    {
    //        // X animate
    //    }
    //}

    private void FixedUpdate()
    {
        if (!paused)
        {

            if (currentState == EnemyState.Patrol) // Where is my precious...
            {
                // Patrol
                if (currentTime < .25f)
                {
                    currentTime += Time.deltaTime;
                }
                else
                {
                    PlayerVisibility();
                    currentTime = 0;
                }
            }

            if (currentState == EnemyState.Thrown) // FLY YOU FOOLS
            {
                transform.position = Vector3.MoveTowards(transform.position, pTransform.position, speed * Time.deltaTime);
                // Change Anim to flying/thrown
                return;
            }
            // Make anim not flying/thrown
            difference = player.transform.position - transform.position;
            if (difference.sqrMagnitude < radiussquared && currentState == EnemyState.Attack) // For Frodo!!!
            {
                if (transform.position.x > player.transform.position.x + .05f || transform.position.x < player.transform.position.x - .05f)
                {
                    ChangeSpeedX();
                    transform.position += new Vector3(speedX * Time.deltaTime, 0, 0);
                }
                else
                {
                    speedX = 0;
                }
                if (transform.position.y > player.transform.position.y + .05f || transform.position.y < player.transform.position.y - .05f)
                {
                    ChangeSpeedY();
                    transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
                }
                else
                {
                    speedY = 0;
                }
            }
            //else
            //{
            //    speedY = 0;
            //    speedX = 0;
            //}
        }
        else
        {
            speedX = 0;
            speedY = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttackCo());
        }
    }

    private void PlayerVisibility()
    {
        hit = Physics2D.Raycast(transform.position, -transform.right, 6f, layermask);

        if (hit.collider != null)
        {
            Debug.Log("hit something");
            if (hit.collider.CompareTag("Player"))
            {
                if (GetComponentInParent<BossGoose>() != null)
                {
                    GetComponentInParent<BossGoose>().DecideAction();
                    return;
                }
                currentState = EnemyState.Attack;
                Debug.Log("player in sight");
            }
        }
    }

    public IEnumerator ThrowCo()
    {
        paused = true;
        currentState = EnemyState.Thrown;
        yield return new WaitForSeconds(3);
        currentState = EnemyState.Attack;
        paused = false;
    }

    private IEnumerator AttackCo()
    {
        player.GetComponent<PlayerHealth>().Health(attackDamage);
        paused = true;
        yield return new WaitForSeconds(.5f);
        paused = false;
    }

    private void ChangeSpeedX()
    {
        if (transform.position.x < player.transform.position.x)
        {
            speedX = speed;
        }
        else
        {
            speedX = speed * -1;
        }
    }

    private void ChangeSpeedY()
    {
        if (transform.position.y < player.transform.position.y)
        {
            speedY = speed;
        }
        else
        {
            speedY = speed * -1;
        }
    }

}
