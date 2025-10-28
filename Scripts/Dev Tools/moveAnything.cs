using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveAnything : MonoBehaviour
{
    public float speed;

    private float Horizontal;
    private float Vertical;

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(Horizontal * speed * Time.deltaTime, Vertical * speed * Time.deltaTime, 0);
    }
}
