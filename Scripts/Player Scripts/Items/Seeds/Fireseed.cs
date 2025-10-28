using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Fireseed : Seed
{    // WORKING ON VELOCITY TO ADD SPEED TO PARTICLE EFFECTS    // WORKING ON VELOCITY TO ADD SPEED TO PARTICLE EFFECTS
    private bool activeCo;
    void Update()
    {
        if (!activeCo)
        {
            StartCoroutine (EmitCo());
        }

        if (boom)
        {
            boom = false;
            NewBoom();
        }
    }
    private void NewBoom()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            string colliderTag = colliders[i].gameObject.tag;
            switch (colliderTag)
            {
                case "Seed":
                    colliders[i].GetComponent<Seed>().Boom();
                    break;
                case "Enemy":
                    if (colliders[i].TryGetComponent<EnemyHurtbox>(out EnemyHurtbox hurtbox))
                    {                    
                        hurtbox.Coroutine2Param("FireCo", 1);// stun time
                        hurtbox.Health(damage);
                        break;
                    }
                    break;
                case "Throwable":
                    colliders[i].GetComponent<Bush>().player = player;
                    colliders[i].GetComponent<Bush>().StartCoroutine($"FireCo");
                    break;
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator EmitCo() // Lava bubble in mc spawns multiple bunched together particles (black and grey) and they spread out.
                                 // Some stick relatively close with the particle while others disperse up quickly
                                 // Each particle has a portion of the fire particle's horizontal speed, immitating wind
    {
        activeCo = true;

        int frames = Random.Range(13, 25);
        if (move) frames /= 2; else frames *= 2;

        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        Emit();
        StartCoroutine(EmitCo());
    }
    public void Emit()
    {
        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams // This is a variable we are making here to assign to the ps we have a reference to.
                                                                     // This is not making a new particle system, just a variable
        {
            position = transform.position,
            velocity = new Vector3(0, .2f, 0) - this.velocity * Random.Range(0, 4),
            
        };
       if (ps)  ps.Emit(ep, 1);
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using Unity.Cinemachine;
//using UnityEngine;

//public class Fireseed : MonoBehaviour
//{// Add slowing down as it goes further when thrown?

//    [HideInInspector]
//    public AudioClip boomSound;

//    public int damage;
//    [SerializeField] float reach;

//    [SerializeField] private float speedMultiplier = 1;
//    [SerializeField] float timeToIgnite;

//    [SerializeField] Rigidbody2D rb;

//    private Collider2D[] colliders; // Enemies, Boxes, Whatever

//    private bool Exploded;

//    public PlayerMovement player;

//    private Vector3 target;
//    public Vector2 landing;

//    private bool move;
//    private Animator animator;

//    [SerializeField]
//    private SpriteRenderer spriteRenderer;

//    public Sprite sprite;

//    private void Start()
//    {
//        animator = GetComponentInChildren<Animator>();
//        StartCoroutine(BoomBoomCo());
//        move = true;
//        spriteRenderer.sprite = sprite;
//        animator.speed = speedMultiplier;
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Enemy"))
//        {
//            Boom();
//        }
//    }

//    private IEnumerator BoomBoomCo()
//    {
//        int x = 0;
//        int y = 0;
//        if (player.lastDir.x != 0)
//        {
//            x = (int)Mathf.Floor(player.lastDir.x * 3);
//            if (player.lastDir.x < 0) { landing.x /= -4; } else { landing.x /= 4; }
//        }
//        if (player.lastDir.y != 0)
//        {
//            y = (int)Mathf.Floor(player.lastDir.y * 3);
//            if (player.lastDir.y < 0) { landing.y /= -4; } else { landing.y /= 4; }
//        }

//        target = new Vector3(player.transform.position.x + landing.x + x, player.transform.position.y - .5f + landing.y + y);
//        animator.Play("Fireseed_Bounce");

//        float dif = speedMultiplier * 24 - 24;

//        for (int i = 0; i < 24 - dif; i++) // 24 frame wait
//        {
//            yield return new WaitForFixedUpdate();
//        }
//        if (Random.Range(0, 3) <= 1) // 66% chance
//            spriteRenderer.flipX = true;

//        for (int i = 0; i < 24 - dif; i++) // 24 frame wait
//        {
//            yield return new WaitForFixedUpdate();
//        }
//        if (Random.Range(0, 3) == 0)
//        {
//            if (spriteRenderer.flipX == true)
//                spriteRenderer.flipX = false;
//            else { spriteRenderer.flipX = true; }
//        }


//        yield return new WaitUntil(() => transform.position == target);
//        animator.Play("None");
//        move = false;
//        animator.speed = 1;
//        yield return new WaitForSeconds(timeToIgnite);
//        Boom();
//    }

//    public void Boom()
//    {
//        if (!Exploded)
//        {
//            Exploded = true;
//            StopCoroutine(BoomBoomCo());
//            StartCoroutine(DestroyCo());
//        }
//    }

//    private void NewBoom()
//    {
//        colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reach);

//        for (int i = 0; i < colliders.Length; i++)
//        {
//            string colliderTag = colliders[i].gameObject.tag;
//            switch (colliderTag)
//            {
//                case "Fireseed":
//                    colliders[i].GetComponent<Fireseed>().Boom();
//                    break;
//                case "Enemy":
//                    colliders[i].GetComponent<Enemy>().StartCoroutine("FireCo", 1);
//                    colliders[i].GetComponent<Enemy>().Health(damage);
//                    break;
//                case "Throwable":
//                    colliders[i].GetComponent<Bush>().player = player;
//                    colliders[i].GetComponent<Bush>().StartCoroutine("FireCo");
//                    break;
//            }
//        }
//    }
//    private IEnumerator DestroyCo()
//    {
//        animator.Play("Fireseed_Explode");

//        if (boomSound != null)
//        {
//            player.sounds.PlayEffect(boomSound);
//        }

//        yield return new WaitForSeconds(.16f);
//        NewBoom();
//        Destroy(gameObject);
//    }

//    private void FixedUpdate()
//    {
//        if (move == true)
//        {
//            transform.position = Vector2.MoveTowards(transform.position, target, 3 * Time.fixedDeltaTime * speedMultiplier);
//        }
//    }
//}
