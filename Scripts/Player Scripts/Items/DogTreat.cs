using UnityEngine;
using System.Collections;

public class DogTreat : MonoBehaviour, IItem
{
    public Collider2D[] colliders;

    private bool delete;
    private PlayerAnimator pAnim;

    public void Use(PlayerMovement player)
    {
        pAnim = player.GetComponent<PlayerAnimator>();
        //pAnim.currentAction = PlayerAction.Use;
        //pAnim.MakePlayerMove();
        colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 6);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Dog>())
            {
                StartCoroutine(collider.GetComponent<Dog>().TreatCo());
                delete = true;
                break;
            }
        }
        StartCoroutine(WaitCo());
    }

    private IEnumerator WaitCo()
    {
        yield return new WaitForSeconds(.5f); // Animation time
        pAnim.currentAction = PlayerAction.Idle;
        pAnim.MakePlayerMove();
        if (delete)
        {
            // Remove from inventory
            Destroy(gameObject);
        }
    }
}
