using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection
{
    Left,
    Right,
    Down,
    Up
}

public enum PlayerAction
{
    Idle,
    Walk,
    Run,
    Roll,
    Jump,

    Action, // Sword
    Use, // Whip

    Lift,
    Carry,
    CarryIdle,
    Throw,

    LiftDog,
    CarryDog,
    CarryIdleDog,

    Kick,
    Wave
}

public class PlayerAnimator : MonoBehaviour
{
    private enum PlayerType { Player, SoccerAI, }

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private PlayerType objectType;
    public PlayerDirection currentDirection;
    public PlayerAction currentAction;

    public float Horizontal;
    public float Vertical;

    public GameObject hay;

    private void Start()
    {
        Vertical = -1;
    }

    public void PlayAnimation(PlayerAction action, PlayerDirection dir)
    {
        currentDirection = dir;
        if (dir == PlayerDirection.Left)
        {
            spriteRenderer.flipX = true;
            dir = PlayerDirection.Right;
        }
        animator.Play($"{objectType}_{action}_{dir}");
        Horizontal = 0;
        Vertical = 0;
    }

    public void MakePlayerMove()
    {
        if (Vertical != 0)
        {
            VerticalMovement();
        } else if (Horizontal != 0)
        {
            HorizontalMovement();
        }
    }

    private void HorizontalMovement()
    {
        spriteRenderer.flipX = false;
        animator.Play($"{objectType}_{currentAction}_Right");

        if (Horizontal < 0)
        {
            spriteRenderer.flipX = true;
            currentDirection = PlayerDirection.Left;
        }
        else
        {
            spriteRenderer.flipX = false;
            currentDirection = PlayerDirection.Right;
        }
    }
    private void VerticalMovement()
    {
        spriteRenderer.flipX = false;
        if (Vertical < 0)
        {
            animator.Play($"{objectType}_{currentAction}_Down");
            currentDirection = PlayerDirection.Down;
        }
        else
        {
            animator.Play($"{objectType}_{currentAction}_Up");
            currentDirection = PlayerDirection.Up;
        }
    }
}
