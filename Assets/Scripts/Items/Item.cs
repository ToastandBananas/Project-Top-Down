using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public bool isPickupable = true;

    [Header("Basic Info")]
    new public string name = "New Item";
    public Sprite sprite = null;
    public string description;
    public int value = 1;

    [Header("Inventory Icon Info")]
    public int iconWidth = 1;
    public int iconHeight = 1;
    public Sprite inventoryIcon = null;

    [Header("Stackability")]
    public bool isStackable;
    public int currentStackSize = 1;
    public int maxStackSize = 1;

    public virtual void Use()
    {
        // Use the item
        Debug.Log("Using " + name);
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
