using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour, IInteractable
{
    // When holding a bush, the player is able to open chests and that breaks things !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    // Have Trevor make small fire anim for bush, probably overlay. Do NOT allow player to pick up. allow player to destroy.
    // Are enemies fearless or do they run away? Does it depend on the enemy possibly? 
    // Enemies should get ignited by bush. make player hurt when in contact with bush, not on fire
    // If bush being carried then set on fire (or hit) by enemy, bush destroyed, dropped by player and player gets hurt.
    // when player goes through door, goes into water, or gets hurt (not healed)(Possibly Knocked back as solution) then destroy bush

    private bool canBreak = true;

    [SerializeField]
    private AudioClip breakSound;
    [SerializeField]
    private AudioClip throwSound;

    [SerializeField]
    private float speed = 6;

    private float x = 1;
    private float y;

    private int dir = 1;

    private Vector3 startPos;
    [HideInInspector]
    public PlayerMovement player;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Sprite stump;

    [SerializeField]
    private GameObject flame;
    private GameObject flameObj;

    public bool pickedUp = false;

    private PlayerDirection playerDirection;

    public bool dead = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pickedUp) return; 
        if (speed == 0) return;
        speed = 0;
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.name == "Spearhead")
            {
                collision.transform.parent.transform.parent.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                collision.transform.parent.transform.parent.GetComponent<Enemy>().Health(1);
                player.GetComponentInChildren<Knockback>().KnockbackObject(collision.gameObject.transform.parent.transform.parent);
            }
            else
            {
                collision.transform.parent.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                collision.GetComponent<EnemyHurtbox>().Health(1);
                player.GetComponentInChildren<Knockback>().KnockbackObject(collision.gameObject.transform.parent);
            }
            StartCoroutine(DestroyCo());
        }
    }

    private void Stump()
    {
        GameObject obj = new GameObject("Stump");
        obj.AddComponent<SpriteRenderer>().sprite = stump;
        obj.transform.position = transform.position;
        stump = null;
    }
    
    private void Update()
    {
        if (dead) { return; }
        if (pickedUp)
        {
            OrderLayer();
        }
    }
    private void OrderLayer(int specific = 1)
    {
        playerDirection = player.CurrentDir;
        switch (playerDirection)
        {
            case PlayerDirection.Down:
                GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
                break;

            case PlayerDirection.Up:
                GetComponentInChildren<SpriteRenderer>().sortingOrder = specific;
                break;

            case PlayerDirection.Left:
                GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                break;

            case PlayerDirection.Right:
                GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                break;
        }
    }
    public void Interact(PlayerMovement player)
    {
        if (dead) { return; }   
        if (!pickedUp) // Not picked up yet
        {
            Stump();
            this.player = player;
            player.currentState = PlayerState.interact;
            StartCoroutine(PickUp());
        }
        else
        {
            Throw(player.pAnimator.currentDirection);
            player.sounds.PlayEffect(throwSound, 1);
        }
    }

    private IEnumerator PickUp()
    {
        OrderLayer(0);
        transform.parent = player.transform; // Sets player as this object's parent
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0); // Moved to after wait

        // Use positions array to figure out where it should be.
        // When facing down (NOT UP) and picking up, make sure sprite render layer is above the player's

        player.Interactable = this;

        gameObject.layer = LayerMask.NameToLayer("Player"); // Doesn't collide with player, does with enemies
        animator.Play("Bush_Pickup_" + player.CurrentDir);
        yield return new WaitForSeconds(.32f);
        animator.Play("Idle_Bush");


        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0); // Make sure in the correct spot
        player.currentState = PlayerState.walk;
        player.pAnimator.currentAction = PlayerAction.Carry;
        player.pAnimator.MakePlayerMove();

        pickedUp = true;
    }

    private void Throw(PlayerDirection pDirection)
    {
        player.Interactable = null;

        player.pAnimator.currentAction = PlayerAction.Throw;
        player.pAnimator.MakePlayerMove();

        transform.parent = null;
        startPos = player.transform.position;
        switch (pDirection)
        {
            case PlayerDirection.Down:
                GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
                x = 0;
                startPos = new(startPos.x, startPos.y + 0.5f);
                speed *= -2.5f;
                break;

            case PlayerDirection.Up:
                x = 0;
                startPos = new(startPos.x, startPos.y + 1);
                speed *= 2;
                break;

            case PlayerDirection.Left:
                dir = -1;
                break;
        }
        StartCoroutine(WaitCo());
        StartCoroutine(Movement());
    }
    private IEnumerator WaitCo()
    {   
        player.paused = true;
        yield return new WaitForSeconds(.25f);
        player.pAnimator.currentAction = PlayerAction.Idle;
        player.pAnimator.MakePlayerMove();
        yield return new WaitForSeconds(.25f);
        player.paused = false;
    }

    private IEnumerator Movement()
    {
        if (x == 0)
        {
            for (int i = 0; i < 25; i++)
            {
                y = i * .01f * speed;
                yield return new WaitForFixedUpdate();
                Vector3 newPos = new(0, y);
                rb.MovePosition(startPos + newPos);
            }
        }
        else
        {
            for (x = 0; x < 4; x += speed * Time.deltaTime)
            {
                y = -.1f * Mathf.Pow(x, 2) + 1; // Originally -.0625f
                yield return new WaitForFixedUpdate();
                Vector3 newPos = new(x * dir, y);
                rb.MovePosition(startPos + newPos);
            }
        }
        StartCoroutine(DestroyCo());
    }

    public IEnumerator FireCo()
    {
        if (!flameObj)
        {
            flameObj = Instantiate(flame);
            flameObj.transform.position = new Vector2(transform.position.x, transform.position.y + .25f);
            flameObj.GetComponent<Animator>().Play("Small_Flame");
            yield return new WaitForSeconds(2);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
            foreach (Collider2D collider in colliders)
            {
                if (collider.GetComponent<Bush>() == true)
                {
                    collider.GetComponent<Bush>().StartCoroutine("FireCo");
                    collider.GetComponent<Bush>().player = player;
                }
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(DestroyCo());
        }
    }

    private IEnumerator DestroyCo()
    {
        if (stump)
        {
            Stump();
        }
        if (canBreak)
        {
            canBreak = false;
            if (dead) { animator.Play("Bush_Break"); } // DEAD BUSH BREAK
            else { animator.Play("Bush_Break"); }

            player.sounds.PlayEffect(breakSound, .4f);
        }
        yield return new WaitForSeconds(.33f);
        player.paused = false;
        Destroy(flameObj);
        Destroy(gameObject);
    }
}
