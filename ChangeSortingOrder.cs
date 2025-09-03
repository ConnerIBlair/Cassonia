using UnityEngine;

public class ChangeSortingOrder : MonoBehaviour
{
    private int originalLayer = -1;
    public void ChangeOrder(SpriteRenderer spriteRen)
    {

        if (originalLayer < 0)
        {
            originalLayer = spriteRen.gameObject.layer;
        }

        if (GetComponent<BridgeInteraction>().onBridge)
        {
            if (spriteRen.sortingOrder < 10)
            {
                spriteRen.sortingOrder += 10;
                gameObject.layer = LayerMask.NameToLayer("Bridge");
            }
        }
        else
        {
            if (spriteRen.sortingOrder >= 10)
            {
                spriteRen.sortingOrder -= 10;
                spriteRen.gameObject.layer = originalLayer;
            }
        }
    }
}
