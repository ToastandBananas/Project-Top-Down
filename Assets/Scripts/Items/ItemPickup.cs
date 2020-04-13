using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    ItemData itemData;

    AudioManager audioManager;
    ItemDrop itemDropScript;
    WeaponDamage weaponDamageScript;
    BoxCollider2D boxCollider;

    void Awake()
    {
        audioManager = AudioManager.instance;
        itemDropScript = GetComponent<ItemDrop>();
        weaponDamageScript = GetComponent<WeaponDamage>();
        boxCollider = GetComponent<BoxCollider2D>();
        itemData = GetComponent<ItemData>();
    }

    void Start()
    {
        item = itemData.item;

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
        bool wasPickedUp = Inventory.instance.AddToInventory(item, itemData); // If this returns true, it will add the item to the appropriate bag

        if (wasPickedUp || itemData.currentStackSize <= 0)
            Destroy(gameObject); // Then we'll destroy the actual gameobject, since it will only exist in our inventory system until we use/equip it
        else
            Debug.Log("Not enough room in your inventory to pick up " + item.name + ".");
    }
}
