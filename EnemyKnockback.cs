using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    private PlayerMovement playerS;
    public float force;
    private Rigidbody2D rbody;

    public Transform obj2;

    private float differenceX;
    private float differenceY;
    private float radians;
    private float angle;

    public void KnockbackObject(Transform obj, Vector3 optDir = new Vector3())
    {
        if (rbody != null) { return; }
        Debug.Log("Knockback " + obj);
        rbody = obj.GetComponent<Rigidbody2D>();
        playerS = obj.gameObject.GetComponent<PlayerMovement>();

        if (optDir != new Vector3())
        {
            optDir += transform.position;
            getAngle(optDir);
        }
        else
        {
            getAngle(obj.position);
        }

        if (playerS)
            playerS.paused = true;

        transform.eulerAngles = new Vector3(0, 0, angle);
        rbody.linearVelocity = -transform.right * force;
        StartCoroutine(WaitCo(rbody)); // , enemyS
    }

    public IEnumerator WaitCo(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;

        if (playerS)
        {
            if (playerS.gameObject.GetComponent<PlayerHealth>().health > 0)
            {
                playerS.paused = false;
            }
            playerS.GetComponent<PlayerHealth>().NormalSprite();
            playerS = null;
        }

        obj2 = null;
        rbody = null;
    }

    private void getAngle(Vector3 obj2)
    {
        differenceX = transform.position.x - obj2.x;
        differenceY = transform.position.y - obj2.y;
        radians = Mathf.Atan2(differenceY, differenceX);
        angle = radians * 57.296f;
    }
}
