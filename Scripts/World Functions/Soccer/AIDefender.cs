using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDefender : AISoccerPlayer
{
    [SerializeField]
    private Vector3[] positions; // 0 = upper, 1 = mid, 2 = lower

    [SerializeField]
    private int idleDir;

    private void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        ChangePositions();
    }

    private void ChangePositions()
    {
        lastPos = transform.position;
        if (ball.position.x < centerField.x) { return; }

        if (ball.position.y - centerField.y < 1 && ball.position.y - centerField.y > -1)
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[1], speed * Time.deltaTime);
        }
        else if (ball.position.y < centerField.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[2], speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[0], speed * Time.deltaTime);
        }
        DetermineChange();

        IdleDirection();

        UpdateAnimation();
    }


    //protected void UpdateAnimation()
    //{
    //    currentState = PlayerState.walk;
    //    animator.currentAction = PlayerAction.Idle;
    //    Debug.Log("Idle");

    //    animator.MakePlayerMove();
    //}

    private void IdleDirection()
    {
        if (change == Vector3.zero)
            if (Mathf.Abs(ball.position.x - transform.position.x) > Mathf.Abs(ball.position.y - transform.position.y))
            { // Difference of x is greater
                animator.Vertical = 0;
                animator.Horizontal = idleDir;
                currentState = PlayerState.walk;
                animator.currentAction = PlayerAction.Idle;
                animator.MakePlayerMove();
            }
            else
            {
                animator.Vertical = VIdleDir();
                animator.Horizontal = 0;
                currentState = PlayerState.walk;
                animator.currentAction = PlayerAction.Idle;
                animator.MakePlayerMove();
            }
    }
    private int VIdleDir()
    {
        if (ball.position.y < transform.position.y)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
