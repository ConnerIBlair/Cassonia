using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour, IInteractable
{
    // In ALTTP thrown objects go right/left 4 tiles, down to player's feet
    // Also hands/arms don't reach object, no where close
    // Object position changes when being picked up multiple times
    // One less time moving than animation frames for picking up. 5 picking up frames, one is just flying sweat
    
    private bool canThrow;
    private bool thrown;

    private bool increaseY = false;
    private float xSpeed;

    private float dirX;
    private float dirY;

    public float speed;

    public Animator animator;
    private Rigidbody2D rb;

    private IEnumerator lastCoroutine;
    private PlayerMovement player;
    private PlayerAnimator pAnim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && thrown == true)
        {
            other.GetComponent<Enemy>().Health(1);
            StopCoroutine(lastCoroutine);
            //player.
            StartCoroutine(BreakCo());
        }
        if (other.CompareTag("Player") && other.GetComponent<PlayerMovement>().Interactable == null)
        {
            other.GetComponent<PlayerMovement>().Interactable = this;
            this.player = other.GetComponent<PlayerMovement>();
            pAnim = other.GetComponent<PlayerAnimator>();
        }
        if (other.CompareTag("Sword") || other.CompareTag("Projectile"))
        {
            StartCoroutine(BreakCo());
        }
    }

    public void Interact(PlayerMovement player)
    {
        this.player = player;
        if (!canThrow)
        {
            pAnim.currentAction = PlayerAction.Lift;
            StartCoroutine(LiftAnimCo());
            gameObject.transform.parent = player.transform;
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0);
            //canThrow = true;
            //player.currentState = PlayerState.walk;
        }
        else
        {
            StartCoroutine(ThrowCo());
        }
    }

    private IEnumerator LiftAnimCo()
    {
        pAnim.MakePlayerMove();
        yield return new WaitForSeconds(.5f);
        canThrow = true;
        player.currentState = PlayerState.walk;
        pAnim.currentAction = PlayerAction.Carry;
        pAnim.MakePlayerMove();
    }
    private IEnumerator ThrowAnimCo()
    {
        pAnim.MakePlayerMove();
        yield return new WaitForSeconds(.33f);
        pAnim.currentAction = PlayerAction.Walk;
        pAnim.MakePlayerMove();
    }

    public IEnumerator ThrowCo()
    {
        canThrow = false;
        player.currentState = PlayerState.walk;
        pAnim.currentAction = PlayerAction.Throw;
        StartCoroutine(ThrowAnimCo());
        lastCoroutine = ThrowCo();
        thrown = true;
        player.Interactable = null;
        transform.parent = null;
        dirX = pAnim.Horizontal;
        dirY = pAnim.Vertical;
        yield return null;

        if (dirY == 0)
        {
            dirY = -.1f;
            increaseY = true;
            xSpeed = dirX * speed + 2;
        }
        else
        {
            xSpeed = 0;
            rb.linearVelocity = new Vector2(0, dirY * speed);
        }

        yield return new WaitForSeconds(0.5f);
        player.currentState = PlayerState.walk;
        pAnim.currentAction = PlayerAction.Idle;
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(BreakCo());
    }

    public IEnumerator BreakCo()
    {
        if (player != null) player.Interactable = null;

        animator.Play("Break_Bush");
        thrown = false;
        dirY = 0;
        rb.linearVelocity = Vector2.zero;
        increaseY = false;
        yield return new WaitForSeconds(.465f);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (increaseY == true)
        {
            dirY *= 1.075f;
        }
        if (thrown == true)
        {
            rb.linearVelocity = new Vector2(xSpeed, dirY * speed);
        }
    }
}
