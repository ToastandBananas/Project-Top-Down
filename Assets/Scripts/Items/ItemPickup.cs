using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    ItemData itemData;

    AudioManager audioManager;
    ItemDrop itemDropScript;
    WeaponDamage weaponDamageScript;
    BoxCollider2D boxCollider;
    [HideInInspector] public CircleCollider2D pickUpRadiusCollider;

    void Awake()
    {
        audioManager = AudioManager.instance;
        itemDropScript = GetComponent<ItemDrop>();
        weaponDamageScript = GetComponent<WeaponDamage>();
        boxCollider = GetComponent<BoxCollider2D>();
        itemData = GetComponent<ItemData>();
    }

    public override void Start()
    {
        base.Start();

        item = itemData.item;
        
        pickUpRadiusCollider = GetComponent<CircleCollider2D>();
        pickUpRadiusCollider.radius = interactRadius;

        if (itemDropScript.isDropped == false)
        {
            pickUpRadiusCollider.enabled = false;
            enabled = false;
        }
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
        if (itemDropScript.isDropped && playerInRange)
        {
            base.Interact();
            PickUp();
        }
    }

    void PickUp()
    {
        Debug.Log(gm.currentlySelectedInteractable);
        bool wasPickedUp = Inventory.instance.AddToInventory(item, itemData); // If this returns true, it will add the item to the appropriate bag

        if (wasPickedUp || itemData.currentStackSize <= 0)
        {
            gm.currentlySelectedInteractable = null;
            Destroy(gameObject); // Then we'll destroy the actual gameobject, since it will only exist in our inventory system until we use/equip it
        }
        else
            Debug.Log("Not enough room in your inventory to pick up " + item.name + ".");
    }
}
