using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenChanger : MonoBehaviour
{
    public int sceneToLoad;
    public int timeToFade;
    public RawImage image;

    public float fadeInTime;

    public VolumeScript volumeS;


    public Color fadeColour;

    public AsyncOperation asyncOperation;
    public bool canStart = true;

    private bool starting = false;

    private void Start()
    {
        //_asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad); // Begin to load the scene you want to
        //_asyncOperation.allowSceneActivation = false; // Prevent it from loading
        canStart = false;
        image.gameObject.SetActive(true);
        StartCoroutine(FadeTransitionCo(1, 0, timeToFade));
        //image.color = new Color(1.0f, 1.0f, 1.0f, 0);
    }


    public void StartGame(AudioClip song)
    {
        if (starting) return;
        starting = true;
        StartCoroutine(StartGameCo(song));
    }

    private IEnumerator StartGameCo(AudioClip song)
    {

        yield return new WaitUntil(() => canStart == true);

        _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad); // Begin to load the scene you want to
        _asyncOperation.allowSceneActivation = false; // Prevent it from loading

        volumeS = FindFirstObjectByType<VolumeScript>();
        image.gameObject.SetActive(true);
        volumeS.PlaySong(song, false, true);
        StartCoroutine(FadeTransitionCo(image.color.a, 1, fadeInTime));
    }

    private IEnumerator FadeTransitionCo(float start, float end, float fadeTime)
    {

        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            //change color as you want
            var newColor = new Color(0, 0, 0, Mathf.Lerp(start, end, t));
            image.color = newColor;

            yield return null;
        }
        if (end != 0)
        {
            this._asyncOperation.allowSceneActivation = true;
        }
        else
        {
            image.gameObject.SetActive(false);
        }
        canStart = true;
    }

    [SerializeField] private string _sceneName = "maingame";
    public string _SceneName => this._sceneName;

    private AsyncOperation _asyncOperation;

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
    {
        // Begin to load the Scene you have specified.
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Don't let the Scene activate until you allow it to.
        this._asyncOperation.allowSceneActivation = false;

        while (!this._asyncOperation.isDone)
        {
            Debug.Log($"[scene]:{sceneName} [load progress]: {this._asyncOperation.progress}");

            yield return null;
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return) && this._asyncOperation == null)
    //    {
    //        Debug.Log("Started Scene Preloading");

    //        // Start scene preloading.
    //        this.StartCoroutine(this.LoadSceneAsyncProcess(sceneName: this._sceneName));
    //    }

    //    // Press the space key to activate the Scene.
    //    if (Input.GetKeyDown(KeyCode.Space) && this._asyncOperation != null)
    //    {
    //        Debug.Log("Allowed Scene Activation");

    //        this._asyncOperation.allowSceneActivation = true;
    //    }
    //}
}
