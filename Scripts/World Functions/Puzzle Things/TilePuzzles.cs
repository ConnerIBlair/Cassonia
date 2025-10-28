using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// This script is to change tiles when activated by a switch or world event
public class TilePuzzles : AResetSave, IPuzzles
{
    public int puzzleID;

    [SerializeField]
    private Tile[] tiles;

    [Header("Put the floored values for position")]
    [SerializeField]
    private Vector3Int position;

    [SerializeField]
    private int areaX = 2;
    [SerializeField]
    private int areaY = 2;

    private int tileIteration;

    private Tilemap[] tilemaps;

    private int tilemapA = 10;
    private int tilemapD;

    public bool newPuzzle;
    public bool Reset;

    private void Start()
    {
        areaX -= 1;
        areaY -= 1;

        if (newPuzzle)
        {
            PlayerPrefs.SetInt("TilePuzzlesA_" + puzzleID, 10);
            PlayerPrefs.SetInt("TilePuzzlesD_" + puzzleID, 0);
            
        }
        if (Reset)
        {
            ResetSave();
        }

        if (PlayerPrefs.GetInt("TilePuzzlesA_" + puzzleID) != 10)
        {
            tilemapA = PlayerPrefs.GetInt("TilePuzzlesA_" + puzzleID);
            tilemapD = PlayerPrefs.GetInt("TilePuzzlesD_" + puzzleID);
            Activate(tilemapA);
            Deactivate(tilemapD);
        }
    }

    public void Activate(int tilemap)
    {
        tilemapA = tilemap;
        PlayerPrefs.SetInt("TilePuzzlesA_" + puzzleID, tilemapA);
        tilemaps = GameObject.Find("Tilemaps").GetComponentsInChildren<Tilemap>();
        for (int i = areaY; i >= 0; i--) // y
        {
            for (int j = 0; j <= areaX; j++)// x
            {
                Vector3Int areaPos = new Vector3Int(position.x + j, position.y + i);
                tilemaps[tilemap].SetTile(areaPos, tiles[tileIteration]);
                tileIteration += 1;
            }
        }
    }

    public void Deactivate(int tilemap)
    {
        if (tilemap < 0) { return; }
        tilemapD = tilemap;
        PlayerPrefs.SetInt("TilePuzzlesD_" + puzzleID, tilemapD);
        for (int i = 0; i <= areaX; i++) // x
        {
            for (int j = 0; j <= areaY; j++)// y
            {
                Vector3Int areaPos = new Vector3Int(position.x + i, position.y + j);
                tilemaps[tilemap].SetTile(areaPos, null);
            }
        }
    }

    public override void ResetSave()
    {
        PlayerPrefs.SetInt("TilePuzzlesA_" + puzzleID, 10);
        PlayerPrefs.SetInt("TilePuzzlesD_" + puzzleID, 0);
    }
}
