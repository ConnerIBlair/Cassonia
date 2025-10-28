using UnityEngine;
using System.Collections;
using System;

public class IceSoldier : Enemy
{
    //protected float speed;
    //protected int attackDamage;
    //protected int maxHealth;
    //public int health;
    //public int defense;

    //public bool invincible;
    //public Animator animator;
    //public VolumeScript volumeScript;
    //public PlayerMovement player;
    //protected Transform pTransform;
    //public bool paused;

    protected float speedY;
    protected float speedX;

    public int radius;

    private int radiussquared;

    protected Vector3 difference;
    private Vector3 pathFindPos;
    private bool moving = false;
    private int regenerate;

    private int weaponHealth = 1;
    private int shieldHealth = 3;

    [SerializeField]
    private GameObject shieldObj;
    [SerializeField]
    private GameObject weaponObj;

    public EnemyAnimator eAnim;

    public Transform[] points;
    public int currentPoint;
    private bool changePoint;
    public bool patrolling;
    public bool circlePatrol;
    private LayerMask layermask = 1 << 7;

    private RaycastHit2D hit;

    private Coroutine lastCo;

    private void Awake()
    {
        patrolling = true;
        radiussquared = radius * radius;
        Decide();
    }

    private void IceMelting(int damage)
    {
        patrolling = false;
        if (shieldHealth != 0)
        {
            DamageShield(damage);
            Debug.Log("SHIELD");
            return;
        }
        if (weaponHealth != 0)
        {
            DamageWeapon(damage);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (onFire)
        {
            IceMelting(3);
            return;
        }
        if (beingHit)
        {
            beingHit = false;
            Debug.Log("Hit");
            IceMelting(1);
            return;
        }

        if (paused){ return; }

        if (moving == true)
        {
            Move();
        }
        if (patrolling == true)
        {
            Patrol();
        }
    }
    //private void changeSpeedX()
    //{
    //    if (transform.position.x < player.transform.position.x)
    //    {
    //        speedX = speed;
    //    }
    //    else
    //    {
    //        speedX = speed * -1;
    //    }
    //}

    //private void changeSpeedY()
    //{
    //    if (transform.position.y < player.transform.position.y)
    //    {
    //        speedY = speed;
    //    }
    //    else
    //    {
    //        speedY = speed * -1;
    //    }
    //}

    private void Move()
    {
        difference = player.transform.position - transform.position;

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
            patrolling = true;
        }


        //if (difference.sqrMagnitude < radiussquared)
        //{
        //    if (transform.position.x > player.transform.position.x + .05f || transform.position.x < player.transform.position.x - .05f)
        //    {
        //        changeSpeedX();
        //        transform.position += new Vector3(speedX * Time.deltaTime, 0, 0);
        //    }
        //    else
        //    {
        //        speedX = 0;
        //    }
        //    if (transform.position.y > player.transform.position.y + .05f || transform.position.y < player.transform.position.y - .05f)
        //    {
        //        changeSpeedY();
        //        transform.position += new Vector3(0, speedY * Time.deltaTime, 0);
        //    }
        //    else
        //    {
        //        speedY = 0;
        //    }
        //}
        //else
        //{
        //    patrolling = true;
        //    speedY = 0;
        //    speedX = 0;
        //}
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

    private void HitRay()
    {
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                lastCo = StartCoroutine(NoPatrolCo());
            }
        }
    }

    private void Patrol()
    {
        moving = false;
        Vector3 oldPosition = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime);

        Vector3 posToBe = (transform.position - oldPosition) * 150; // 3 units in front

        if (Mathf.Abs(oldPosition.x - transform.position.x) > Mathf.Abs(oldPosition.y - transform.position.y))
        { // x dif greater than y
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x, transform.position.y + posToBe.y + 1), 3f, layermask);
            HitRay();
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x, transform.position.y + posToBe.y), 3f, layermask);
            HitRay();
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x, transform.position.y + posToBe.y - 1), 3f, layermask);
            HitRay();
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x + 1, transform.position.y + posToBe.y), 3f, layermask);
            HitRay();
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x, transform.position.y + posToBe.y), 3f, layermask);
            HitRay();
            hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x + posToBe.x - 1, transform.position.y + posToBe.y), 3f, layermask);
            HitRay();
        }
        hit = Physics2D.Raycast(transform.position, transform.position + player.transform.position, 2, layermask); // Towards the player 2 units
        HitRay();

        var pointsDifference = transform.position - points[currentPoint].position;
        if (Mathf.Abs(pointsDifference.sqrMagnitude) < .1)
        {
            if (changePoint == true)
            {
                changePoint = false;

                if (currentPoint + 1 == points.Length)
                {
                    if (circlePatrol)
                    {
                        currentPoint = 0;
                    }
                    else
                    {
                        currentPoint--;
                    }
                }
                else
                {
                    currentPoint++;
                }
            }
        }
        else
        {
            StartCoroutine(ChangePointCo());
        }
    }
    private IEnumerator NoPatrolCo()
    {
        if (speed != 0)
        {
            int speedHolder;
            speedHolder = speed;
            speed = 0;
            yield return new WaitForSeconds(.5f);
            //hit = null;
            speed = speedHolder;
            patrolling = false;
            moving = true;
            lastCo = StartCoroutine(AttackCo());
        }
    }
    private IEnumerator ChangePointCo()
    {
        yield return new WaitForSeconds(.5f);
        // Animation
        for (int i = 0; i < 5; i++)
        {
            hit = Physics2D.Raycast(transform.position, transform.position - player.transform.position, 3, layermask);
            if (hit.collider != null && hit.collider.tag == "player")
            {
                patrolling = false;
                StartCoroutine(AttackCo());
                yield break;
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.5f);
        changePoint = true;
    }

    private void Decide()
    {
        regenerate += 1;
        Vector2 direction = new(player.transform.position.x - transform.position.x, player.transform.position.x - transform.position.x);
        float difference = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(direction.x), 2) + Mathf.Pow(Mathf.Abs(direction.y), 2));

        if (regenerate > 3 && FullyEquipped() != true)
        {
            regenerate = 0;
            StartCoroutine(RegenerateCo());
            return;
        }

        if (difference < 6 && health > 2 && weaponHealth != 0)
        {
            lastCo = StartCoroutine(AttackCo());
            return;
        }
        if (difference > 6 && health <= 2 && shieldHealth != 0)
        {
            lastCo = StartCoroutine(DefendCo());
            return;
        }
    }

    private IEnumerator AttackCo() // Faster moving, directly toward player, 
    {
        moving = true;
        yield return new WaitForSeconds(4);
        Decide();
    }

    private IEnumerator DefendCo() // Slow, moving toward player with shield up
    {
        yield return null;
    }
    private IEnumerator RegenerateCo()
    {
        Debug.Log("Regenerate");
        paused = true;
        if (weaponHealth == 0)
        {
            weaponHealth = 1;
            weaponObj.SetActive(true);
            // animate and reset weapon
        }
        if (shieldHealth == 0)
        {
            shieldHealth = 3;
            defense = 1;
            shieldObj.SetActive(true);
            // animate and reset shield
        }
        yield return new WaitForSeconds(3);
        paused = false;
    }
    private bool FullyEquipped()
    {
        if (weaponHealth == 0 || shieldHealth == 0) return false; 
        else return true;
    }

    public void DamageWeapon(int damage)
    {
        weaponHealth-= damage;
        StopCoroutine(lastCo);
        if (weaponHealth <= 0)
            StartCoroutine(DestroyWCo());
    }
    private IEnumerator DestroyWCo()
    {
        weaponObj.SetActive(false);
        regenerate = 0;
        paused = true;
        weaponHealth = 0;
        //animator.Play("IceEnemy_Stunned"); // + direction
        // Weapon Animation
        yield return new WaitForSeconds(2);
        Decide();
    }
    public void DamageShield(int damage)
    {
        shieldHealth -= damage;
        StopCoroutine(lastCo);
        if (shieldHealth <= 0)
            StartCoroutine(DestroySCo());
    }
    private IEnumerator DestroySCo()
    {
        shieldObj.SetActive(false);
        regenerate = 0;
        paused = true;
        shieldHealth = 0;
        defense = 0;
        //animator.Play("IceEnemy_Stunned"); // + direction
        // Shield Animation
        yield return new WaitForSeconds(2);
        Decide();
    }
}
