using UnityEngine;

public class TeleportCloud : MonoBehaviour
{
    public Transform teleportSpot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<Transform>().position = new(teleportSpot.position.x, other.GetComponent<Transform>().position.y);
    }
}
