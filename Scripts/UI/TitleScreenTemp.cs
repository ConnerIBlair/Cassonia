using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenTemp : MonoBehaviour
{
    public Texture2D[] images;

    public RawImage DisplayImage;

    public float time;

    void Start()
    {
        StartCoroutine(ChangeImageCo());
    }

    private IEnumerator ChangeImageCo()
    {
        for (int i = 0; i < 3; i++)
        {
            DisplayImage.texture = images[i];
            yield return new WaitForSeconds(time);
        }
        DisplayImage.texture = images[1];
        yield return new WaitForSeconds(time);
        StartCoroutine(ChangeImageCo());
    }
}
