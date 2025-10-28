using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] protected float speed;
    protected float baseSpeed;

    public Animator animator;
    [SerializeField]
    protected SpriteRenderer spriteR;

    protected bool canMoveOn = true;
    protected bool moving = false;

    [SerializeField]
    protected Vector3 direction;
    [SerializeField]
    protected Rigidbody2D rb;

    protected bool GOGOGO = false;

    public bool notScared = false;

    private void Start()
    {
        baseSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (notScared)
            {
                return;
            }

            Run(other.transform.position);
            foreach (Animal animal in transform.parent.GetComponentsInChildren<Animal>())
            {

                Vector3 difference = animal.transform.position - transform.position;
                if (difference.sqrMagnitude < 49)
                    animal.Run(other.transform.position);
            }
        }
    }
    public void Run(Vector3 position)
    {
        if (notScared)
        {
            return;
        }

        StopAllCoroutines();
        direction = position - transform.position;

        if (direction.x < 0)
        {
            spriteR.flipX = false; // Opposite of normal because speed is negative
        }
        else
        {
            spriteR.flipX = true;
        }

        canMoveOn = false;
        moving = true;
        speed = -baseSpeed * 4;
        animator.speed = 1.5f;
        GOGOGO = true;
    }
}
