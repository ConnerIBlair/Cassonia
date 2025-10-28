using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{ // Name of video: Inventory System | Unity Tutorial  Name of Channel: Pogle 
    public InventoryItem myItem ;//{ get; set; } // This is instantiated as child of Inventory Slot. Contains a scriptable object that is stored in project folder

    public SlotTag myTag;

    public Sprite clearImage;

    [SerializeField]
    private Image itemUISprite;

    [SerializeField]
    private AudioClip hoverEffect;

    [SerializeField]
    private AudioClip clickEffect;

    [SerializeField]
    private AudioClip equipEffect;

    private PlayerMovement player;

    private void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (InventoryManager.carriedItem == null)
            //player.sounds.PlayEffect(hoverEffect);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            player.sounds.PlayEffect(clickEffect);
            if (InventoryManager.carriedItem == null)
            {
                itemUISprite.sprite = clearImage;
                return;
            }// Checks if we're carrying an item
            if (myTag != SlotTag.None && InventoryManager.carriedItem.myItem.itemTag != myTag) return; // Makes sure a non-equipable doesn't go in a non-equipable slot... Only checks if we're holding something that isn't SlotTag.None
            
            SetItem(InventoryManager.carriedItem); // then checks something... mentioned armor in the video
        }
    }

    public void SetItem(InventoryItem item)
    {
        InventoryManager.carriedItem = null;

        //item.activeSlot.myItem = null; // reset old slot  // When this wasn't commented out, replacing an equiped item with another item made it not work

        myItem = item; // Set slot's item
        myItem.activeSlot = this; // lets item know that this inventory slot is its slot
        myItem.transform.SetParent(transform); // set transform
        myItem.transform.position = transform.position;                      /////////////////////Changed this line for positioning
        myItem.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);/////////////////////Added this line for scaling
        myItem.canvasGroup.blocksRaycasts = true; // makes sure that it's clickable again

        if (myTag == SlotTag.Item) // If placed in equip item slot
        {
            if (item.activeSlot.myItem == null) // remove item from slot
            {
                itemUISprite.sprite = clearImage;
                //print("Remove Image");
            }
            else // place item in slot
            {
                itemUISprite.sprite = myItem.myItem.sprite;
               // print("Place Image");
            }
        }

        if (myTag != SlotTag.None) // If put into equipment slot, equip it
        {
            InventoryManager.Singleton.EquipEquipment(myTag, myItem);
            player.sounds.PlayEffect(equipEffect);
        }
    }
}
