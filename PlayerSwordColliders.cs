using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordColliders : MonoBehaviour
{
    public PlayerAttack pAttack;
    public Knockback knockB;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(CHANGETHISCo(other));
        }
        if (other.CompareTag("Pot"))
        {
            knockB.rbody = other.attachedRigidbody;
            knockB.KnockbackObject(other.transform);
        }
        if (other.CompareTag("Throwable"))
        {
            StartCoroutine (CHANGETHISCoTOO(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            pAttack.enemy = null;
        }
    }
    private IEnumerator CHANGETHISCo(Collider2D other)
    {
        yield return new WaitForSeconds(.16f); // Change because of this... it should hit it when the sword reaches the target
        pAttack.enemy = other.gameObject;
        other.GetComponent<Enemy>().Health(pAttack.damage);
        pAttack.StartCoroutine("AttackCo"); // Originally not here
        knockB.rbody = other.attachedRigidbody;
        knockB.KnockbackObject(other.transform);
    }
    private IEnumerator CHANGETHISCoTOO(Collider2D other)
    {
        if (other.GetComponent<Bush>().dead) { yield break; }
        yield return new WaitForSeconds(.16f); // Change because of this... it should hit it when the sword reaches the target
        other.GetComponent<Bush>().player = GetComponentInParent<PlayerMovement>();
        other.GetComponent<Bush>().StartCoroutine("DestroyCo");
    }
}
