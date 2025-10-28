using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoccerPlayer : MonoBehaviour
{
    // The player will have a team of two AIs (Goalie, Defender)
    // Opposing team has three AIs (Defender, Midfield, Forward)

    // References to each AI position. Take from enemy AI to not run into eachother (Once implemented)
    // Or, all AIs act separately from eachother. Defender does Defender things etc.
    // Could still have small logic checks to not kick ball into own goal

    protected enum Team { Home, Guest }

    public PlayerState currentState = PlayerState.walk;
    protected Rigidbody2D rb;
    protected Vector3 change;
    protected Vector3 lastPos;
    private Vector2 lastDir;

    [SerializeField]
    protected Team team;

    [SerializeField]
    protected int speed;
    public bool paused;
    public Transform ball;

    [SerializeField]
    protected Vector2 centerField;
    [SerializeField]
    protected Transform targetGoal;

    [HideInInspector]
    public PlayerAnimator animator;

    public Transform[] teammates;

    public Transform[] opponents;

    protected void Start()
    {
        animator = GetComponent<PlayerAnimator>();
        rb = GetComponent<Rigidbody2D>();
        animator.Vertical = 0;
        animator.Horizontal = -1;
    }

    protected void Update()
    { 
        if (paused)
        {
            return;
        }
    }

    protected void DetermineChange()
    {
        if (Mathf.Abs(lastPos.x - transform.position.x) > Mathf.Abs(lastPos.y - transform.position.y))
        {
            if (lastPos.x < transform.position.x)
            {
                change = new(1, 0);
            }
            else
            {
                change = new(-1, 0);
            }
        }
        else
        {
            if (lastPos.y < transform.position.y)
            {
                change = new(0, 1);
            }
            else if (lastPos.y > transform.position.y)
            {
                change = new(0, -1);
            }
            else
            {
                change = Vector2.zero;
            }
        }
    }

    protected void UpdateAnimation()
    {
        if (change != Vector3.zero)
        {
            animator.currentAction = PlayerAction.Run;

            // Normally where to move player
            animator.Horizontal = change.x;
            animator.Vertical = change.y;
            lastDir = new Vector2(change.x, change.y);
        }
        else
        {
            currentState = PlayerState.walk;
            animator.currentAction = PlayerAction.Idle;
        }
        animator.MakePlayerMove();
    }
}
