using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool isEmpty = true;

    [Header("Slot Coordinate")]
    public Vector2 slotCoordinate = Vector2.zero;

    [Header("Prefabs")]
    public GameObject iconPrefab;

    [Header("Slot Background")]
    public Image slotBackgroundImage;
    public Sprite emptySlotSprite;
    public Sprite fullSlotSprite;

    [Header("Slot Item")]
    public SpriteRenderer iconSprite;
    public Item item;
    public ItemData itemData;
    public Text stackSizeText;
    public InventorySlot parentSlot;
    public InventorySlot[] childrenSlots = new InventorySlot[5]; // Item width * height - 1 will have a maximum of 5

    [HideInInspector]
    public Transform slotParent;
    Inventory inv;
    InventoryUI invUI;
    Vector3 mousePos;

    void Awake()
    {
        inv = Inventory.instance;
        invUI = InventoryUI.instance;
        stackSizeText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        // If we have a selected item & we're moving the item from this slot & we have an icon sprite
        if (invUI.currentlySelectedItem != null && invUI.movingFromSlot == this && iconSprite != null)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            iconSprite.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    public void AddItem(Item newItem)
    {
        GameObject newIcon = Instantiate(iconPrefab, transform.GetChild(0).transform, true);
        iconSprite = newIcon.GetComponent<SpriteRenderer>();
        itemData = iconSprite.GetComponent<ItemData>();
        newIcon.transform.position = transform.position;

        item = newItem;
        
        newIcon.name = item.name;
        iconSprite.sprite = item.inventoryIcon;

        slotBackgroundImage.sprite = fullSlotSprite;

        isEmpty = false;
        
        iconSprite.transform.localPosition = inv.GetItemInvPosition(newItem);
    }

    /// <summary> Only use when intending to destory an icon object. </summary>
    public void ClearSlot()
    {
        if (iconSprite != null)
        {
            Destroy(iconSprite.gameObject);

            iconSprite = null;
            item = null;

            slotBackgroundImage.sprite = emptySlotSprite;

            isEmpty = true;
        }
    }

    /// <summary> Sets slot to empty and changes the slot's background sprite to the empty slot sprite. </summary>
    public void SoftClearSlot(InventorySlot slotToClear)
    {
        slotToClear.isEmpty = true;
        slotToClear.slotBackgroundImage.sprite = emptySlotSprite;
    }

    public void SoftFillSlot(InventorySlot slotToFill)
    {
        slotToFill.isEmpty = false;
        slotToFill.slotBackgroundImage.sprite = fullSlotSprite;
    }

    public void UpdateSlot(Item newItem)
    {
        item = newItem;

        iconSprite.name = item.name;
        iconSprite.sprite = item.inventoryIcon;
        iconSprite.transform.position = transform.position;

        isEmpty = false;
        slotBackgroundImage.sprite = fullSlotSprite;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            ClearSlot();
        }
    }

    public void MoveItem()
    {
        if (isEmpty == false && invUI.currentlySelectedItem == null) // If there's an item in the slot and we haven't selected an item to move yet
        {
            // Determine the parent slot & item (in case we select the item from one of the children slots)
            if (parentSlot == null)
            {
                invUI.currentlySelectedItem = item; // If parent slot is null, then this must be the parent slot
                invUI.movingFromSlot = this;
                SoftClearSlot(this);
            }
            else
            {
                invUI.currentlySelectedItem = parentSlot.item; // Otherwise we'll grab the parent slot & item of this child slot
                invUI.movingFromSlot = parentSlot;
                SoftClearSlot(parentSlot);
            }

            // Set isEmpty and slotBackgroundImage of our parent and children slots
            if (parentSlot != null)
            {
                parentSlot.isEmpty = true;
                parentSlot.slotBackgroundImage.sprite = emptySlotSprite;
                foreach(InventorySlot childSlot in parentSlot.childrenSlots) // Set all children to empty
                {
                    if (childSlot != null) // Our child slot array won't always be full
                        SoftClearSlot(childSlot);
                }
            }
            else
            {
                foreach(InventorySlot childSlot in childrenSlots)
                {
                    if (childSlot != null)
                        SoftClearSlot(childSlot);
                }
            }

            if (slotParent.name == "Pockets")
                inv.pocketItems.Remove(invUI.currentlySelectedItem);
            else if (slotParent.name == "Bag")
                inv.bagItems.Remove(invUI.currentlySelectedItem);
            else if (slotParent.name == "Horse Bags")
                inv.horseBagItems.Remove(invUI.currentlySelectedItem);
        }
        else if (invUI.currentlySelectedItem != null) // If we've selected an item to move (currently will be following cursor if using mouse)
        {
            if (slotParent.name == "Pockets")
            {
                inv.pocketItems.Remove(invUI.currentlySelectedItem);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.pocketsSlots, inv.pocketItems, itemData) == false)
                {
                    Debug.Log("Cannot place item here.");
                }
                else
                {
                    Debug.Log("Item successfully placed");
                }
            }
            else if (slotParent.name == "Bag")
            {
                inv.bagItems.Remove(invUI.currentlySelectedItem);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.bagSlots, inv.bagItems, itemData) == false)
                {
                    Debug.Log("Cannot place item here.");
                }
                else
                {
                    Debug.Log("Item successfully placed");
                }
            }
            else if (slotParent.name == "Horse Bags")
            {
                inv.horseBagItems.Remove(invUI.currentlySelectedItem);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.horseBagSlots, inv.horseBagItems, itemData) == false)
                {
                    Debug.Log("Cannot place item here.");
                }
                else
                {
                    Debug.Log("Item successfully placed");
                }
            }

            /*if (invUI.movingFromSlot == this) // If we're placing the item back in its original slot
            {
                invUI.currentlySelectedItem = null;
                invUI.movingFromSlot = null;
                iconSprite.transform.position = transform.position;
                slotBackgroundImage.sprite = fullSlotSprite;
            }
            else if (item != null) // If slot already has an item in it
            {
                // Set this item as our newly selected item
                //invUI.movingFromSlot.UpdateSlot(item); // Update the slot we're moving the item from
                //UpdateSlot(invUI.currentlySelectedItem); // Update slot we're trying to move the item to
                
            }
            else // If slot is empty
            {
                AddItem(invUI.currentlySelectedItem); // Add item to the slot we want to move it to
                invUI.movingFromSlot.ClearSlot(); // Clear the slot we're moving it from
            }
            
            invUI.currentlySelectedItem = null;
            invUI.movingFromSlot = null;*/
        }
    }
}
