using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    // When doing map mechanics, figure out how to see when player collides
    // with camera border then change to new boundries depending on map position possibly

    public CameraMovement cameraScript;
    public bool leftRight;

    public Vector2 maxPos1;
    public Vector2 minPos1;

    public Vector2 maxPos2;
    public Vector2 minPos2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cameraScript.transitioning = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (leftRight)
            {
                if (collision.gameObject.transform.position.x < this.gameObject.transform.position.x)
                {
                    cameraScript.maxPosition = maxPos2;
                    cameraScript.minPosition = minPos2;
                }
                else
                {
                    cameraScript.maxPosition = maxPos1;
                    cameraScript.minPosition = minPos1;
                }
            }
            else
            {
                if (collision.gameObject.transform.position.y < this.gameObject.transform.position.y)
                {
                    cameraScript.maxPosition = maxPos2;
                    cameraScript.minPosition = minPos2;
                }
                else
                {
                    cameraScript.maxPosition = maxPos1;
                    cameraScript.minPosition = minPos1;
                }
            }
        }
        cameraScript.transitioning = false;
    }
}
