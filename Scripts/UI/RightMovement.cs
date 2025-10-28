using UnityEngine;

public class RightMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;

    private Vector3 change = new(1, 0, 0);

    void FixedUpdate()
    {
        transform.position += change * Time.deltaTime * speed;
    }
}
