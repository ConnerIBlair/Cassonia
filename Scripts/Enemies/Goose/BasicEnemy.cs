using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class BasicEnemy : Enemy
{
    public float stallTime;
    [SerializeField]
    private float paceTime = 4;
    [SerializeField]
    private float lookTime = 2;
    public int minLookTime = 1;

    public bool pacing = true;
    private LayerMask layermask = 3 | 7;

    private int speedY;
    private int speedX;

    public int radius;

    private int radiussquared;

    private Vector3 difference;
    private Vector3 pathFindPos;

    private RaycastHit2D hit;

    private int x, y;
    private int pastX = 1, pastY = 1;
    private bool inProgress = false;

    [SerializeField]
    EnemyAnimator.EnemyState pacingState = EnemyAnimator.EnemyState.Move;
    [SerializeField]
    EnemyAnimator.EnemyState transitionState = EnemyAnimator.EnemyState.Move;
    [SerializeField]
    EnemyAnimator.EnemyState chaseState = EnemyAnimator.EnemyState.Move;

    [SerializeField]
    private EnemyAnimator eAnim; // Used for multiple enemies so ignore certain enemy states ie idle
    private int decreasedRadius;

    private void Awake()
    {
        radiussquared = radius * radius;
        decreasedRadius = (radius - 3) * (radius - 3);
        speedX = speed;
        speedY = speed;
        sRenderer = eAnim.sRenderer;
    }

    private void FixedUpdate()
    {
        if (paused != true)
        {
            hit = Physics2D.Linecast(transform.position, pTransform.position, layermask);
            difference = pTransform.position - transform.position;
            if (pathFindPos != Vector3.zero) { MoveIt(); return; }
            if (!pacing) // Chase player
            {
                if (difference.sqrMagnitude <= radiussquared) // within range
                {
                    if ((int)transform.position.y == (int)pTransform.position.y || (int)transform.position.x == (int)pTransform.position.x)
                    {
                        if (hit.collider != null)
                        {
                            if (eAnim.x != 0)
                            {
                                StartCoroutine(PathFindX());
                            }
                            else
                            {
                                StartCoroutine(PathFindY());
                            }
                        }
                    }
                    MoveIt();
                    return;
                }

                if (difference.sqrMagnitude > radiussquared) // too far away
                {
                    pacing = true;
                }
            }
            else // If pacing
            {
                if (PlayerInRange())
                {
                    StartCoroutine(NoPatrolCo()); // Start attacking
                }
                else
                {
                    Pacing();
                    transform.position += Time.deltaTime * speed * new Vector3(x, y);
                }
            }
        }
    }

    private void MoveIt()
    {
        transform.position = Vector3.MoveTowards(transform.position, pTransform.position + pathFindPos, speed * 3 * Time.fixedDeltaTime);

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y)) // x dif is greater
        {
            eAnim.y = 0;
            if (transform.position.x > pTransform.position.x)
            {
                eAnim.x = -1;
            }
            else
            {
                eAnim.x = 1;
            }
        }
        if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x)) // y dif is greater
        {
            eAnim.x = 0;
            if (transform.position.y > pTransform.position.y)
            {
                eAnim.y = -1;
            }
            else
            {
                eAnim.y = 1;
            }
        }
    }

    private IEnumerator PathFindX() // player is left or right of enemy
    {
        if (pathFindPos.y == 0)
        {
            pathFindPos.y = 1;
            yield return new WaitForSeconds(1.5f);
            if (hit.collider == null)
            {
                pathFindPos = Vector3.zero;
                yield break;
            }
            pathFindPos.y = -1;
            yield return new WaitForSeconds(1.5f);
            pathFindPos = Vector3.zero;
        }
    }

    private IEnumerator PathFindY()// player is above or below enemy
    {
        if (pathFindPos.x == 0)
        {
            pathFindPos.x = 1;
            yield return new WaitForSeconds(1.5f);
            if (hit.collider == null)
            {
                pathFindPos = Vector3.zero;
                yield break;
            }
            pathFindPos.x = -1;
            yield return new WaitForSeconds(1.5f);
            pathFindPos = Vector3.zero;
        }
    }

    private bool PlayerInRange()
    {
        if (difference.sqrMagnitude < decreasedRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().Health(attackDamage);
            other.GetComponentInChildren<EnemyKnockback>().KnockbackObject(other.transform, transform, optForce: KBForce);
            StartCoroutine(NoPatrolCo());
        }
    }

    private void Pacing()
    {
        if (inProgress == false)
        {
            inProgress = true;
            pastX = -x;
            pastY = -y;
            x = 0;
            y = 0;

        TryAgain:

            x = Random.Range(-1, 2);
            y = Random.Range(-1, 2);
            if (x == 0 && y == 0) { goto TryAgain; }
            if (x == pastX && y == pastY) // Don't move forward then backward
            {
                goto TryAgain;
            }
            if (x != 0 && y != 0)
            {
                int xOrY = Random.Range(0, 2); // 0 or 1
                if (xOrY == 0) { y = 0; } else { x = 0; } // Only x or only y
            }

            eAnim.x = x;
            eAnim.y = y;

            // hit is being set in fixed update
            if (hit.collider == null && difference.sqrMagnitude < radiussquared) // Can see player again and close enough to him
            {
                pastX = speedX;
                pastY = speedY;
                pacing = false;
            }
            StartCoroutine(PacingCo());
        }
    }

    private IEnumerator PacingCo() // Look around
    {        
        eAnim.animator.speed = 1; // Animation speed back to original
        // stop moving, look around
        if (lookTime != 0)
        {
            if (transitionState == EnemyAnimator.EnemyState.Move)
            {
                eAnim.currentState = EnemyAnimator.EnemyState.Look;
            }
            else
            {
                eAnim.currentState = EnemyAnimator.EnemyState.Idle;
            }

            int speedHolder;
            speedHolder = speed;
            speed = 0;
            if (minLookTime > 0)
            {
                yield return new WaitForSeconds(Random.Range(minLookTime, lookTime) + .49f);
            }
            else
            {
                yield return new WaitForSeconds(lookTime + .49f);
            }

            //hit = null;
            speed = speedHolder;
        }

        eAnim.currentState = pacingState; // walking

        yield return new WaitForSeconds(paceTime);
        //yield return new WaitUntil(() => UnitsTraveled() == 3); //Wait till travel 3, 4, or 5 units
        inProgress = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Sword"))
        {
            StartCoroutine(NoPatrolCo()); // Player has been detected
            return;
        }
        else if (x != 0 || y != 0)
        {
            x *= -1;
            y *= -1;
            eAnim.x = x;
            eAnim.y = y;
        }
    }
    private IEnumerator NoPatrolCo() // Should face the player when charging up
    {
        if (speed != 0) // if this has not been started already
        {
            pacing = false;
            x = 0;
            y = 0;
            eAnim.currentState = transitionState; // chasing animation
            if (transitionState == EnemyAnimator.EnemyState.Move)
                eAnim.animator.speed = 2; // Animation speed up for chasing vs. walking

            int speedHolder;
            speedHolder = speed;
            speed = 0;
            yield return new WaitForSeconds(stallTime);
            eAnim.currentState = chaseState;
            //hit = null;
            speed = speedHolder;
            //pacing = false;
        }
    }
}
