using UnityEngine;

public enum ItemType { General, Consumable, Weapon, Shield, Armor, Quiver, Ammunition }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public bool isPickupable = true;

    [Header("Basic Info")]
    new public string name = "New Item";
    public GameObject prefab;
    public Sprite sprite = null;
    public Sprite droppedSprite = null;
    public string description;
    public float weight = 1f;
    public int minBaseValue = 1;
    public int maxBaseValue = 10;
    public int staticValue = 1;
    public ItemType itemType = ItemType.General;
    public Rarity baseRarity = Rarity.Common;

    [Header("Inventory Icon Info")]
    public int iconWidth = 1;
    public int iconHeight = 1;
    public Sprite inventoryIcon = null;

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
