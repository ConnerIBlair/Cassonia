using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeInteraction : MonoBehaviour
{


    //                                             Possibly Solved With Bottom Solution
    //                                                      Beta Bridge Problem
    // There's a problem with the current system. If a something is on the bridge, it turns on bridge collision for whatever goes across the bridge.
    // This means that if something is beneath the bridge, then anything on the bridge will interact with it, along with anything on top of the bridge

    // If we make the bridge only interact with itself and anytime something went on to the bridge, then we made its collision layer set to the bridge and
    // kept track of its original layer, then projectiles and the player's sword would interact with the things spawning them in.

    // If there's something that makes it so that child objects don't interact with their parent game object, then this wouldn't be a problem.

    // Or if the sword/projectiles/objects are only trigger colliders and then have a knockback script attached, which will be needed anyway.


    //                                                     Final Bridge Idea
    // When something enters the bridge, change layer to be bridge. This includes projectiles. When a projectile is spawned in,
    // make sure that there is a line of code checking if the parent is on the bridge or not to change the projectile's layer.

    // When items are added, make line of code in player controller that changes item's layer, no matter what it is, to either
    // be rendered on the bridge or not based on the onBridge boolean in this script that would be attached to the player.

    public bool onBridge;
    public bool nearBridge;

    protected int startingLayer;

    public Transform Bridge;

    public float bRadius;

    public Vector3 bDifference;

    public SpriteRenderer sRenderer;

    public SpriteRenderer[] spriteRenderers;

    protected void Start()
    {
        startingLayer = gameObject.layer;
        sRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    //private void FixedUpdate()
    //{
    //    if (onBridge)
    //    {
    //        difference = Bridge.position - transform.position;
    //        if (difference.sqrMagnitude < radiusSquared)
    //        {
    //            nearBridge = true;
    //        }
    //        else
    //        {
    //            nearBridge = false;
    //            NoCollideBridge();
    //            Debug.Log("Not Near Enough");
    //        }
    //    }
    //}

    public bool NearBridge()
    {
        bDifference = Bridge.position - transform.position;
        if (Mathf.Abs(bDifference.x) > bRadius || Mathf.Abs(bDifference.y) > bRadius)
        {            //Debug.Log("Not Near Enough");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ShouldCollideBridge(float radius)
    {
        //Physics2D.IgnoreLayerCollision(gameObject.layer, 8, false); This makes it so the gameobject's layer doesn't interact with layer 8

        gameObject.layer = LayerMask.NameToLayer("AboveBridge");

        this.bRadius = radius;
        onBridge = true;
        //Debug.Log("On Bridge");

        SpriteRendererChange();
    }

    public void NoCollideBridge()
    {
        if (NearBridge() == false || !onBridge)
        {
            gameObject.layer = startingLayer;

            onBridge = false;
            //Debug.Log("Not on Bridge");

            SpriteRendererChange();
        }
    }

    public void SpriteRendererChange()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i].sortingOrder < 11)
            {
                spriteRenderers[i].sortingOrder += 11;
            }
            else if (spriteRenderers[i].sortingOrder >= 11 && !onBridge)
            {
                spriteRenderers[i].sortingOrder -= 11;
            }
        }
    }
}
