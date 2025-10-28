using UnityEngine;
using System.Collections;
public class BasicShooter : Enemy
{
    public float stallTime;
    public bool pacing = true;
    private LayerMask layermask = 1 << 7;

    private int speedY;
    private int speedX;

    public int radius;

    private int radiussquared;

    private Vector3 difference;

    private RaycastHit2D hit;

    private int x, y;
    private int pastX = 1, pastY = 1;
    private bool inProgress = false;

    public Sprite projectileSprite;

    private enum EnemyState // Change these names
    {
        Idling,
        Moving, 
        Attack,
        Looking
    }

    [SerializeField]
    private EnemyAnimator eAnim; // Used for multiple enemies so ignore certain enemy states ie idle
    private EnemyState currentState;
    private int decreasedRadius;

    private GameObject shot;

    private void Awake()
    {
        radiussquared = radius * radius;
        decreasedRadius = (radius - 3) * (radius - 3);
        speedX = speed;
        speedY = speed;
    }

    private void FixedUpdate()
    {
        if (paused != true)
        {
            hit = Physics2D.Linecast(transform.position, player.transform.position);
            difference = player.transform.position - transform.position;
            if (currentState == EnemyState.Moving) // !pacing
            {
                if (difference.sqrMagnitude < radiussquared)
                {
                    SpotToMove();
                    if (transform.position.x > player.transform.position.x + .5f || transform.position.x < player.transform.position.x - .5f)
                    {
                        changeSpeedX();
                        transform.position += new Vector3(speedX * Time.deltaTime, 0, 0);
                    }
                    else
                    {
                        StartCoroutine(ShootCo());
                        speedX = 0;
                        eAnim.x = 0;
                    }
                    if (transform.position.y > player.transform.position.y + .5f || transform.position.y < player.transform.position.y - .5f)
                    {
                        changeSpeedY();
                        transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
                    }
                    else
                    {
                        StartCoroutine(ShootCo());
                        speedY = 0;
                        eAnim.y = 0;
                    }
                }
                else if (hit.collider != null || difference.sqrMagnitude > radiussquared) 
                {
                    //pacing = true;
                    currentState = EnemyState.Idling;
                }
            }
            else if (currentState != EnemyState.Attack)
            {
                if (DetectPlayer())
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

    private void SpotToMove() 
    {
        // Whichever way the player is closest, line up there.
        // Player closer x than y, line up enemy with player on y axis
        if (difference.x < difference.y) // Closer on x
        {
            // Move above or below the player
        }
        else
        {
            // Move to the left or right of the player
        }
    }

    private IEnumerator ShootCo()
    {
        int speedHolder;
        speedHolder = speed;
        speed = 0;
        currentState = EnemyState.Attack;

        Debug.Log("Summon Projectile");
        shot = new GameObject("Projectile");
        shot.layer = 3;
        shot.transform.position = transform.position;
        BasicProjectile projectile = shot.AddComponent<BasicProjectile>();
        projectile.sprite = projectileSprite;
        projectile.canRotate = false;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            eAnim.y = 0;
        }
        else
        {
            eAnim.x = 0;
        }
        projectile.dir = new Vector3(eAnim.x, eAnim.y);

        yield return new WaitForSeconds(1);
        speed = speedHolder;
        currentState = EnemyState.Moving;

    }

    private bool DetectPlayer()
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
    private void changeSpeedX()
    {
        if (transform.position.x < player.transform.position.x)
        {
            eAnim.x = 1;
            speedX = speed * 3;
        }
        else
        {
            eAnim.x = -1;
            speedX = speed * -3;
        }
    }
    private void changeSpeedY()
    {
        if (transform.position.y < player.transform.position.y)
        {
            eAnim.y = 1;
            speedY = speed * 3;
        }
        else
        {
            eAnim.y = -1;
            speedY = speed * -3;
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
                //pacing = false;
                currentState = EnemyState.Attack;
            }
            StartCoroutine(PacingCo());
        }
    }

    private IEnumerator PacingCo()
    {
        // stop moving, look around
        eAnim.currentState = EnemyAnimator.EnemyState.Look;
        currentState = EnemyState.Looking;
        eAnim.animator.speed = 1; // Animation speed back to original
        int speedHolder;
        speedHolder = speed;
        speed = 0;
        yield return new WaitForSeconds(2);
        //hit = null;
        speed = speedHolder;

        eAnim.currentState = EnemyAnimator.EnemyState.Move; // walking
        currentState = EnemyState.Idling;
        yield return new WaitForSeconds(4);
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
        else if (x != 0 || y != 0) // Potential problem?? pacing && 
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
            //pacing = false;
            currentState = EnemyState.Moving;
            x = 0;
            y = 0;
            eAnim.currentState = EnemyAnimator.EnemyState.Move; // chasing animation
            eAnim.animator.speed = 2; // Animation speed up for chasing vs. walking

            int speedHolder;
            speedHolder = speed;
            speed = 0;
            yield return new WaitForSeconds(stallTime);
            speed = speedHolder;
        }
    }
}
