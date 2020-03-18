using UnityEngine;

public enum ItemType { General, Consumable, Weapon, Shield, Armor, Quiver, Ammunition }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public bool isPickupable = true;
    public GameObject prefab;

    [Header("In Game Sprites")]
    public Sprite sprite;
    [Tooltip("Only used when the default sprite will differ from the dropped sprite.")]
    public Sprite droppedSprite;

    [Header("Basic Info")]
    new public string name = "New Item";
    public string description;
    public float weight = 1f;

    [Header("Enums")]
    public ItemType itemType = ItemType.General;
    [Tooltip("Determines spawn rate.")]
    public Rarity baseRarity = Rarity.Common;

    [Header("Value")]
    public int minBaseValue = 1;
    public int maxBaseValue = 10;
    [Tooltip("Only used when the value will not be calculated.")]
    public int staticValue = 1;

    [Header("Inventory Icon Info")]
    public int iconWidth = 1;
    public int iconHeight = 1;
    public Sprite inventoryIcon;

    [Header("Stackability")]
    public bool isStackable;
    public int maxStackSize = 1;

    [Header("Use Item Text")]
    public string onUseItemText;
    public string onFailedToUseItemText = "I can't do that";

    public virtual void Use(ItemData itemData, EquipmentManager equipmentManager = null, InventorySlot invSlot = null)
    {
        // Use or equip the item
        // Debug.Log("Using " + name);
    }

    public void RemoveFromInventory(ItemData itemData)
    {
        Inventory.instance.RemoveItem(itemData);
    }
}
