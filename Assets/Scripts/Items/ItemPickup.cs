using UnityEngine;

public class ItemPickup : Interactable
{
    [HideInInspector] public Item item;
    ItemData itemData;
    
    ItemDrop itemDropScript;
    WeaponDamage weaponDamageScript;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        itemDropScript = GetComponent<ItemDrop>();
        weaponDamageScript = GetComponent<WeaponDamage>();
        boxCollider = GetComponent<BoxCollider2D>();
        item = itemDropScript.item;
        itemData = GetComponent<ItemData>();

        if (itemDropScript.isDropped == false)
            enabled = false;
        else
        {
            if (weaponDamageScript != null)
                weaponDamageScript.enabled = false;
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }

    public override void Interact()
    {
        if (itemDropScript.isDropped)
        {
            base.Interact();
            PickUp();
        }
    }

    void PickUp()
    {
        // Debug.Log("Picking up " + item.name);
        bool wasPickedUp = Inventory.instance.AddToPockets(item, itemData); // If this returns true, it will add the item to the appropriate bag
        if (wasPickedUp == false)
            wasPickedUp = Inventory.instance.AddToBag(item, itemData);

        if (wasPickedUp)
            Destroy(gameObject); // Then we'll destroy the actual gameobject, since it will only exist in our inventory system until we use/equip it
        else
            Debug.Log("Not enough room in your inventory to pick up " + item.name + ".");
    }
}
