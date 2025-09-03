using UnityEngine;
using System.Collections;
using Unity.Hierarchy;
public class DumbShooter : Enemy
{
    private int baseSpeed;

    private bool canMoveOn = true;
    private bool moving = false;

    private Vector3 dir;
    private Rigidbody2D rb;

    [SerializeField]
    private EnemyAnimator eAnim;

    [SerializeField]
    private float stallTime;
    [SerializeField]
    private float moveTime;

    private Vector3 difference;
    public GameObject projectile;
    public Sprite projectileSprite;
    public AnimationClip clip;
    private GameObject shot;

    private void Awake()
    {
        baseSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            rb.MovePosition(transform.position + speed * Time.deltaTime * dir);
        }
        if (canMoveOn)
        {
            canMoveOn = false;
            StartCoroutine(ControlCo());
        }
    }
    private IEnumerator ControlCo()
    {
        IdleMoving();
        if (stallTime != 0)
        {
            eAnim.currentState = EnemyAnimator.EnemyState.Attack;
            yield return new WaitForSeconds(stallTime);
            StartCoroutine(ShootCo());
        }
        speed = baseSpeed;
        eAnim.currentState = EnemyAnimator.EnemyState.Move;
        moving = true;
        yield return new WaitForSeconds(moveTime);
        moving = false;
        canMoveOn = true;
    }

    private IEnumerator ShootCo()
    {
        int speedHolder;
        speedHolder = speed;
        speed = 0;
        moving = false;

        Debug.Log("Summon Projectile");
        shot = Instantiate(projectile);
        if (transform.GetChild(0).childCount > 0)
        {
            Debug.Log("2 kids");
            shot.transform.position = transform.GetChild(0).GetChild(0).position;
        }
        else
        {
            if (dir.x != 0)
            {
                shot.transform.position = new Vector2(transform.position.x, transform.position.y - .1875f);
            }
            else
            {
                shot.transform.position = transform.position;
            }
        }
        BasicProjectile script = shot.GetComponent<BasicProjectile>();
        script.sprite = projectileSprite;
        script.canRotate = false;
        if (clip != null)
        {
            script.clip = clip.name;
        }
        else
        {
            shot.GetComponent<Animator>().enabled = false;
        }


        script.dir = new Vector3(eAnim.x, eAnim.y);

        yield return new WaitForSeconds(1);
        speed = speedHolder;
        moving = true;
    }

    private void IdleMoving()
    {
        dir.x = 0;
        dir.y = 0;

    TryAgain:
        dir.x = Random.Range(-1, 2);
        dir.y = Random.Range(-1, 2);
        if (dir.x == 0 && dir.y == 0) { goto TryAgain; }

        if (dir.x != 0 && dir.y != 0)
        {
            int xOrY = Random.Range(0, 2);
            if (xOrY == 0) { dir.y = 0; } else { dir.x = 0; } 
        }
        eAnim.x = (int)dir.x;
        eAnim.y = (int)dir.y;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Sword"))
        {
            return;
        }
        else if (dir.x != 0 || dir.y != 0)
        {
            dir.x *= -1;
            dir.y *= -1;
            eAnim.x *= -1;
            eAnim.y *= -1;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().Health(attackDamage);
        }
    }
}
