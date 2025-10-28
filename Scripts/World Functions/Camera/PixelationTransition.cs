using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PixelationTransition : MonoBehaviour
{
    public RenderTexture texture;

    public GameObject pixelation;
    private RawImage overlay;

    [SerializeField]
    private GameObject thisCamera;
    public GameObject mainCamera;
    public float fadeTime;

    private void Start()
    {
        overlay = pixelation.GetComponent<RawImage>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            FadeIn();

        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        thisCamera.transform.position = mainCamera.transform.position;
        pixelation.SetActive(true);
        thisCamera.SetActive(true);
        //mainCamera.SetActive(false);
        StartCoroutine(FadeOutCo());
    }
    public void FadeIn()
    {
        thisCamera.transform.position = mainCamera.transform.position;
        overlay.CrossFadeColor(Color.white, fadeTime * 12, false, false);
        StartCoroutine(FadeInCo());
    }

    private IEnumerator FadeOutCo()
    {
        texture.Release();
        texture.width = 368;
        texture.height = 206;
        texture.Create();
        for (int i = texture.width; i > 48; i -= 16)
        {
            if (i == 208) { overlay.CrossFadeColor(Color.black, fadeTime * 11, false, false); }
            texture.Release();
            texture.width = i;
            texture.height -= 9;
            texture.Create();
            yield return new WaitForSeconds(fadeTime);
        }
        texture.Release();
        texture.width = 48;
        texture.height = 27;
        texture.Create();
    }
    private IEnumerator FadeInCo()
    {
        texture.Release();
        texture.width = 48;
        texture.height = 27;
        texture.Create();
        for (int i = texture.width; i < 400; i += 16)
        {
            texture.Release();
            texture.width = i;
            texture.height += 9;
            texture.Create();
            yield return new WaitForSeconds(fadeTime);
        }
        texture.Release();
        texture.width = 400;
        texture.height = 224;
        texture.Create();
        //ClearOutRenderTexture(texture);
        pixelation.SetActive(false);
        thisCamera.SetActive(false);
       // mainCamera.SetActive(true);
    }
    //public void ClearOutRenderTexture(RenderTexture renderTexture)
    //{
    //    RenderTexture rt = RenderTexture.active;
    //    RenderTexture.active = renderTexture;
    //    GL.Clear(true, true, Color.clear);
    //    RenderTexture.active = rt;
    //}
}
