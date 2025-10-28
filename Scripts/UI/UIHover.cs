using UnityEngine;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform UItransform;

    public float size;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UItransform.localScale = new Vector3(size, size, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UItransform.localScale = new Vector3(1, 1, 1);
    }
}
