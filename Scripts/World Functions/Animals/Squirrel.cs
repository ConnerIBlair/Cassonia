using System.Collections;
using UnityEngine;

public class Squirrel : Animal
{
    private int layerMask = 1 << 7 | 1 << 3 | 1 << 0;

    private void FixedUpdate()
    {
        if (GOGOGO == true)
        {
            animator.Play("Squirrel_Run"); 
            StartCoroutine(DestroyCo()); 
            GOGOGO = false;
        }

        if (moving)
        {
            rb.MovePosition(transform.position + speed * Time.deltaTime * direction.normalized);
        }
        if (canMoveOn)
        {
            canMoveOn = false;
            StartCoroutine(ControlCo());
        }
    }

    private IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private IEnumerator ControlCo()
    {
        animator.Play("Squirrel_Idle");
        yield return new WaitForSeconds(1);
        speed = baseSpeed;

        if (Random.Range(0, 3) <= 1)
        {
            IdleMoving();
        }
        else
        {
            StartCoroutine(EatCo());
        }

        yield return new WaitForSeconds(3);
        moving = false;
        canMoveOn = true;
    }

    private void IdleMoving()
    {
        direction = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4, layerMask);

        if (hit.collider != null)
        {
            IdleMoving();
            return;
        }

        if (direction.x < 0) // Left
        {
            spriteR.flipX = true;
            animator.Play("Squirrel_Run");
            moving = true;
        } else if (direction.x > 0) // Right
        {
            spriteR.flipX = false;
            animator.Play("Squirrel_Run");
            moving = true;
        }
        else
        {
            speed = 0;
            animator.Play("Squirrel_Idle");
        }
    }

    private IEnumerator EatCo()
    {
        canMoveOn = false;
        speed = 0;
        animator.Play("Squirrel_Idle");
        yield return new WaitForSeconds(.5f);
        animator.Play("Squirrel_Eat");
        yield return new WaitForSeconds(Random.Range(1, 3));
    }
}
