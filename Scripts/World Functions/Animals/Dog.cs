using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Particle System, Stop Action was destroy, killed dog once done after giving dog a treat

// Could implement way of checking if dog is facing away from player, make the raycast
// distance less, as if hearing the player instead of seeing // Once player Walking/Running is added, adjust speeds to match 
public class Dog : MonoBehaviour, IInteractable {

    public enum DogState
    {
        Resting,
        Wagging,
        PickedUp,
        Walking,
        Running,
        Fetching
    }
    public enum DogDir
    {
        Right,
        Left,
        Up,
        Down
    }

    private float fixY = 0;
    private float bobNum;
    private bool bobbing;

    [SerializeField] private float speed;

    [SerializeField] private DogState currentState = DogState.Resting;
    [SerializeField] private DogDir currentDir;

    private VolumeScript sounds;
    [SerializeField] private AudioClip bark;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteR;
    [SerializeField] private Transform originalParent;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D cCollider;

    public Vector2[] jumpingPositions;
    public Vector2[] jPosHolder = { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    private Vector3 direction;

    private int LayerMask => 1 << 7 | 1 << 3 | 1 << 0;

    private PlayerMovement player;
    private PlayerAnimator pAnim;
    public Sprite[] playerSprites;
    [HideInInspector]
    public Transform targetT;
    public float speedModifier = 2;

    private bool moving = false;
    private bool noCoStarted = true;

    private int timer;
    private bool paused;



    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        pAnim = player.GetComponent<PlayerAnimator>();
        targetT = player.transform;
        sounds = FindFirstObjectByType<VolumeScript>();
    }

    private void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        if (currentState == DogState.PickedUp)
        {
            if (pAnim.currentAction == PlayerAction.CarryDog && bobbing == false)
            {
                StartCoroutine(BobbingCo());
            }
            fixY = 0;
            spriteR.flipX = false;
            spriteR.sortingOrder = 3;
            if (player.moving == true)
                fixY = .0625f;

            switch (player.CurrentDir)
            {    
                case PlayerDirection.Right:
                    spriteR.flipX = true;
                    currentDir = DogDir.Left;
                    break;
                case PlayerDirection.Left:
                    spriteR.sortingOrder = 0;
                    currentDir = DogDir.Right;
                    break;
                case PlayerDirection.Up:
                    currentDir = DogDir.Down;
                    break;
                case PlayerDirection.Down:
                    currentDir = DogDir.Up;
                    if (player.moving == true)
                        fixY = -.03125f;
                    break;
            }
            animator.Play("Dog_PickedUp_" + player.CurrentDir);
            if (pAnim.currentAction == PlayerAction.CarryIdleDog) { fixY = 0;}

                transform.position = new(jumpingPositions[((int)currentDir)].x + player.transform.position.x,
                    jumpingPositions[((int)currentDir)].y + player.transform.position.y + bobNum + fixY);
            return;
        }

        if (player.hasStick && noCoStarted == true)
        {
            StartCoroutine(FetchCo());
            noCoStarted = false;
        }

        if (moving)
        {
            Direction(targetT);
            FollowPlayer();
        }
        if (currentState != DogState.Resting)
        {
            return;
        }
        timer++;
        if (timer <= 60)
        {
            if (PlayerNear(5))
            {
                Debug.Log("Is that Bob?!?!");
                moving = true;
                return;
            }
            timer = 0;
        }
    }

    private void Direction(Transform target)
    {
        direction.x = target.position.x - transform.position.x;

        if (target == player.transform)
        direction.y = target.position.y - .5f - transform.position.y;
        else
        {
            direction.y = target.position.y - transform.position.y;
        }

        spriteR.sortingOrder = 0;
        spriteR.flipX = false;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Chooses largest absolute number for direction
        {
            if (direction.x > 0)
            {
                currentDir = DogDir.Right;
            }
            else
            {
                spriteR.flipX = true;
                currentDir = DogDir.Right; // Actually left, but Animator.Play, 1 less animation to make
            }
        }
        else
        {

            if (direction.y > 0)
            {
                spriteR.sortingOrder = 3;
                currentDir = DogDir.Up;
            }
            else
            {
                currentDir = DogDir.Down;
            }
        }
    }

    private void FollowPlayer() //Transform target
    {
        float difference = Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2)); // a^2 + b^2 = c^2
        if (difference < 1.5) // If within a certain distance of the player, stop moving, change anim to be wagging
        {
            if (targetT != player.transform)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetT.position, speed * speedModifier * Time.deltaTime);
                if (difference < .3f)
                {
                    StartCoroutine(GrabStickCo());
                    return;
                }
            }
            currentState = DogState.Wagging;
            animator.Play("Dog_Wagging_" + currentDir);

            if (GetComponentInChildren<Stick>() == true)
            {
                GetComponentInChildren<Stick>().gameObject.transform.parent = null;
            }
            return;
        }
        else if (difference < 4)
        {
            if (targetT == player.transform) speedModifier = 2;

            currentState = DogState.Walking;
            animator.Play("Dog_Walking_" + currentDir);
        }
        else
        {
            timer++;
            if (timer <= 60)
            {
                if (PlayerNear(8) == false)
                {
                    currentState = DogState.Resting;
                    animator.Play("Dog_Wagging_" + currentDir);
                    return;
                }
            }
                    currentState = DogState.Running;
            animator.Play("Dog_Walking_" + currentDir);
            speedModifier = 5f;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetT.position, speed * speedModifier * Time.deltaTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player.Interactable == this || player.Interactable == null)
            {
                player.Interactable = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player.Interactable == this && currentState != DogState.PickedUp)
            {
                player.Interactable = null;
            }
        }
    }

    private bool PlayerNear(int Distance)
    {
        targetT = player.transform;
        speedModifier = 2;
        direction.x = player.transform.position.x - transform.position.x;
        direction.y = player.transform.position.y - .5f - transform.position.y;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Distance, LayerMask);

        if (hit.collider == null) return false;

        if (hit.collider.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator FetchCo()
    {
        yield return new WaitUntil(()=>player.hasStick == false);

        direction.x = FindFirstObjectByType<Stick>().gameObject.transform.position.x - transform.position.x;
        direction.y = FindFirstObjectByType<Stick>().gameObject.transform.position.y - transform.position.y;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 10, LayerMask);

        noCoStarted = true;

        if (hit.collider == null) yield break;

        if (hit.collider.CompareTag("Projectile"))
        {
            targetT = FindFirstObjectByType<Stick>().gameObject.transform;
            moving = true;
            speedModifier = 3;
        }
    }

    private IEnumerator GrabStickCo()
    {
        paused = true;
        targetT.parent = this.transform;
        targetT.position = this.transform.position;
        yield return new WaitForSeconds(.25f);
        paused = false;
        targetT = player.transform;
    }

    public void Interact(PlayerMovement player)
    {
        if (currentState != DogState.PickedUp) // On ground still
        {
            player.paused = true;
            player.currentState = PlayerState.interact;
            pAnim.currentAction = PlayerAction.LiftDog;
            pAnim.MakePlayerMove();
            cCollider.isTrigger = true;
            transform.parent = player.transform;
            animator.Play("Dog_Jumping_" + currentDir);
            StartCoroutine(PickedUpCo());
        }
        else
        {
            pAnim.currentAction = PlayerAction.Idle;
            float modifier = 1;
            if (player.lastDir.y < 0) { modifier = 1.5f; }

            Vector3 newPos = new(transform.parent.position.x + player.lastDir.x, transform.parent.position.y + player.lastDir.y * modifier);
            transform.position = newPos;
            transform.parent = originalParent;
            cCollider.isTrigger = false;
            moving = true;
            currentState = DogState.Wagging;
            animator.Play("Dog_Wagging_" + currentDir);
            if (player.Interactable == this)
                player.Interactable = null;
        }
    }

    private IEnumerator PickedUpCo()
    {
        paused = true;

        for (int i = 0; i < jumpingPositions.Length; i++)
        {

            jPosHolder[i] = new(jumpingPositions[i].x + player.transform.position.x,
                jumpingPositions[i].y + player.transform.position.y);
        }

        while (transform.position.x != jPosHolder[(int)pAnim.currentDirection].x ||
        transform.position.y != jPosHolder[(int)pAnim.currentDirection].y)
        {
            yield return new WaitForFixedUpdate();
            transform.position = Vector2.MoveTowards(transform.position, jPosHolder[(int)pAnim.currentDirection],
                speed * 4 * Time.fixedDeltaTime);
        }
        player.paused = false;
        paused = false;
        currentState = DogState.PickedUp;
        player.currentState = PlayerState.walk;
        pAnim.currentAction = PlayerAction.CarryDog;
        pAnim.MakePlayerMove();
    }

    private IEnumerator BobbingCo() // Waiting time should be 60 frames a second, 10 frames for the for loops
    {
        bobbing = true;
        
        while (pAnim.currentAction == PlayerAction.CarryDog)
        {
            if (pAnim.spriteRenderer.sprite.name == "Player Spritesheet_64" || pAnim.spriteRenderer.sprite.name == "Player Spritesheet_66"
                 || pAnim.spriteRenderer.sprite.name == "Player Spritesheet_67" || pAnim.spriteRenderer.sprite.name == "Player Spritesheet_72"
                  || pAnim.spriteRenderer.sprite.name == "Player Spritesheet_74")
            {
                bobNum = -.0625f;
                transform.position = new(jumpingPositions[((int)currentDir)].x + player.transform.position.x,
    jumpingPositions[((int)currentDir)].y + player.transform.position.y + bobNum + fixY);
            }
            else
            {
                bobNum = 0;
                transform.position = new(jumpingPositions[((int)currentDir)].x + player.transform.position.x,
    jumpingPositions[((int)currentDir)].y + player.transform.position.y + bobNum + fixY);
            }
            if (pAnim.currentAction == PlayerAction.CarryIdleDog)
            {
                bobbing = false;
                bobNum = 0;
                yield break;
            }
            yield return null;
        }
        bobNum = 0;
        bobbing = false;
    }

    public IEnumerator TreatCo()
    {
        // animator.Play("Dog_Spin" + currentDir);
        sounds.PlayEffect(bark, 1);
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2);
        GetComponent<ParticleSystem>().Stop();
    }
}
