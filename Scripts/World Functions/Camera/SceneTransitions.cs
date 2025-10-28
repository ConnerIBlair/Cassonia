using System.Collections;
using TMPro;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
public class SceneTransitions : MonoBehaviour
{
    public string ID;
    [SerializeField]
    private int sceneToLoad;

    public PlayerDirection exitMovement;

    public AudioClip soundEffect;

    public CameraMovement cameraS { private set; get; }

    [Header("If the camera will be static in current scene")]
    [SerializeField]
    private Vector2 staticCameraPos = Vector2.zero;

    public Transform player;

    public bool hasDoor = false;
    public Sprite[] doorSprites;
    [SerializeField]
    private SpriteRenderer sRenderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            player.GetComponent<PlayerMovement>().TransitionScene(ID, sceneToLoad, other.GetComponentInChildren<PixelationTransition>(), soundEffect);
            if (hasDoor)
            {
                StartCoroutine(OpenDoorCo());
            }
        }
    }


    private IEnumerator OpenDoorCo()
    {
        foreach (Sprite sprite in doorSprites)
        {
            yield return new WaitForSeconds(0.05f);
            sRenderer.sprite = sprite;
        }
        sRenderer.sprite = null;
    }

    public void CameraPositionChange()
    {
        if (hasDoor)
            sRenderer.sprite = null;

        cameraS = FindFirstObjectByType<CameraMovement>();
        if (staticCameraPos != Vector2.zero) { cameraS.maxPosition = staticCameraPos; cameraS.stationary = true; cameraS.smoothing = 2; return; }

        cameraS.stationary = false;
        cameraS.parent.locked = false;
        for (var i = 0; i < cameraS.MinMaxBorders.Count; i++)
        {
            if (cameraS.MinMaxBorders[i].Item2.x > transform.position.x && cameraS.MinMaxBorders[i].Item1.x < transform.position.x)
            {
                if (cameraS.MinMaxBorders[i].Item2.y > transform.position.y && cameraS.MinMaxBorders[i].Item1.y < transform.position.y)
                {
                    cameraS.minPosition = cameraS.MinMaxBorders[i].Item1;
                    cameraS.maxPosition = cameraS.MinMaxBorders[i].Item2;
                    cameraS.parent.translationAxis = Axis.Z;
                    cameraS.transform.position = new Vector3(Mathf.Clamp(player.position.x, cameraS.minPosition.x + 12.5f, cameraS.maxPosition.x - 12.5f), Mathf.Clamp(player.position.y, cameraS.minPosition.y + 7, cameraS.maxPosition.y - 7), cameraS.transform.position.z);
                    break;
                }
            }
        }
    }
}