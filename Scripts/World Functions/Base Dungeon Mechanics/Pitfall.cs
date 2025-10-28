using UnityEngine;

public class Pitfall : MonoBehaviour
{
    private PlayerMovement player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerMovement>();
            if (player.currentState == PlayerState.jump)
            {
                return;
            }
            Debug.Log("AHHHHHHH");
        
        }
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().Fall();
        }
    }
}
