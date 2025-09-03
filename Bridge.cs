using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{ // Bridge originally collides with everything but sword.
    public Transform bridgeTrans;

    private float radius;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.tag != "Wall")
        {
            collision.GetComponent<BridgeInteraction>().Bridge = bridgeTrans;
            radius = Mathf.Abs(GetComponent<BoxCollider2D>().offset.x);
            if (radius == 0) radius = Mathf.Abs(GetComponent<BoxCollider2D>().offset.y);
            collision.GetComponent<BridgeInteraction>().ShouldCollideBridge(radius);// + 1
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.tag != "Wall")
        {
            collision.GetComponent<BridgeInteraction>().NoCollideBridge();
            Debug.Log("Exiting");
        }
    }
}
