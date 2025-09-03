using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeedPouch : Linkable, IItem
{
    [SerializeField]
    private AudioClip boomSound;

    public GameObject seed; // Prefab
    private GameObject seedClone;

    public Vector2[] Positions1;

    [SerializeField] int seedCount;

    private Seed[] seedScripts;
    public float coolDown;
    private bool canThrow = true;

    [SerializeField]
    private Sprite[] sprites;

    public ParticleSystem ps;

    public void Use(PlayerMovement player)
    {
        if (!canThrow) return;

        seedScripts = new Seed[seedCount];
        player.gameObject.GetComponent<PlayerAnimator>().currentAction = PlayerAction.Action;
        player.gameObject.GetComponent<PlayerAnimator>().MakePlayerMove();
        player.currentState = PlayerState.attack;
        StartCoroutine(ThrowCo(player));
    }

    private IEnumerator ThrowCo(PlayerMovement player)
    {
        canThrow = false;
        for (int i = 0; i < seedCount; i++)
        {
            seedClone = Instantiate(seed); //, player.transform

            seedClone.transform.position = player.transform.position;

            seedScripts[i] = seedClone.GetComponent<Seed>();
            seedScripts[i].player = player;
            seedScripts[i].sprite = sprites[Random.Range(0, sprites.Length)];
            seedScripts[i].landing = Positions1[i];
            seedScripts[i].ps = ps;
        }
        seedScripts[0].boomSound = boomSound;
        yield return new WaitForSeconds(.416f);
        player.currentState = PlayerState.walk;

        yield return new WaitForSeconds(coolDown - .416f);
        canThrow = true;
    }
}
