using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneTransition : MonoBehaviour
{
    public int sceneToLoad;

    private CameraMovement cameraM;

    public int timeToFade;
    public RawImage image;

    public Vector3 nextScenePCords;
    public Vector3 camMaxCords;
    public Vector3 camMinCords;

    private void Start()
    {
        image = FindFirstObjectByType<UIFunctions>().image;
        cameraM = FindFirstObjectByType<CameraMovement>();
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
        if (end == 1)
        {
            cameraM.maxPosition = camMaxCords;
            cameraM.minPosition = camMinCords;
            cameraM.GetComponent<Camera>().backgroundColor = cameraM.colours.ColourSwatches[sceneToLoad];
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
            other.GetComponent<PlayerMovement>().currentState = PlayerState.interact;
            other.GetComponent<PlayerMovement>().TeleportCharacter(nextScenePCords, SceneManager.GetActiveScene());
        }
    }

    public void LoadScene()
    {
        StartCoroutine(FadeTransitionCo(0, 1, timeToFade));
    }
}
