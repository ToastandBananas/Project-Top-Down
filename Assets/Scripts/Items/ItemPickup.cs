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
        itemDropScript = GetComponentInParent<ItemDrop>();
        weaponDamageScript = GetComponentInParent<WeaponDamage>();
        boxCollider = GetComponentInParent<BoxCollider2D>();
        itemData = GetComponentInParent<ItemData>();
        
        // For Highlighting
        sr = GetComponentInParent<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    void Start()
    {
        item = itemData.item;
        
        pickUpRadiusCollider = GetComponent<CircleCollider2D>();
        pickUpRadiusCollider.radius = radius;

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
            Destroy(transform.parent.gameObject); // Then we'll destroy the actual gameobject, since it will only exist in our inventory system until we use/equip it
        else
            Debug.Log("Not enough room in your inventory to pick up " + item.name + ".");
    }
}
