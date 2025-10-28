using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TemporaryPressurePlateActivator : MonoBehaviour
{
    public GameObject deactivateObject;

    public Scene activatedScene;


    public Tilemap tileMap;

    public Tile[] tiles;

    public BoundsInt position;



    private static TemporaryPressurePlateActivator instance = null;

    public static TemporaryPressurePlateActivator Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        activatedScene = SceneManager.GetActiveScene();
        DontDestroyOnLoad(this.gameObject);
    }


    public IEnumerator ActivateCo()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene() == activatedScene);

        tileMap.SetTilesBlock(position, tiles);

        deactivateObject.SetActive(false);

    }
}
