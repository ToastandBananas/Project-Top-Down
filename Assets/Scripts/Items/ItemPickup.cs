using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;

    ItemDrop itemDropScript;

    private void Awake()
    {
        itemDropScript = GetComponent<ItemDrop>();
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
        bool wasPickedUp = Inventory.instance.AddToPockets(item);
        if (wasPickedUp == false)
            wasPickedUp = Inventory.instance.AddToBag(item);

        if (wasPickedUp)
            Destroy(gameObject);
    }
}
