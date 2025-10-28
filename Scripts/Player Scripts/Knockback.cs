using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public Enemy enemyS;
    public PlayerMovement playerS;
    public float force; 
    public Rigidbody2D rbody;

    public Transform obj2;

    private float differenceX;
    private float differenceY;
    private float radians;
    private float angle;


    private void OnTriggerEnter2D(Collider2D other)
    {
        obj2 = other.transform;
        KnockbackObject(obj2);
    }

    public void KnockbackObject(Transform obj, Vector3 optDir = new Vector3(), float optForce = 0, float stun = .2f) // optForce is 0 because var force isn't
    {                                                                                                                 // compile time complient
        if (optForce == 0) { optForce = force; }

        rbody = obj.GetComponent<Rigidbody2D>();
        enemyS = obj.gameObject.GetComponent<Enemy>();
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

        if (enemyS != null)
        {
            if (enemyS.health <= 0)
            {
                enemyS = null;
                obj2 = null;
                rbody = null;
                return;
            }
            enemyS.paused = true;

        }

        if (playerS)
        playerS.paused = true;

        transform.eulerAngles = new Vector3(0, 0, angle); 
        rbody.linearVelocity = -this.gameObject.transform.right * optForce;
        StartCoroutine(WaitCo(rbody, stun, enemyS)); 
    }

    public IEnumerator WaitCo(Rigidbody2D rb, float stunTime, Enemy enemy) 
    {
        yield return new WaitForSeconds(0.25f);
        rb.linearVelocity = Vector2.zero;

        if (enemy != null)
        {
            enemyS.normalSprite();
            if (enemy.health > 0)
            {
                Debug.Log("Enemy Health: " + enemyS.health);
                yield return new WaitForSeconds(stunTime);
                enemy.paused = false;
            }
            enemy = null;
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