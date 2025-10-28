using UnityEngine;
using UnityEngine.UIElements;

public class ChangeSortingOrder : MonoBehaviour
{
    private int originalLayer = -1;
    public void ChangeOrder(SpriteRenderer sRenderer)
    {

        if (originalLayer < 0) // hasn't been set to the right layer yet
        {
            originalLayer = sRenderer.gameObject.layer;
        }

        if (GetComponent<BridgeInteraction>().onBridge)
        {
            if (sRenderer.sortingOrder < 11)
            {
                sRenderer.sortingOrder += 11;
                gameObject.layer = LayerMask.NameToLayer("AboveBridge");
            }
        }
        else
        {
            if (sRenderer.sortingOrder >= 11)
            {
                sRenderer.sortingOrder -= 11;
                sRenderer.gameObject.layer = originalLayer;
            }
        }
    }

    public void ChangeLayerOnly(GameObject obj, bool inclChildren = false)
    {
        if (originalLayer < 0) // hasn't been set to the right layer yet
        {
            originalLayer = obj.layer;
        }

        if (GetComponent<BridgeInteraction>().onBridge)
        {
            obj.layer = LayerMask.NameToLayer("AboveBridge");
            if (inclChildren)
            {
                foreach (Transform cObj in GetComponentsInChildren<Transform>())
                {
                    obj = cObj.gameObject;
                    obj.layer = LayerMask.NameToLayer("AboveBridge");
                }
            }
        }
        else
        {
            obj.layer = originalLayer;
            if (inclChildren)
            {
                foreach (Transform cObj in GetComponentsInChildren<Transform>())
                {
                    obj = cObj.gameObject;
                    obj.layer = originalLayer; // Assumes that the child object's layer is the same as the immediate parent obj
                }
            }
        }
    }
}
