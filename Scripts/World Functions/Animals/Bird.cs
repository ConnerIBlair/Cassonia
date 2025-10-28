using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bird : Animal
{
    [SerializeField]
    private Tilemap tilemap;

    private Vector3Int position;
    private Vector3 difference;

    [SerializeField]
    private float glideSpeed;
    private Vector3 startPos;

    private string tileName;

    public bool goToSpot;

    [SerializeField]
    private bool perched;

    [SerializeField]
    private Transform player;
    private void Start()
    {
        baseSpeed = speed;
        player = FindFirstObjectByType<PlayerMovement>().transform;
        if (!CanLand(transform.position.x, transform.position.y))
        {
            perched = true;
        }
    }

    private void FixedUpdate()
    {
        if (GOGOGO == true) { animator.Play("Bird_Flying"); StartCoroutine(DestroyCo()); GOGOGO = false; }

        if (moving)
        {
            rb.MovePosition(transform.position + speed * Time.fixedDeltaTime * direction.normalized);
            if (perched)
            {
                difference = position - transform.position;
            }
        }

        if (canMoveOn)
        {
            canMoveOn = false;
            StartCoroutine(ControlCo());
        }
    }

    private IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private IEnumerator ControlCo()
    {
        speed = (float)Random.Range(1, 2);
            yield return new WaitForSeconds(1);
        if (Random.Range(0, 3) <= 1 && CanLand(transform.position.x, transform.position.y)) // 2/3rd chance
        {
            animator.Play("Bird_Idle");
            speed = 0;
            moving = false;
            perched = false;
            canMoveOn = true;

        }
        else
        {
            if (perched)
            {
                if (Random.Range(0, 3) <= 1)
                {
                    StartCoroutine(GlideCo());
                }
                else
                {
                    StartCoroutine(FlyingCo());
                }
            }
            else
            {
                StartCoroutine(FlyingCo());
            }
        }
    }

    private IEnumerator GlideCo()
    {
        goToSpot = false;
        float switchDir;

        float y;
        startPos = transform.position;
        if (CanLand(startPos.x + 4, startPos.y - 2))
        {
            switchDir = 1;
            spriteR.flipX = false;
        }
        else
        {
            switchDir = -1;
            spriteR.flipX = true;
        }

        animator.Play("Bird_Glide");
        yield return new WaitForSeconds(Time.fixedDeltaTime * 5);

        for (float x = 0; x < 4; x += glideSpeed * Time.fixedDeltaTime)
        {
            y = (float).15 * (x + .83f) * Mathf.Pow((x - 4), 2) - 2; // When x is 0, y is 0

            // x = y^2 is pretty close to what we want

            yield return new WaitForFixedUpdate();
            Vector3 newPos = new(x * switchDir, y);
            rb.MovePosition(startPos + newPos);
        }
        animator.Play("Bird_Idle");
        perched = false;
        canMoveOn = true;
    }

    private IEnumerator FlyingCo()
    {
        animator.Play("Bird_Flying");
        perched = false;
        moving = true;
        speed = baseSpeed * 2;
        WhichWay();

        if ((transform.position - player.position).sqrMagnitude > 15 * 15)
        {
            direction = player.position - transform.position;
            if (direction.x < 0) spriteR.flipX = true; else spriteR.flipX = false;
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(2.25f);
        for (float i = 0; i < .75; i += .25f) // seconds
        {
            SurroundingTiles();
            yield return new WaitForSeconds(.25f);
        }

        yield return null;
        
// fly toward player here
        canMoveOn = true;
    }

    private void WhichWay()
    {
        if (Random.Range(0, 2) == 1)
        {
            spriteR.flipX = false;
            direction.x = 1;
        }
        else
        {
            spriteR.flipX = true;
            direction.x = -1;
        }
        direction = new(direction.x, Random.Range(-1, 2));
    }

    private bool CanLand(float xPos, float yPos)
    {
        position = new Vector3Int(Mathf.FloorToInt(xPos), (int)yPos);
        if (tilemap.HasTile(position))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void SurroundingTiles() // Goes in collumns from top left to bottom right
    {
        position = new Vector3Int(Mathf.FloorToInt(transform.position.x), (int)transform.position.y);
        for (int i = -2; i <= 2; i++) // x
        {
            if (perched)
            {
                break;
            }
            for (int j = 2; j >= -2; j--)// y
            {
                Vector3Int surroundPos = new Vector3Int(position.x + i, position.y + j);
                if (!tilemap.HasTile(surroundPos)) continue;

                TileBase spot = tilemap.GetTile(surroundPos);
                tileName = spot.name;
                if (tileName == "Tree Canopy1_5" || tileName == "Tree Canopy1_6")
                {
                    StopAllCoroutines();
                    StartCoroutine(LandOnTreeCo());
                    moving = true;
                    perched = true;
                    position = new(surroundPos.x + 1, surroundPos.y + 1);
                    break;
                }
            }
        }
    }

    private IEnumerator LandOnTreeCo()
    {
        yield return new WaitForFixedUpdate(); // These two line allow for fixed update 
        yield return new WaitForFixedUpdate(); // to run, making sure difference isn't zero;

        direction = position - transform.position;
        if (direction.x < 0) spriteR.flipX = true; else spriteR.flipX = false;
        yield return new WaitUntil(() => difference.sqrMagnitude < .2); // This is difference.sqrMag, not dir because dif is changed every frame while going to land
        animator.Play("Bird_Idle");
        moving = false;
        //perched = false;
        yield return new WaitForSeconds(2);
        canMoveOn = true;
    }
}