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

    [Header("Item Data")]
    public SpriteRenderer iconSprite;
    public Item item;
    public ItemData itemData;
    public InventorySlot parentSlot;
    public InventorySlot[] childrenSlots = new InventorySlot[7];

    [HideInInspector] public Transform slotParent;
    [HideInInspector] public Text stackSizeText;
    Inventory inv;
    InventoryUI invUI;
    
    Vector3 mousePos;
    float xPosOffset = 0;
    float yPosOffset = 0;

    void Awake()
    {
        inv = Inventory.instance;
        invUI = InventoryUI.instance;
        stackSizeText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        FollowMouse();
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
            stackSizeText.text = "";

            isEmpty = true;

            for (int i = 0; i < childrenSlots.Length; i++)
            {
                if (childrenSlots[i] != null)
                {
                    childrenSlots[i].parentSlot = null;
                    SoftClearSlot(childrenSlots[i]);
                    childrenSlots[i] = null;
                }
            }
        }
    }

    /// <summary> Sets slot to empty and changes the slot's background sprite to the empty slot sprite. </summary>
    public void SoftClearSlot(InventorySlot slotToClear)
    {
        slotToClear.isEmpty = true;
        slotToClear.slotBackgroundImage.sprite = emptySlotSprite;
        slotToClear.stackSizeText.text = "";
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

    public void MoveItem()
    {
        if (isEmpty == false && invUI.currentlySelectedItem == null) // If there's an item in the slot and we haven't selected an item to move yet
        {
            // Determine the parent slot & item (in case we select the item from one of the children slots)
            if (parentSlot == null)
            {
                stackSizeText.text = "";
                invUI.currentlySelectedItem = item; // If parent slot is null, then this must be the parent slot
                invUI.currentlySelectedItemData = itemData;
                invUI.invSlotMovingFrom = this;
                SoftClearSlot(this);
            }
            else
            {
                parentSlot.stackSizeText.text = "";
                invUI.currentlySelectedItem = parentSlot.item; // Otherwise we'll grab the parent slot & item of this child slot
                invUI.currentlySelectedItemData = parentSlot.itemData;
                invUI.invSlotMovingFrom = parentSlot;
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
                inv.pocketItems.Remove(invUI.currentlySelectedItemData);
            else if (slotParent.name == "Bag")
                inv.bagItems.Remove(invUI.currentlySelectedItemData);
            else if (slotParent.name == "Horse Bags")
                inv.horseBagItems.Remove(invUI.currentlySelectedItemData);
        }
        else if (invUI.currentlySelectedItem != null) // If we've selected an item to move (currently will be following cursor if using mouse)
        {
            if (slotParent.name == "Pockets")
            {
                inv.pocketItems.Remove(invUI.currentlySelectedItemData);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.pocketsSlots, inv.pocketItems, invUI.currentlySelectedItemData) == false)
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
                inv.bagItems.Remove(invUI.currentlySelectedItemData);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.bagSlots, inv.bagItems, invUI.currentlySelectedItemData) == false)
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
                inv.horseBagItems.Remove(invUI.currentlySelectedItemData);
                if (inv.DetermineIfValidInventoryPosition(invUI.currentlySelectedItem, this, invUI.horseBagSlots, inv.horseBagItems, invUI.currentlySelectedItemData) == false)
                {
                    Debug.Log("Cannot place item here.");
                }
                else
                {
                    Debug.Log("Item successfully placed");
                }
            }
        }
    }

    void FollowMouse()
    {
        // If we have a selected item & we're moving the item from this slot & we have an icon sprite
        if (invUI.currentlySelectedItem != null && invUI.invSlotMovingFrom == this && iconSprite != null)
        {
            if (invUI.currentlySelectedItem.iconWidth == 1)
                xPosOffset = 0;
            else if (invUI.currentlySelectedItem.iconWidth == 2)
                xPosOffset = 0.5f;
            else
                xPosOffset = 1;

            if (invUI.currentlySelectedItem.iconHeight == 1)
                yPosOffset = 0;
            else if (invUI.currentlySelectedItem.iconHeight == 2)
                yPosOffset = 0.5f;
            else
                yPosOffset = 1;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            iconSprite.transform.position = new Vector3(mousePos.x + xPosOffset, mousePos.y - yPosOffset, 0);
            Cursor.visible = false;
        }
    }

    public InventorySlot GetParentSlot(InventorySlot slot)
    {
        InventorySlot parentSlot;
        if (slot.parentSlot == null) // If this is the parent slot
            parentSlot = slot;
        else
            parentSlot = slot.parentSlot; // If this is a child slot
        
        return parentSlot;
    }
}
