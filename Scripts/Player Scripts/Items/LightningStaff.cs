using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LightningStaff : MonoBehaviour, IItem
{
    public float speed;

    public int damage;
    public float reach;
    public int limit;
    private int shockedNumber;
    [SerializeField] private int shockedTime;

    private bool canCast = true;

    public int mana;
    public float coolDown;

    public GameObject prefab;
    private Vector3 lightningPos;

    [SerializeField]
    private float deathTime; // Make this the same as the player time when he can't move

    public List<bool> newShocked;

    public Collider2D[] colliders;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("Staff");
            CastSpell();
        }
    }

    public void Use(PlayerMovement player)
    {
        if (canCast)
            CastSpell();
    }

    public void CastSpell()
    {
        StartCoroutine(WaitTimeCo());
        lightningPos = transform.position;
       // newShocked = new List<bool>(limit);
        shockedNumber = 0;
        colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), reach);
        LightningStem();
    }

    private void LightningStem() 
    {
        StartCoroutine(SpellFunctionCo());
    }

    private float angle;

    private IEnumerator SpellFunctionCo()
    {
        //yield return null;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy") && limit > shockedNumber && // if it is an enemy and haven't reached the limit yet USED TO HAVE limit + 1
                colliders[i].GetComponent<Enemy>().isShocked == false) // if enemy has not been shocked already
            {
                Rotation(lightningPos, colliders[i].transform.position);
                LightningAnimator clone = Instantiate(prefab, lightningPos, Quaternion.identity).GetComponent<LightningAnimator>();
                clone.deathTime = deathTime;
                clone.transform.eulerAngles = new Vector3(0, 0, angle + 180);

                //if (lightningPos == transform.position)
                //{
                //    clone.transform.eulerAngles = new Vector3(0, 0, -angle -180);
                //}

                lightningPos = colliders[i].gameObject.transform.position;

                colliders[i].GetComponent<Enemy>().StartCoroutine("ShockedCo", shockedTime);
                //newShocked.Insert(shockedNumber, true);
                colliders[i].GetComponent<Enemy>().Health(damage);

                colliders = Physics2D.OverlapCircleAll(new Vector2(colliders[i].gameObject.transform.position.x,
                    colliders[i].gameObject.transform.position.y), reach);

                yield return new WaitForSeconds(0.5f / speed);
                shockedNumber += 1;

                StartCoroutine(SpellFunctionCo());
                break;
            }
        }
    } 

    private void Rotation(Vector3 startPos, Vector3 endPos)
    {
        float differenceX = startPos.x - endPos.x;
        float differenceY = startPos.y - endPos.y;
        float radians = Mathf.Atan2(differenceY, differenceX);
        angle = radians * 57.296f;
    }

    private IEnumerator WaitTimeCo()
    {
        canCast = false;
        yield return new WaitForSeconds(coolDown);
        canCast = true;
    }
}
