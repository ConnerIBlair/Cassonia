using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Seed : MonoBehaviour
{// Add slowing down as it goes further when thrown?
    
    public enum SeedType { Fire, Wind, Nature}

    public SeedType seedType;

    [HideInInspector]
    public AudioClip boomSound;

    public int damage;
    [SerializeField] float reach;

    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] float timeToIgnite;

    [SerializeField] Rigidbody2D rb;

    public Collider2D[] colliders; // Enemies, Boxes, Whatever

    private bool Exploded;

    public PlayerMovement player;

    private Vector3 target;
    public Vector2 landing;

    public bool move = true;
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Sprite sprite;

    protected bool boom;
    private bool noPlayerDetection = false;

    public ParticleSystem ps;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (move == true)
        {
            StartCoroutine(BoomBoomCo());
        }
        else
        {
            noPlayerDetection = true;
        }
        //move = true;
        spriteRenderer.sprite = sprite;
        animator.speed = speedMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.position != target)
        {
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            Boom();
        }
        if (other.CompareTag("Player") && seedType == SeedType.Wind)
        {
            Boom();
        }
    }

    private IEnumerator BoomBoomCo()
    {
        int x = 0;
        int y = 0;
        if (player.lastDir.x != 0)
        {
            x = (int)Mathf.Floor(player.lastDir.x * 3);
            if (player.lastDir.x < 0) { landing.x /= -4; } else { landing.x /= 4; }
        }
        if (player.lastDir.y != 0)
        {
            y = (int)Mathf.Floor(player.lastDir.y * 3);
            if (player.lastDir.y < 0) { landing.y /= -4; } else { landing.y /= 4; }
        }

        target = new Vector3(player.transform.position.x + landing.x + x, player.transform.position.y - .5f + landing.y + y);
        animator.Play($"{seedType}seed_Bounce");

        float dif = speedMultiplier * 24 - 24;

        for (int i = 0; i < 24 - dif; i++) // 24 frame wait
        {
            yield return new WaitForFixedUpdate();
        }
        if (Random.Range(0, 3) <= 1) // 66% chance
            spriteRenderer.flipX = true;

        for (int i = 0; i < 24 - dif; i++) // 24 frame wait
        {
            yield return new WaitForFixedUpdate();
        }
        if (Random.Range(0, 3) == 0)
        {
            if (spriteRenderer.flipX == true)
                spriteRenderer.flipX = false;
            else { spriteRenderer.flipX = true; }
        }

        yield return new WaitUntil(() => transform.position == target);
        animator.Play("None");
        move = false;
        animator.speed = 1;
        yield return new WaitForSeconds(timeToIgnite);
        Boom();
    }

    public void Boom()
    {
        if (!animator) { animator = GetComponentInChildren<Animator>(); }

        if (!Exploded)
        {
            Exploded = true;
            StopAllCoroutines();
            StartCoroutine(DestroyCo());
        }
    }

    //private void NewBoom()
    //{
    //    colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reach);

    //    for (int i = 0; i < colliders.Length; i++)
    //    {
    //        string colliderTag = colliders[i].gameObject.tag;
    //        switch (colliderTag)
    //        {
    //            case "Player":
    //                if (seedType == SeedType.Wind)
    //                    colliders[i].GetComponent<PlayerMovement>().WindJump();
    //                break;
    //            case "Seed":
    //                colliders[i].GetComponent<Seed>().Boom();
    //                break;
    //            case "Enemy":
    //                colliders[i].GetComponent<Enemy>().StartCoroutine($"{seedType}Co", 1); // Stun time
    //                colliders[i].GetComponent<Enemy>().Health(damage);
    //                break;
    //            case "Throwable":
    //                colliders[i].GetComponent<Bush>().player = player;
    //                colliders[i].GetComponent<Bush>().StartCoroutine($"{seedType}Co");
    //                break;
    //        }
    //    }
    //}
    private IEnumerator DestroyCo()
    {
        animator.Play($"{seedType}seed_Explode");

        if (boomSound != null)
        {
            player.sounds.PlayEffect(boomSound);
        }

        yield return new WaitForSeconds(.16f);
        if (noPlayerDetection == true)
        {
            colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reach, 1 << 0 | 1 << 3);
        }
        else
        {
            colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reach);
        }
        boom = true;
    }

    private void FixedUpdate()
    {
        if (move == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 3 * Time.fixedDeltaTime * speedMultiplier);
        }
    }
}
