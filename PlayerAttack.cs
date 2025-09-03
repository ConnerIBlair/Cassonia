using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage;

    public GameObject enemy;

    public float animationTime;

    public Animator sAnimator;

    public PlayerAnimator pAnimator;
    public PlayerMovement pMovement;

    public GameObject NorthCollider;
    public GameObject SouthCollider;
    public GameObject EastCollider;
    public GameObject WestCollider;

    [SerializeField]
    private AudioClip slashSound;

    private VolumeScript volumeScript;
    private void Start()
    {
        pAnimator = this.GetComponentInParent<PlayerAnimator>();
        volumeScript = FindFirstObjectByType<VolumeScript>();
    }

    private void ActivateCollider()
    {
        sAnimator.GetComponent<SpriteRenderer>().flipX = false;
        if (pAnimator.currentDirection == PlayerDirection.Up)
        {
            NorthCollider.SetActive(true);
            sAnimator.Play("Sword_Swipe_Up");
        }
        if (pAnimator.currentDirection == PlayerDirection.Down)
        {
            SouthCollider.SetActive(true);
            sAnimator.Play("Sword_Swipe_Down");
        }
        if (pAnimator.currentDirection == PlayerDirection.Right)
        {
            EastCollider.SetActive(true);
            sAnimator.Play("Sword_Swipe_Right");
        }
        if (pAnimator.currentDirection == PlayerDirection.Left)
        {
            WestCollider.SetActive(true);
            sAnimator.GetComponent<SpriteRenderer>().flipX = true;
            sAnimator.Play("Sword_Swipe_Left");
        }
    }

    public IEnumerator AttackCo()
    {
        NorthCollider.SetActive(false);
        EastCollider.SetActive(false);
        SouthCollider.SetActive(false);
        WestCollider.SetActive(false);

        yield return null; // May make attacking feel slow

        pAnimator.currentAction = PlayerAction.Action;
        pAnimator.MakePlayerMove();
        ActivateCollider();

        //volumeScript.PlayEffect(slashSound, 1);

        yield return new WaitForSeconds(animationTime);
        enemy = null;
        sAnimator.Play(null);
        sAnimator.GetComponent<SpriteRenderer>().sprite = null;
        pMovement.currentState = PlayerState.walk;
        pAnimator.MakePlayerMove(); // Sets animation back to idle

        this.gameObject.SetActive(false);
    }
}
