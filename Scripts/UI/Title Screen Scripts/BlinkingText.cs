using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    //public float blinkTime;
    //public int showTime;

    //public float fadeTime;
    //public RawImage image;

    public Animator animator;

    //private bool blinking = true;

    //private void Start()
    //{
    //    image.color = new Color(1.0f, 1.0f, 1.0f, 1);
    //    StartCoroutine(BlinkingCo());
    //}

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            animator.SetBool("FadeOut", true);
            //blinking = false;
            //StartCoroutine(FadeTransitionCo(fadeTime, 0));

        }
    }



    //public IEnumerator BlinkingCo()
    //{
    //    while (blinking == true)
    //    {
    //        image.color = new Color(1.0f, 1.0f, 1.0f, 1);
    //        StartCoroutine(FadeTransitionCo(showTime / 2, 0));
    //        yield return new WaitForSeconds(showTime + 0.05f);
    //        image.color = new Color(1.0f, 1.0f, 1.0f, 0);
    //        StartCoroutine(FadeTransitionCo(blinkTime / 2, 1));
    //        yield return new WaitForSeconds(blinkTime);
    //    }
    //    image.color = new Color(1.0f, 1.0f, 1.0f, 1);
    //}



    //private IEnumerator FadeTransitionCo(float timeToFade, int upDown) // upDown | 1 = up, 0 = Down
    //{
    //    var alpha = image.color.a;
    //    for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / timeToFade)
    //    {
    //        //change color as you want
    //        var newColor = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(alpha, upDown, t)); // Goes from a to b based on what t is. 0 = a. 1 = b
    //        image.color = newColor;
    //        yield return null;
    //    }
    //}
}
