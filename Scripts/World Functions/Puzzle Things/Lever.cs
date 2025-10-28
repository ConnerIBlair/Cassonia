using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{

    public Transform leverHandle;

    public void Activate()
    {
        Debug.Log("Freeze!");
        Quaternion rotation = new Quaternion();
        rotation.z = 45;
        leverHandle.rotation = rotation;
        FindFirstObjectByType<PlayerMovement>().paused = true;
    }
}
