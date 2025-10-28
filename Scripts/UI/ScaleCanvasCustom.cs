using UnityEngine;
using UnityEngine.UI;

public class ScaleCanvasCustom : MonoBehaviour
{
    public CanvasScaler CanvasScaler;

    private void Update() // Make this happen only when the screen size is changing instead of update
    {
        if (Screen.width < 800 || Screen.height < 448)
        {
            CanvasScaler.scaleFactor = 1;
        } else if (Screen.width < 1200 || Screen.height < 672)
        {
            CanvasScaler.scaleFactor = 2;
        }
        else if (Screen.width < 1600 || Screen.height < 896)
        {
            CanvasScaler.scaleFactor = 3;
        }
        else if (Screen.width < 2000 || Screen.height < 1120)
        {
            CanvasScaler.scaleFactor = 4;
        }
        else if (Screen.width < 2400 || Screen.height < 1344)
        {
            CanvasScaler.scaleFactor = 5;
        }
        else if (Screen.width < 2800 || Screen.height < 1568)
        {
            CanvasScaler.scaleFactor = 6;
        }
        else if (Screen.width < 3200 || Screen.height < 1792)
        {
            CanvasScaler.scaleFactor = 7;
        }
        else if (Screen.width < 3600 || Screen.height < 2016)
        {
            CanvasScaler.scaleFactor = 8;
        }
        else if (Screen.width < 4000 || Screen.height < 2240)
        {
            CanvasScaler.scaleFactor = 9;
        }
        else if (Screen.width < 4400 || Screen.height < 2464)
        {
            CanvasScaler.scaleFactor = 10;
        }
        else if (Screen.width < 4800 || Screen.height < 2688)
        {
            CanvasScaler.scaleFactor = 11;
        }
    }
}
