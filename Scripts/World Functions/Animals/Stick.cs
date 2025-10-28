using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour, IInteractable
{
    public bool pickedUp;

    private PlayerMovement playerS;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerMovement>().Interactable == null)
            other.GetComponent<PlayerMovement>().Interactable = this;
        }
        else if (other.CompareTag("Dog"))
        {
            if (other.GetComponent<Dog>().targetT == this.transform)
            {
                gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                transform.parent = other.transform;
                transform.position = other.transform.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerMovement>().Interactable == this)
            {
                other.GetComponent<PlayerMovement>().Interactable = null;
            }
        }
    }

    private void Update()
    {
        if (pickedUp)
        {
            transform.position = playerS.transform.position;
        }
    }

    public void Interact(PlayerMovement player)
    {
        playerS = player;
        if (pickedUp)
        {
            pickedUp = false;
            StartCoroutine(BADIDEACO());
            transform.parent = null;
            GetComponent<Rigidbody2D>().angularVelocity = 720;
            GetComponent<Rigidbody2D>().AddForce(player.lastDir * 500);
            return;
        }

        pickedUp = true;
        player.hasStick = true;
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        transform.parent = player.transform;
        transform.position = player.transform.position;
    }

    private IEnumerator BADIDEACO()
    {
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        //pickedUp = false;
        playerS.hasStick = false;
    }
}
