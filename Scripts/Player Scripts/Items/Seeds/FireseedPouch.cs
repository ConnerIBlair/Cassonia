using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireseedPouch : MonoBehaviour, IItem
{
    [SerializeField]
    private AudioClip boomSound;

    public GameObject Fireseed; // Prefab
    private GameObject FireseedClone;

    public Vector2[] Positions1;

    [SerializeField] int fireseedCount;

    private Fireseed[] fireScripts;
    public float coolDown;
    private bool canThrow = true;

    [SerializeField]
    private Sprite[] sprites;

    public void Use(PlayerMovement player)
    {
        if (!canThrow) return;

        fireScripts = new Fireseed[fireseedCount];
        player.gameObject.GetComponent<PlayerAnimator>().currentAction = PlayerAction.Action;
        player.gameObject.GetComponent<PlayerAnimator>().MakePlayerMove();
        player.currentState = PlayerState.attack;
        StartCoroutine(ThrowCo(player));
    }

    private IEnumerator ThrowCo(PlayerMovement player)
    {
        canThrow = false;
        for (int i = 0; i < fireseedCount; i++)
        {
            FireseedClone = Instantiate(Fireseed); //, player.transform

            FireseedClone.transform.position = player.transform.position;

            fireScripts[i] = FireseedClone.GetComponent<Fireseed>();
            fireScripts[i].player = player;
            fireScripts[i].sprite = sprites[Random.Range(0, sprites.Length)];
            fireScripts[i].landing = Positions1[i];
            
        }
        fireScripts[0].boomSound = boomSound;
        yield return new WaitForSeconds(.416f);
        player.currentState = PlayerState.walk;

        yield return new WaitForSeconds(coolDown - .416f);
        canThrow = true;
    }
}
