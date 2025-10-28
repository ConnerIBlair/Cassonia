using UnityEngine;
using UnityEngine.UI;
public class InGameTooltip : MonoBehaviour
{
    public GameObject tooltip;

    public void Activate()
    {
        tooltip.SetActive(true);
    }
    public void DeActivate()
    {
        tooltip.SetActive(false);
    }
}
