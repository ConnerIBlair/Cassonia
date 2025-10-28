using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : AResetSave
{
    [SerializeField]
    private int puzzleID;

    [SerializeField]
    private int tilemapDeactivate;

    [SerializeField]
    private int tilemapActivate;

    private MultiSceneFunctions player;

    [SerializeField]
    private bool inScene;

    [Header("Nothing if not in scene")]

    [SerializeField]
    private TilePuzzles tilePuzzle;

    private bool activated;

    [Header("Reset removes saved state")]
    public bool Reset;

    [SerializeField]
    private Sprite activatedSprite;

    private void Start()
    {
        if (Reset) ResetSave();

        activated = (PlayerPrefs.GetInt("PressurePlate_" + puzzleID) != 0);
        if (!activated) return;

        GetComponent<SpriteRenderer>().sprite = activatedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            if (inScene)
            {
                tilePuzzle.Activate(tilemapActivate);
                tilePuzzle.Deactivate(tilemapDeactivate);
            }
            else
            {
                player = other.GetComponentInChildren<MultiSceneFunctions>();
                player.puzzleIDs.Add(puzzleID);
                player.tilemapDeactivate.Add(tilemapDeactivate);
                player.tilemapActivate.Add(tilemapActivate);
                player.StartActivation();
            }
            activated = true;
            PlayerPrefs.SetInt("PressurePlate_" + puzzleID, (activated ? 1 : 0));
            GetComponent<SpriteRenderer>().sprite = activatedSprite;
        }
    }

    public override void ResetSave()
    {
        PlayerPrefs.SetInt("PressurePlate_" + puzzleID, 0);
    }
}
