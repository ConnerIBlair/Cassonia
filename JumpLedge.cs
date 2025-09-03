using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLedge : MonoBehaviour
{
    public PlayerMovement playerS;
    public float timeTillJump;

    // Make sure that the player can't jump out of bounds from directional based jumps
    // make sure can jump off of 30, 45, and 90 degree cliffs
    // 1 tall and 2 tall cliffs
    // walk up to edge, keep on walking in that dir, jump
    // animation of shadow
    // animation of player
    // shadow and player line up
    // TREVOR PROOF IT!!!



    //   NOTES FOR BRIDGE MECHANICS
    // Enemies should be able to walk on top while the player is underneath without interacting with eachother
    // But when the player shoots an arrow or swings on top of the bridge, an enemy underneath the bridge should not get hit

    // Idea
    // Change a bool in any enemy or player attack script to say on/off bridge,









    // TILEMAP THINGS. Change Tile data for tiles that you can jump off of and detect
    // if the tile in front of the player is that tile so that he can jump off.
    // Changing Tile Data of tiles you can jump from 



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Jump"))
        {
            StartCoroutine(ShouldJumpCo());
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Jump"))
        {
            StopCoroutine(ShouldJumpCo());
        }
    }

    private IEnumerator ShouldJumpCo()
    {
        yield return null;
    }
}
