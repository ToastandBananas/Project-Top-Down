using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
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
        Debug.Log("Picking up " + item.name);
        bool wasPickedUp = Inventory.instance.AddToPockets(item, itemData);
        if (wasPickedUp == false)
            wasPickedUp = Inventory.instance.AddToBag(item, itemData);

        if (wasPickedUp)
            Destroy(gameObject);
        else
            Debug.Log("Not enough room in your inventory to pick up " + item.name + ".");
    }
}
