using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldFollowPlayer : Enemy
{
    private float speedY;
    private float speedX;

    public int radius;

    private int radiussquared;

    private Vector3 TemporaryEnemyCords;

    private Vector3 difference;

    public bool pathfinding = false;

    private readonly int layerMask = 1 << 0;

    private void Awake()
    {
        radiussquared = radius * radius;
        speedX = speed;
        speedY = speed;
    }

    private void FixedUpdate()
    {
        difference = player.transform.position - transform.position;
        if (pathfinding)
            return;

  
        if (difference.sqrMagnitude < radiussquared && !pathfinding)
        {
            if (transform.position.x > player.transform.position.x + .05f || transform.position.x < player.transform.position.x - .05f)
            {
                changeSpeedX();
                transform.position += new Vector3(speedX * Time.deltaTime, 0, 0);
            }
            else
            {
                speedX = 0;
            }
            if (transform.position.y > player.transform.position.y + .5f || transform.position.y < player.transform.position.y - .5f)
            {
                changeSpeedY();
                transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
            }
            else
            {
                speedY = 0;
            }
        }
    }

    private void changeSpeedX()
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

    private void changeSpeedY()
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

    private void OnCollisionStay2D(Collision2D other)
    {
        Debug.Log(other);
        if (other.gameObject.CompareTag("Wall"))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position, layerMask);

            if (hit.collider == null)
            {
                pathfinding = false;
                return;
            }
            Debug.Log(hit.collider);
            //if (hit.collider != null)
            //{

            //    if (hit.collider.tag != "Wall")
            //    {
            //        pathfinding = false;
            //        return;
            //    }
            //}
            //else
            //{
            //    pathfinding = false;
            //    return;
            //}
                Vector3 enemyChange = TemporaryEnemyCords - transform.position;
            if (enemyChange.sqrMagnitude < .1f)
            {
                if (Mathf.Abs(difference.y) < Mathf.Abs(difference.x)) // Change this to use raycasts instead
                {
                    pathfinding = true;
                    speedY = speed * -1;
                    transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
                }
            }
            TemporaryEnemyCords = transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        pathfinding = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        player.GetComponent<PlayerHealth>().Health(attackDamage);
    }
}
