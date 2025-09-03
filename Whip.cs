using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : Linkable, IItem
{


    // Look to ALTTP for boomerang player movement. Player 4 dir, boomerang 8. But instead of boomerang, it's a whip


    public int damage;

    private Vector3 direction;

    private float xOffset;
    private float yOffset;

//    private int fixOffset;

    [SerializeField]
    private int reach = 3;

    private bool canUse = true;

    private RaycastHit2D[] hits;
    public AudioClip hitSound;

    private int layerMask = 1 << 3 | 1 << 10 | 1 << 0;

    private Knockback kb;

    [SerializeField]
    private Animator animator;
    public float WindUpTime;
    private string animDir;

    private PlayerMovement player;

    public void Use(PlayerMovement player)
    {
        this.player = player;
        if (!canUse) { return; }

        canUse = false;
        direction = player.lastDir;
        kb = player.GetComponentInChildren<Knockback>(false);
        StartCoroutine(WhipCo());
        player.sounds.PlayEffect(hitSound, 1);
        //colliders = new RaycastHit2D[3];
    }

    private IEnumerator WhipCo()
    {
        animDir = player.pAnimator.currentDirection.ToString();
        player.pAnimator.currentAction = PlayerAction.Use;
        player.currentState = PlayerState.attack;
        player.pAnimator.MakePlayerMove();

        animator.Play("Whip_" + animDir);
        yield return new WaitForSeconds(WindUpTime);
        RaycastsFunction();

        yield return new WaitForSeconds(.0888f);
        animator.Play("Whip"); 
        player.currentState = PlayerState.walk;
        player.pAnimator.currentAction = PlayerAction.Idle;
        player.pAnimator.MakePlayerMove();
        yield return new WaitForFixedUpdate();
        canUse = true;
    }

    private void RaycastsFunction()
    {
        xOffset = 0;
        yOffset = 0;
        //fixOffset = 1;

        if (direction.x != 0)
        {
            yOffset = .2f;
        }
        if (direction.y < 0)
        {
            xOffset = .2f;
        } else if (direction.y > 0) { xOffset = -.2f; }
        

        if (direction.x == 1 && direction.y == 1) yOffset = 0; xOffset = 0;
        if (direction.x == -1 && direction.y == -1) yOffset = 0; xOffset = 0;

        direction = direction.normalized;
        direction = new Vector3(direction.x + xOffset, direction.y + yOffset);

        hits = new RaycastHit2D[3];

        hits = Physics2D.CircleCastAll(transform.position, .25f, direction, reach, layerMask);

        if (linked)
        {
            AttachedAttack();
        }
        else
        {
            WhipAttack();
        }


        //hit = Physics2D.Raycast(transform.position + new Vector3(xOffset * fixOffset, yOffset), transform.position + direction + new Vector3(xOffset * fixOffset, yOffset), reach, layerMask);
        //WhipAttack();
        //hit = Physics2D.Raycast(transform.position - new Vector3(xOffset * fixOffset, yOffset), transform.position + direction - new Vector3(xOffset * fixOffset, yOffset), reach, layerMask);
        //WhipAttack();
        //hit = Physics2D.Raycast(transform.position, direction, reach, layerMask);
        //WhipAttack();
        //Debug.DrawLine(transform.position, transform.position + direction * 3, Color.white, 2.5f);
        //Debug.DrawLine(transform.position + new Vector3(xOffset * fixOffset, yOffset), transform.position + (direction * 3) + new Vector3(xOffset * fixOffset, yOffset), Color.white, 2.5f);
        //Debug.DrawLine(transform.position - new Vector3(xOffset * fixOffset, yOffset), transform.position + (direction * 3) - new Vector3(xOffset * fixOffset, yOffset), Color.white, 2.5f);
    }

    private void AttachedAttack()
    {
        foreach (RaycastHit2D hit in hits) // Use a switch instead of if statements
        {
            if (hit.collider == null)
            {
                return;
            }
            Seed instantiation = Instantiate(linkee.GetComponent<SeedPouch>().seed).GetComponent<Seed>();
            instantiation.player = player;
            instantiation.transform.position = hit.point;
            instantiation.move = false;
            instantiation.Boom();
        }
    }

    private void WhipAttack()
    {
        foreach (RaycastHit2D hit in hits) // Use a switch instead of if statements
        {
          //  Debug.Log(hits.Length);
            if (hit.collider == null)
            {
                return;
            }
            if (hit.collider.CompareTag("Seed"))
            {
                hit.collider.GetComponent<Seed>().Boom();
               break;
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<Enemy>().Health(damage);
                kb.KnockbackObject(hit.collider.gameObject.transform, direction, stun: 1);
                break;
            }
            if (hit.collider.CompareTag("Lever"))
            {
                hit.collider.GetComponent<Lever>().Activate();
                break;
            }
        }
    }
}
