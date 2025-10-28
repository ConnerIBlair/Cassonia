using UnityEngine;
using System.Collections;

public class Windseed : Seed
{
    [SerializeField]
    private Knockback knockback;

    private void Awake()
    {
        knockback = GetComponentInChildren<Knockback>();
    }

    void Update()
    {
        if (boom)
        {
            boom = false;
            StartCoroutine(NewBoomCo());
        }
    }
    private IEnumerator NewBoomCo()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            string colliderTag = colliders[i].gameObject.tag;
            switch (colliderTag)
            {
                case "Player":
                        colliders[i].GetComponent<PlayerMovement>().StartCoroutine("WindJump");
                    break;
                case "Seed":
                    colliders[i].GetComponent<Seed>().Boom();
                    break;
                case "Enemy":
                    if (colliders[i].TryGetComponent<EnemyHurtbox>(out EnemyHurtbox hurtbox))
                    {
                        knockback.KnockbackObject(colliders[i].transform.parent, optForce: 10, stun: .5f);
                    }
                    break;
            }
        }
        yield return new WaitForSeconds(1.5f); // Animation time
        Destroy(gameObject);
    }
}
