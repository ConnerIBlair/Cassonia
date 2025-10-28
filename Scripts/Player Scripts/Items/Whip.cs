using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : Linkable, IItem
{


    // Look to ALTTP for boomerang player movement. Player 4 dir, boomerang 8. But instead of boomerang, it's a whip

    public BoxDraw_er boxdraw;



    public int damage;
    private Vector2 origin;
    private Vector3 direction;

    private float xOffset;
    private float yOffset;

    //    private int fixOffset;

    [SerializeField]
    private int reach = 3;

    private bool canUse = true;

    private RaycastHit2D[] hits = new RaycastHit2D[3];
    //private int index;
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
            yOffset = -.28125f;
        }
        else
        if (direction.y < 0)
        {
            xOffset = .34375f;
            yOffset = -.3125f;
        }
        else if (direction.y > 0) { xOffset = -.21875f; }
        if (direction.x != 0 && direction.y != 0) { yOffset = 0; xOffset = 0; }

        direction = direction.normalized;

        origin = new Vector2(transform.parent.position.x + xOffset, transform.parent.position.y + yOffset);

        //hits = Physics2D.CircleCastAll(transform.position, .25f, direction, reach, layerMask);
        boxdraw.boxDirection = direction;
        boxdraw.boxDistance = reach;
        boxdraw.boxAngle = 0;
        boxdraw.boxSize = Vector2.one / 2;
        boxdraw.hitLayers = layerMask;
        boxdraw.origin = origin;

        hits = Physics2D.BoxCastAll(transform.parent.position, Vector2.one / 2, 0, direction, reach, layerMask); // The 0 is for the rotation of the square
        if (hits.Length < 1)
        {
            return;
        }
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

    private void AttachedAttack() // Needs to be changed so that it only spawns one seed at the first area it hits
    {
        //foreach (RaycastHit2D hit in hits)
        //{
        if (hits[0].collider == null)
        {
            return;
        }
        if (hits[0].collider.TryGetComponent<Seed>(out Seed s)) // prevents multiplying seeds
        {
            if (hits[0].collider.GetComponent<Seed>().seedType == linkee.GetComponent<SeedPouch>().seed.GetComponent<Seed>().seedType)
            {
                return;
            }
        }
        Seed instantiation = Instantiate(linkee.GetComponent<SeedPouch>().seed).GetComponent<Seed>();
        instantiation.player = player;
        instantiation.transform.position = hits[0].point;
        instantiation.move = false;
        instantiation.Boom();
        //}n
    }

    private void WhipAttack()
    {
        if (hits[0].collider == null)
        {
            return;
        }
        if (hits[0].collider.CompareTag("Seed"))
        {
            hits[0].collider.GetComponent<Seed>().Boom();
        }
        else
        if (hits[0].collider.CompareTag("Enemy"))
        {
            if (hits[0].collider.TryGetComponent<EnemyHurtbox>(out EnemyHurtbox hurtbox))
            {
                hits[0].collider.GetComponent<EnemyHurtbox>().Health(damage);
                kb.KnockbackObject(hits[0].collider.gameObject.transform.parent, direction, stun: 1);
            }
            else if (hits[0].collider.gameObject.name == "Spearhead")
            {
                hits[0].collider.transform.parent.transform.parent.GetComponent<Enemy>().Health(damage);
                kb.KnockbackObject(hits[0].collider.gameObject.transform.parent.transform.parent, direction, stun: 1);
            }
        }
        else
        if (hits[0].collider.CompareTag("Lever"))
        {
            hits[0].collider.GetComponent<Lever>().Activate();
        }
        else // DEFAULT CASE (still hit something)
        {
            //if (hits[index].collider.isTrigger && index < hits.Length - 2) // To not play the hitting effect if you didn't hit anything solid
            //{
            //    index++;
            //    WhipAttack();
            //    return; // To not play the hitting effect if you didn't hit anything solid
            //}
        }
        // Play the hit particle effect
        Array.Clear(hits, 0, hits.Length);
    }
}
//if (hits[0].collider.CompareTag("Enemy") && hits[0].collider.TryGetComponent<EnemyHurtbox>(out EnemyHurtbox hurtbox))
//{
//    if ()
//        hits[0].collider.GetComponent<EnemyHurtbox>().Health(damage);
//    kb.KnockbackObject(hits[0].collider.gameObject.transform.parent, direction, stun: 1);
//    //break;
//}
//else
