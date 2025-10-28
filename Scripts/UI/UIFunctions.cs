using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{ // Saving video is in watch later
    public RawImage image;
    public GameObject Menu;
    public GameObject DeathMenu;

    [Header("Only if attached to the Player")]
    [SerializeField]
    private GameObject player;
    private InventoryManager inventoryManager;

    private new Camera camera;

    private AsyncOperation asyncOperation;

    public bool currentlyFading;

    private InventoryItem[] inventoryItems;
    [HideInInspector]
    public Vector3 playerSpawn;

    private void Awake()
    {
        if (player != null)
        {
            inventoryManager = player.GetComponent<InventoryManager>();
        }

        camera = FindFirstObjectByType<Camera>();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadScene(int sceneToLoad, int start = 0, int end = 1)
    {
        StartCoroutine(FadeTransitionCo(sceneToLoad, start, end));
    }

    public void TitleScreen(int scene)
    {
        StartCoroutine(FadeTransitionCo(scene, 0, 1));
    }

    public void Fade_LoadScene(int scene = 0, int start = 0, int end = 1)
    {
        StartCoroutine(FadeTransitionCo(scene, start, end));
    }

    private IEnumerator FadeTransitionCo(int sceneToLoad = -1, int start = 0, int end = 1)
    {
        if (currentlyFading) yield break;

        currentlyFading = true;

        if (sceneToLoad != -1)
        {
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
            asyncOperation.allowSceneActivation = false;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 2)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(start, end, t));
            image.color = newColor;

            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        currentlyFading = false;

        if (sceneToLoad != -1) asyncOperation.allowSceneActivation = true;
            
        yield return new WaitForSeconds(.5f);
        if (player != null)
        {
            //player.SetActive(false);
        }
        image.color = new Color(0, 0, 0, 0);
        Menu.SetActive(false);
    }

    public void SaveAll()
    {
        //SaveInventory();

        SaveSpawn();

        // Save the scene
    }

    public void SaveSpawn()
    {
        PlayerPrefs.SetFloat("x" , playerSpawn.x);
        PlayerPrefs.SetFloat("y", playerSpawn.y);// Spawn location is set by playerMovement
        PlayerPrefs.SetFloat("z", playerSpawn.z);
    }
    public void SaveInventory()
    {
        for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            inventoryItems[i] = inventoryManager.inventorySlots[i].myItem;
            // Save the item
        }
    }

    public void LoadAll()
    {
        /*for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            // Load items and inventoryItems[i] to equal item
            inventoryManager.inventorySlots[i].SetItem(inventoryItems[i]);
        }

        player.transform.position = new Vector3(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"), PlayerPrefs.GetFloat("z"));
        camera.transform.position = new Vector3(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"), -10);
        */
        // Change the camera position to match the player
    }

    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo()
    {
        yield return new WaitForSeconds(5);
        Destroy(player);
    }
}
