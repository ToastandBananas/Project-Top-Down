using UnityEngine;

public enum ItemType { General, Consumable, Weapon, Shield, Armor, Quiver, Ammunition }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public bool isPickupable = true;

    [Header("Basic Info")]
    new public string name = "New Item";
    public Sprite sprite = null;
    public string description;
    public float weight = 1f;
    public int baseValue = 1;
    public ItemType itemType = ItemType.General;
    public Rarity baseRarity = Rarity.Common;

    [Header("Inventory Icon Info")]
    public int iconWidth = 1;
    public int iconHeight = 1;
    public Sprite inventoryIcon = null;

    [Header("Stackability")]
    public bool isStackable;
    public int maxStackSize = 1;

    public virtual void Use(ItemData itemData)
    {
        // Use or equip the item
        Debug.Log("Using " + name);
    }

    public void RemoveFromInventory(ItemData itemData)
    {
        Inventory.instance.Remove(itemData);
    }
}
