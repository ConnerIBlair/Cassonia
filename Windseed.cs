using UnityEngine;

public class Windseed : Seed
{
    [SerializeField]
    private Knockback knockback;

    void Update()
    {
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
                case "Player":
                        colliders[i].GetComponent<PlayerMovement>().StartCoroutine("WindJump");
                    break;
                case "Seed":
                    colliders[i].GetComponent<Seed>().Boom();
                    break;
                case "Enemy":
                    knockback.KnockbackObject(colliders[i].transform);
                    break;
            }
        }
        Destroy(gameObject);
    }
}
