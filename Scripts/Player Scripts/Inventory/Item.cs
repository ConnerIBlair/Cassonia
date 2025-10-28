using UnityEngine;

public enum SlotTag { None, Item, Helmet, Chestplate, Legs, Coin, Stackable}

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{

    public Sprite sprite;
    public SlotTag itemTag;

    [Header("If the item can be eqipped")]
    public GameObject equipmentPrefab;

    public string codeName;

    public string itemName;
    public string description;

    public bool linkable = false;
    public bool linkee = false;
    // Need some reference to the item functionality

    public int count = 1;
}
