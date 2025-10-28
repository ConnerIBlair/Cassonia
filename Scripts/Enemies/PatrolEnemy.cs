using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{

    public Transform[] points;
    public int currentPoint;
    private bool changePoint;
    public bool patrolling;
    public bool circlePatrol;
    private LayerMask layermask = 1 << 7;

    private int speedY;
    private int speedX;

    public int radius;

    private int radiussquared;

    private Vector3 difference;

    private RaycastHit2D hit;

    private void Awake()
    {
        radiussquared = radius * radius;
        speedX = speed;
        speedY = speed;
    }

    private void FixedUpdate()
    {
        if (paused != true)
        {
            difference = player.transform.position - transform.position;
            if (!patrolling)
            {

                if (difference.sqrMagnitude < radiussquared)
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
                else if (!patrolling && hit.collider != null)
                {
                    patrolling = true;
                }
            }
            else
            {
                if (difference.sqrMagnitude < radiussquared - 11)
                {
                    StartCoroutine(NoPatrolCo());
                }
                Patrol();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().Health(attackDamage);
        }
    }

    private void Patrol()
    {
        Vector3 oldPosition = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime * 0.5f);

        hit = Physics2D.Raycast(transform.position, transform.position - oldPosition, 6f, layermask);
        if (hit.collider != null)
        {
            Debug.Log("hit something");
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("player in sight");
                StartCoroutine(NoPatrolCo());
            }
        }

        var pointsDifference = transform.position - points[currentPoint].position;
        if (Mathf.Abs(pointsDifference.sqrMagnitude) < .1)
        {
            if (changePoint == true)
            {
                changePoint = false;

                if (currentPoint + 1 == points.Length)
                {
                    if (circlePatrol)
                    {
                        currentPoint = 0;
                    }
                    else
                    {
                        currentPoint--;
                    }
                }
                else
                {
                    currentPoint++;
                }
            }
        }
        else
        {
            changePoint = true;
        }
    }
    private IEnumerator NoPatrolCo()
    {
        if (speed != 0)
        {
            int speedHolder;
            speedHolder = speed;
            speed = 0;
            yield return new WaitForSeconds(.5f);
            //hit = null;
            speed = speedHolder;
            patrolling = false;
        }
    }
}
