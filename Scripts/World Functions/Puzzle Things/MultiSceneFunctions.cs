using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Attached to the player. When a switch is activated, its number is transfered over to thingsToActivate so when the scene changes,
// this script stays activae and activates whatever object was supposed to change through the use of IPuzzles script
public class MultiSceneFunctions : MonoBehaviour
{
    public List<int> puzzleIDs = new List<int>(); // This will store numbers, doesn't matter the order.
    // These numbers correspond to somthing in the game, if a number lines up and the object is currently active, it does something
    public List<int> tilemapDeactivate = new List<int>();
    public List<int> tilemapActivate = new List<int>();

    private Scene currentScene;

    public void StartActivation()
    {
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(WaitSceneCo());
    }

    private IEnumerator WaitSceneCo()
    {
        yield return new WaitUntil(() => currentScene != SceneManager.GetActiveScene());
        for (int i = 0; i < puzzleIDs.Count; i++)
        {
            GameObject.Find("/TilePuzzles/TilePuzzle(" + puzzleIDs[i] + ")").GetComponent<TilePuzzles>().Activate(tilemapActivate[i]);
            GameObject.Find("/TilePuzzles/TilePuzzle(" + puzzleIDs[i] + ")").GetComponent<TilePuzzles>().Deactivate(tilemapDeactivate[i]);
        }
    }
}