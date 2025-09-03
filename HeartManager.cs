using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite fullHeartBeat;
    public Sprite halfFullHeart;
    public Sprite halfFullHeartBeat;
    public Sprite emptyHeart;
    public FloatValue heartContainers;
    public FloatValue playerCurrentHealth;

    public float time;
    public float secondTime;
    private float timeTilBeat = .5f;

    private int currentHeart;

    public bool beating;

    private void Start()
    {
        InitHearts();
    }

    public void InitHearts()
    {
        for (int i = 0; i < heartContainers.initialValue; i++)
        {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHeart;
            if (i + 1 == heartContainers.RuntimeValue)
            {
                beating = true;
                currentHeart = i;
                //BeatingHeart(i);
            }
        }
    }
    public void UpdateHearts()
    {
        beating = false;
        float tempHealth = playerCurrentHealth.RuntimeValue / 2; // / by 2 because half a heart is one health point

        for (int i = 0; i < heartContainers.RuntimeValue; i++)
        {
            if (i <= tempHealth - 1)
            {
                //full
                hearts[i].sprite = fullHeart;
            }
            else if (i >= tempHealth)
            {
                hearts[i].sprite = emptyHeart;
            }
            else
            {
                // half
                hearts[i].sprite = halfFullHeart;
            }

            if (hearts[i].sprite == emptyHeart)
            {
                beating = true;
                //BeatingHeart(i - 1);
                currentHeart = i - 1;
                return;
            }else if (i + 1 == heartContainers.RuntimeValue)
            {
                beating = true;
                //BeatingHeart(i);
                currentHeart = i;
            }
        }
    }

    private void Update()
    {
        if (playerCurrentHealth.RuntimeValue <= 0)
        {
            return;
        }

        if (beating)
        {
            if (time < timeTilBeat)
            {
                time += Time.deltaTime;
                return;
            }
            else
            {
                if (hearts[currentHeart].sprite == fullHeart)
                {
                    hearts[currentHeart].sprite = fullHeartBeat;
                }
                if (hearts[currentHeart].sprite == halfFullHeart)
                {
                    hearts[currentHeart].sprite = halfFullHeartBeat;
                }

                if (secondTime < timeTilBeat)
                {
                    secondTime += Time.deltaTime;
                    return;
                }
                else
                {
                    if (hearts[currentHeart].sprite == fullHeartBeat)
                    {
                        hearts[currentHeart].sprite = fullHeart;
                    }
                    else
                    {
                        hearts[currentHeart].sprite = halfFullHeart;
                    }
                }
                time = 0;
                secondTime = 0;
            }
        }
    }

    //private IEnumerator BeatingHeart(int heart)
    //{
    //    if (beating)
    //    {
    //        if (hearts[heart].sprite == fullHeart)
    //        {
    //            yield return new WaitForSeconds(.25f);
    //            hearts[heart].sprite = fullHeartBeat;
    //            yield return new WaitForSeconds(.25f);
    //            hearts[heart].sprite = fullHeart;
    //        }
    //        if (hearts[heart].sprite == halfFullHeart)
    //        {
    //            yield return new WaitForSeconds(.25f);
    //            hearts[heart].sprite = halfFullHeartBeat;
    //            yield return new WaitForSeconds(.25f);
    //            hearts[heart].sprite = halfFullHeart;
    //        }
    //    }
    //}
}