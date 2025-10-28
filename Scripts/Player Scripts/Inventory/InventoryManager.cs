using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Singleton;
    public static InventoryItem carriedItem;

    public InventorySlot[] inventorySlots;

    [SerializeField] Transform draggablesTransform; // Empty transform that holds transform of items being dragged
    [SerializeField] InventoryItem itemPrefab;

    private PlayerMovement player = PlayerMovement.Instance;

    private void Awake()
    {
        Singleton = this;
    }

    private Item InInventory(Item item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem != null)
            {
                if (slot.myItem.myItem.itemName == item.itemName)
                {
                    return slot.myItem.myItem;
                }
            }
        }
        return null;
    }

    public bool AddItem(Item item)
    {
        Item _item = item;

        if (_item.itemTag == SlotTag.Coin && Int32.TryParse(_item.codeName, out int j)) // make string to int
        { 
            player.CoinCount(j);
            return true;
        }
        if (_item.itemTag == SlotTag.Stackable && InInventory(_item) != null) // already in inventory?
        {
            InInventory(_item).count++;
            Debug.Log(_item.count);
            Debug.Log(_item);
            Debug.Log(_item.name);
            return true;
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null && inventorySlots[i].myTag == SlotTag.None) // Makes sure the spot being checked is empty
            { // and is able to hold anything
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]); // Spawns item in that spot
                return true;
                // Saving
            }
            if (i == inventorySlots.Length)
            {
                return false;
            }
        }
        return false;
    }

    private void Update()
    { // Sets item pos to cursor if picked up
        if (carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item) // called when clicking on an item
    {
        if (carriedItem != null) // Checks if we're already holding an item and switches positions with it
        {
            if (item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }
        else
        {
            //playerScript.sounds.PlayEffect(playerScript.soundEffects[0], .5f);
        }

        if (item.activeSlot.myTag != SlotTag.None) // checks if Item is in an item slot and unequips it
        {
            EquipEquipment(item.activeSlot.myTag, null);
        }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Item:
                if (item == null)
                {
                    // Destroy item.equipmentPrefab on the Player
                    //Debug.Log("Unequiped Item on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the player
                    //Debug.Log("Equiped " + item.myItem.name + " on " + tag);
                }
                break;

            case SlotTag.Helmet:
                if (item == null)
                {
                    // Destroy item.equipmentPrefab on the Player
                    Debug.Log("Unequiped Item on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the player
                    Debug.Log("Equiped " + item.myItem.name + " on " + tag);
                }
                break;
        }
    }
}
