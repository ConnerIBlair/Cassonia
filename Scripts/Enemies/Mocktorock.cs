using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

public class Mocktorock : Enemy
{
    private bool canMoveOn = true;
    private bool moving;
    private Vector3 direction;
    private Rigidbody2D rb;
    private int baseSpeed = 2;

    private int layerMask = 1 << 7 | 1 << 3 | 1 << 0; // These work for squirrel
    //private int PlayerMask = 7 | 3 | 0; // Change numbers

    private void FixedUpdate()
    {
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

    private IEnumerator ControlCo()
    {
        animator.Play("Squirrel_Idle");
        yield return new WaitForSeconds(1);
        speed = baseSpeed;

        IdleMoving();

        yield return new WaitForSeconds(3);
        moving = false;
        canMoveOn = true;
    }

    private void IdleMoving()
    {
        direction = new Vector3Int(Random.Range(-1, 2), Random.Range(-1, 2));

        if (direction.x != 0) direction = new(direction.x, 0);
        if (direction.y != 0) direction = new(0, direction.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4, layerMask);



        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                //shoot 
            }
            else
            {
                IdleMoving();
                return;
            }
        }

        if (direction.x < 0) // Left
        {
            sRenderer.flipX = true;
            animator.Play("Squirrel_Run");
            moving = true;
        }
        else if (direction.x > 0) // Right
        {
            sRenderer.flipX = false;
            animator.Play("Squirrel_Run");
            moving = true;
        }
        else
        {
            speed = 0;
            animator.Play("Squirrel_Idle");
        }
    }

    private void PlayerCheck()
    {

    }
}
