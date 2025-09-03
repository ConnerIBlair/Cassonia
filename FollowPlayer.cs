using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyName
{
    Temp,
    Squirrel
}
public class FollowPlayer : Enemy
{
    protected float speedY;
    protected float speedX;

    public int radius;

    private int radiussquared;

    protected Vector3 difference;

    public bool pathfinding = false;

    public EnemyName enemyName;

    private void Awake()
    {
        radiussquared = radius * radius;
        Application.targetFrameRate = 50;
    }
    private void Update()
    {
        if (paused && speed == 0)
        {
            if (enemyName == EnemyName.Squirrel)
            {
                animator.Play("Squirrel_Idle");
                return;
            }
        }
        if (onFire)
        {
            animator.Play("Temp_Enemy_Fire");
            return;
        }
        if (isShocked)
        {
            animator.Play("Temp_Enemy_Shocked");
            return;
        }
        if (enemyName == EnemyName.Temp)
        {
            animator.Play("Temp_Enemy_Normal");
            return;
        }

        if (speedY != 0 && Mathf.Abs(difference.x) < Mathf.Abs(difference.y))
        {
            VerticalMovement();
        }
        else if (speedX != 0)
        {
            HorizontalMovement();
        }
        else
        {
            animator.Play("Squirrel_Idle");
        }
    }

    private void FixedUpdate()
    {
        if (!paused)
        {
            difference = player.transform.position - transform.position;
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
                if (transform.position.y > player.transform.position.y + .05f || transform.position.y < player.transform.position.y - .05f)
                {
                    changeSpeedY();
                    transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
                }
                else
                {
                    speedY = 0;
                }
            }
            else
            {
                speedY = 0;
                speedX = 0;
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
            StartCoroutine(AttackCo());
        }
    }

    private IEnumerator AttackCo()
    {
        player.GetComponent<PlayerHealth>().Health(attackDamage);
        paused = true;
        yield return new WaitForSeconds(.3f);
        paused = false;
    }

    private void HorizontalMovement()
    {
        animator.Play("Squirrel_Run_Right");

        if (speedX < 0)
        {
            sRenderer.flipX = true;
        }
        else
        {
            sRenderer.flipX = false;
        }
    }
    
    private void VerticalMovement()
    {
        sRenderer.flipX = false;
        if (speedY < 0)
        {
            animator.Play("Squirrel_Run_Down");
        }
        else
        {
            animator.Play("Squirrel_Run_Up");
        }
    }
}

    