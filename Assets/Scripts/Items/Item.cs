using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public bool isDefaultItem = false;

    new public string name = "New Item";
    public Sprite icon = null;
    public string description;
    public int value = 1;
    public bool isStackable;

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
