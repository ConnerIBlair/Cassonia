using System.Collections;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public Vector3 dir;

    public int speed = 5;

    [HideInInspector]
    public Sprite sprite;
    [HideInInspector]
    public string clip;

    [SerializeField]
    private SpriteRenderer sRenderer;
    [SerializeField]
    private Animator animator;

    private int activeFrames;
    [HideInInspector]
    public bool canRotate = true;

    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = .25f;
        if (sprite != null) { sRenderer.sprite = sprite; }
        if (!canRotate)
        {
            if (clip.Length < 1)
            {
                if (dir.x == 1)
                {
                    transform.Rotate(0, 0, -90, Space.World);
                }
                if (dir.x == -1)
                {
                    transform.Rotate(0, 0, 90, Space.World);
                }
                if (dir.y == -1)
                {
                    transform.Rotate(0, 0, 180, Space.World);
                }
            }
            else
            {
                animator.Play(clip);
            }
        }
    }

    private void FixedUpdate()
    {
        if (canRotate){transform.Rotate(0, 0, speed, Space.Self); }
        transform.position += Time.deltaTime * speed * dir.normalized;
        activeFrames++;
        if (activeFrames > 120){ Destroy(gameObject); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().Health(1);
            other.GetComponentInChildren<EnemyKnockback>().KnockbackObject(other.transform, transform);
            StartCoroutine(DestroyCo());
            return;
        }

        StartCoroutine(DestroyCo());
    }
    private IEnumerator DestroyCo()
    {
        circleCollider.enabled = false;

        if (speed - 2 > 0) { speed -= 2; }

        speed *= -1;
        dir = new Vector3(dir.x, dir.y - .5f, dir.z);
        yield return new WaitForSeconds(.13f);
        sRenderer.enabled = false;
        yield return new WaitForSeconds(.08f); // To make sure knockback script runs through all code
        Destroy(gameObject);
    }
}
