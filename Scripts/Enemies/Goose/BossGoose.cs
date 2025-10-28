using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentAction
{
    Patrol,
    Guard,
    Throw,
    Attack
}

public class BossGoose : Enemy
{
    //speed
    //attackDamage
    //maxHealth
    //health
    //invincible
    //animator    set
    //player      Set
    //pTransform   Set
    //paused

    private RaycastHit2D hit;
    public Transform[] points;
    public int currentPoint;
    private bool changePoint;
    public bool patroling;
    public bool circlePatrol;
    private LayerMask layermask = 1 << 7;


    public CurrentAction currentAction;
    public Goose[] geese;
    private Goose gooseToThrow;


    private float currentTime;

    private int nextAction;

    private new void Start()
    {
        currentAction = CurrentAction.Patrol;
    }

    private void FixedUpdate()
    {

        //Vector3 difference = pTransform.position - transform.position;
        //if (difference.sqrMagnitude > 49)
        //{
        //    currentAction = CurrentAction.Patrol;
        //}

        if (currentAction == CurrentAction.Patrol)
        {
            Patrol();
            if (currentTime < .25f)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                PlayerVisibility();
                currentTime = 0;
            }
        }
    }

    public void DecideAction()
    {
        // Can't be Patrol
        nextAction = Random.Range(0, 3);

        if (health <= 1)
        {
            nextAction = Random.Range(0, 2);
        }

        if (nextAction == 0) // Guard
        {
            // PROTECT THY KING THOU LOWLY ONES
        }

        if (nextAction == 1) // Throw
        {
            // COME HITHER!!! THOU SHALT BE LOBBED

            foreach (Goose goose in geese)
            {
                if (Mathf.Abs(goose.transform.position.x - transform.position.x) < 1.5f
                    && Mathf.Abs(goose.transform.position.y - transform.position.y) < 1.5f)
                {
                    gooseToThrow = goose;
                    break;
                }
            }

            gooseToThrow.transform.position = new(transform.position.x, transform.position.y + 2);
            gooseToThrow.StartCoroutine("ThrowCo");
        }

        if (nextAction == 2) // Attack
        {
            foreach (Goose goose in geese)
            {
                goose.currentState = EnemyState.Attack;
                goose.transform.parent = transform.parent;
            }
            // BOB HATH ANGERED THINE LORD
        }
    }

    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime * 0.5f);

        var pointsDifference = transform.position - points[currentPoint].position;
        if (Mathf.Abs(pointsDifference.sqrMagnitude) < .1 && changePoint == true)
        {
            //if (changePoint == true)
            //{
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
            //}
        }
        else
        {
            changePoint = true;
        }
    }
    private void PlayerVisibility()
    {
        hit = Physics2D.Raycast(transform.position, points[currentPoint].position, 6f, layermask);

        if (hit.collider != null)
        {
            Debug.Log("hit something");
            if (hit.collider.CompareTag("Player"))
            {
                DecideAction();
                Debug.Log("player in sight");
            }
        }
    }
 
    private void GuardMovement()
    {
        
    }
    private IEnumerator GuardCo()
    {
        yield return null;
    }
}
