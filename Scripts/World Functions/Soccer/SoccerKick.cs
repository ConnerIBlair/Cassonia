using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerKick : MonoBehaviour
{

    private bool isAI = true;

    private AISoccerPlayer AI;
    private SoccerPlayer player;
    private PlayerAnimator animator;

    private Transform ball;

    public Transform knockBDir;
    public float kickReach;
    public float timeKicking;

    private float differenceX;
    private float differenceY;
    private float radians;
    private float angle;

    public float force;

    private void Start()
    {
        if (TryGetComponent(out SoccerPlayer player))
        {
            isAI = false;
            this.player = player;
            ball = player.ball;
            animator = player.GetComponent<PlayerAnimator>();
        } else if (TryGetComponent(out AISoccerPlayer AI))
        {
            isAI = true;
            this.AI = AI;
            ball = AI.ball;
            animator = AI.GetComponent<PlayerAnimator>();
        }
    }

    public IEnumerator KickCo()
    {
        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), kickReach);

        if (!CanKick()) // Not facing the ball, kick anyway, don't move ball
        {
            yield return new WaitForSeconds(timeKicking);
            if (isAI) { AI.currentState = PlayerState.walk; } else
            {
                player.currentState = PlayerState.walk;
            }

            yield break;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].transform == ball)
            {
                getAngle(colliders[i].transform.position);
                knockBDir.eulerAngles = new Vector3(0, 0, angle);
                colliders[i].GetComponent<Rigidbody2D>().AddForce(-knockBDir.right * force * 300);

                break;
            }
        }

        yield return new WaitForSeconds(timeKicking);
        if (isAI) { AI.currentState = PlayerState.walk; }
        else
        {
            player.currentState = PlayerState.walk;
        }
    }

    private bool CanKick() // Player must be facing the ball
    {
        switch (animator.currentDirection)
        {
            case PlayerDirection.Right:
                if (ball.position.x < transform.position.x)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case PlayerDirection.Left:
                if (ball.position.x > transform.position.x)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case PlayerDirection.Up:
                if (ball.position.y < transform.position.y)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case PlayerDirection.Down:
                if (ball.position.y > transform.position.y)
                {
                    return false;
                }
                else
                {
                    return true;
                }
        }
        return false;
    }

    private void getAngle(Vector3 obj2)
    {
        differenceX = knockBDir.position.x - obj2.x;
        differenceY = knockBDir.position.y - obj2.y;
        radians = Mathf.Atan2(differenceY, differenceX);
        angle = radians * 57.296f;
    }

}
#region Code in SoccerPlayer
/*
private IEnumerator KickCo()
{
    Collider2D[] colliders;
    colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), kickReach);

    if (!CanKick()) // Not facing the ball
    {
        yield return new WaitForSeconds(timeKicking);
        currentState = PlayerState.walk;
        yield break;
    }

    for (int i = 0; i < colliders.Length; i++)
    {
        Debug.Log(colliders[i].gameObject.name);
        if (colliders[i].transform == ball)
        {
            getAngle(colliders[i].transform.position);
            knockBDir.eulerAngles = new Vector3(0, 0, angle);
            colliders[i].GetComponent<Rigidbody2D>().AddForce(-knockBDir.right * force * 300);

            break;
        }
    }

    yield return new WaitForSeconds(timeKicking);
    currentState = PlayerState.walk;
}

private bool CanKick()
{
    switch (pAnimator.currentDirection)
    {
        case PlayerDirection.Right:
            if (ball.position.x < transform.position.x)
            {
                return false;
            }
            else
            {
                return true;
            }
        case PlayerDirection.Left:
            if (ball.position.x > transform.position.x)
            {
                return false;
            }
            else
            {
                return true;
            }
        case PlayerDirection.Up:
            if (ball.position.y < transform.position.y)
            {
                return false;
            }
            else
            {
                return true;
            }
        case PlayerDirection.Down:
            if (ball.position.y > transform.position.y)
            {
                return false;
            }
            else
            {
                return true;
            }
    }
    return false;
}

private void getAngle(Vector3 obj2)
{
    differenceX = knockBDir.position.x - obj2.x;
    differenceY = knockBDir.position.y - obj2.y;
    radians = Mathf.Atan2(differenceY, differenceX);
    angle = radians * 57.296f;
}
*/
#endregion