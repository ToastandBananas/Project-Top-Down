using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    InventoryUI invUI;

    //public delegate void OnItemChanged();
    //public OnItemChanged onItemChangedCallback;

    public int pocketsSlotCount = 10;
    public int bagSlotCount = 10;
    public int horseBagSlotCount = 10;

    public List<ItemData> pocketItems   = new List<ItemData>();
    public List<ItemData> bagItems      = new List<ItemData>();
    public List<ItemData> horseBagItems = new List<ItemData>();

    #region Singleton
    public static Inventory instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of Inventory found!");
            Destroy(this);
        }
    }
    #endregion

    void Start()
    {
        invUI = InventoryUI.instance;
    }

    public bool AddToPockets(Item itemToAdd, ItemData itemData)
    {
        if (itemToAdd.isPickupable)
        {
            if (pocketItems.Count >= pocketsSlotCount) // Only ever used if your inventory is completely full of items 1x1 in size
            {
                Debug.Log("Not enough room in pockets.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.pocketsSlots, pocketItems, itemData) == true)
                return true;
        }

        // Return false if we can't find room for our item (it probably is too large)
        Debug.Log("Not enough room in our pockets (or item is not pickupable.");
        return false;
    }

    public bool AddToBag(Item itemToAdd, ItemData itemData)
    {
        if (itemToAdd.isPickupable)
        {
            if (bagItems.Count >= bagSlotCount)
            {
                Debug.Log("Not enough room in bag.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.bagSlots, bagItems, itemData) == true)
                return true;
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToBag code. We should either return true or false before this point.");
        return false;
    }

    public bool AddToHorseBag(Item itemToAdd, ItemData itemData)
    {
        if (itemToAdd.isPickupable)
        {
            if (horseBagItems.Count >= horseBagSlotCount)
            {
                Debug.Log("Not enough room in horse bags.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.horseBagSlots, horseBagItems, itemData) == true)
                return true;
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToHorseBag code. We should either return true or false before this point.");
        return false;
    }

    public bool CalculateItemInvPositionFromPickup(Item itemToAdd, InventorySlot[] invSlots, List<ItemData> itemsList, ItemData itemData)
    {
        int totalSlotsToCheck = (itemToAdd.iconWidth * itemToAdd.iconHeight);
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];
        int currentSlotsToFillIndex = 0;

        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i].isEmpty) // This slot is empty, so no need to check it with our GetSlotByCoordinates function
            {
                for (int x = 0; x < itemToAdd.iconWidth; x++)
                {
                    for (int y = 0; y < itemToAdd.iconHeight; y++) // Find an appropriate spot in our inv for our item after picking it up
                    {
                        InventorySlot slotToCheck = GetSlotByCoordinates(new Vector2(invSlots[i].slotCoordinate.x + x, invSlots[i].slotCoordinate.y + y), invSlots);
                        if (slotToCheck.isEmpty)
                        {
                            slotsToFill[currentSlotsToFillIndex] = slotToCheck;
                            currentSlotsToFillIndex++;
                        }
                    }
                }

                if (currentSlotsToFillIndex == totalSlotsToCheck) // We found a valid slot to put our item!
                {
                    invSlots[i].AddItem(itemToAdd);

                    if (itemData.currentStackSize > 1)
                        invSlots[i].stackSizeText.text = itemData.currentStackSize.ToString(); // Set our stack size text for the slot

                    itemData.TransferData(itemData, invSlots[i].itemData); // Transfer our item data from the gameobject to the inventory object

                    for (int j = 0; j < slotsToFill.Length; j++)
                    {
                        if (slotsToFill[j] != null)
                        {
                            if (j > 0) // We don't want to set the parent slot as a child of itself
                                invSlots[i].childrenSlots[j] = slotsToFill[j]; // Set the parent slot's children slots

                            slotsToFill[j].isEmpty = false;
                            slotsToFill[j].slotBackgroundImage.sprite = slotsToFill[j].fullSlotSprite;
                            
                            if (invSlots[i] != slotsToFill[j])
                                slotsToFill[j].parentSlot = invSlots[i]; // Set each child slot's parent slot
                        }
                    }
                    
                    itemsList.Add(slotsToFill[0].itemData);
                    return true;
                }
                else // This slot won't work for our item, so reset our slotsToFill array & index and try again with the next slot
                {
                    slotsToFill = new InventorySlot[totalSlotsToCheck];
                    currentSlotsToFillIndex = 0;
                }
            }
        }

        return false;
    }

    public bool DetermineIfValidInventoryPosition(Item itemToAdd, InventorySlot startSlot, InventorySlot[] invSlots, List<ItemData> itemsList, ItemData itemData)
    {
        if (startSlot == invUI.movingFromSlot) // If we're trying to place the item back in the same spot
        {
            startSlot.UpdateSlot(itemToAdd);
            foreach (InventorySlot slot in invUI.movingFromSlot.childrenSlots)
            {
                if (slot != null)
                    slot.SoftFillSlot(slot);
            }

            if (startSlot.itemData.currentStackSize > 1)
                startSlot.stackSizeText.text = itemData.currentStackSize.ToString();

            invUI.StopDraggingInvItem();
            startSlot.iconSprite.transform.localPosition = GetItemInvPosition(startSlot.item);
            itemsList.Add(itemData);
            return true;
        }

        int totalSlotsToCheck = itemToAdd.iconWidth * itemToAdd.iconHeight;
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];
        int currentSlotsToFillIndex = 0;

        for (int x = 0; x < itemToAdd.iconWidth; x++)
        {
            for (int y = 0; y < itemToAdd.iconHeight; y++)
            {
                // Get each slot within our items bounds (width * height) and add it to our slotsToFill array if it's a valid slot
                InventorySlot slotToCheck = GetSlotByCoordinates(new Vector2(startSlot.slotCoordinate.x + x, startSlot.slotCoordinate.y + y), invSlots);
                if (slotToCheck == null) // If the slot doesn't exist (happens when trying to place a large item at the very bottom or far right side of the inventory)
                {
                    Debug.Log("You're trying to place item in an invalid position.");
                    return false;
                }
                else
                {
                    slotsToFill[currentSlotsToFillIndex] = slotToCheck; // Add the slot to our slotsToFill array and increase the array's index
                    currentSlotsToFillIndex++;
                }
            }
        } 

        // Determine if we're trying to replace any items...if we're trying to replace more than 1, return false
        InventorySlot[] parentSlotsTryingToReplace = new InventorySlot[2]; // Max array size of 2 because we can only replace 1 item. So if there's ever 2 items in this array, we return false
        int itemsTryingToReplaceCount = 0; // Index of parentSlotsTryingToReplace array...will increase every time an item is pushed to the array
        foreach (InventorySlot slot in slotsToFill) // Determine how many (if any) items we're trying to place this item on top of
        {
            // Debug.Log(slot.slotCoordinate);
            if (itemsTryingToReplaceCount <= 1) // If we're trying to replace one item or less
            {
                if (slot.isEmpty == false) // If there's something already in this slot
                {
                    if (slot.parentSlot != null) // If this is a child slot
                    {
                        if (itemsTryingToReplaceCount > 0 && slot.parentSlot != parentSlotsTryingToReplace[0]) // If we already found one item and the parent slot of this slot isn't the same
                        {
                            parentSlotsTryingToReplace[1] = slot.parentSlot; // Add the item's slot to our array and increase the array's index
                            itemsTryingToReplaceCount++;
                        }
                        else if (itemsTryingToReplaceCount == 0) // If we haven't found an item yet
                        {
                            parentSlotsTryingToReplace[0] = slot.parentSlot; // Add the item's slot to our array and increase the array's index
                            itemsTryingToReplaceCount++;
                        }
                    }
                    else // If this is a parent slot
                    {
                        if (itemsTryingToReplaceCount > 0 && slot != parentSlotsTryingToReplace[0]) // If we already found one item and this slot isn't the same as that item's slot
                        {
                            parentSlotsTryingToReplace[1] = slot; // Add the item's slot to our array and increase the array's index
                            itemsTryingToReplaceCount++;
                        }
                        else if (itemsTryingToReplaceCount == 0) // If we haven't found an item yet
                        {
                            parentSlotsTryingToReplace[0] = slot; // Add the item's slot to our array and increase the array's index
                            itemsTryingToReplaceCount++;
                        }
                    }
                }
            }
            else
                return false; // If we're trying to replace more than one item, then this position is invalid
        }

        if (itemsTryingToReplaceCount == 0) // If we're not going to replace any items
        {
            ClearParentAndChildSlots(startSlot);
            startSlot.AddItem(itemToAdd); // Add the item to the slot we clicked on

            // Here we'll be determining the parent and child slots:
            SetParentAndChildSlots(itemToAdd, startSlot, invSlots);

            // Clear out the parent/children slots of the slot we're moving the item from
            for (int i = 0; i < invUI.movingFromSlot.childrenSlots.Length; i++)
            {
                if (invUI.movingFromSlot.childrenSlots[i] != null)
                {
                    invUI.movingFromSlot.childrenSlots[i].parentSlot = null;
                    invUI.movingFromSlot.childrenSlots[i] = null;
                }
            }

            if (itemData.currentStackSize > 1)
                startSlot.stackSizeText.text = itemData.currentStackSize.ToString();
            else
                startSlot.stackSizeText.text = "";

            itemData.TransferData(itemData, startSlot.itemData); // Transfer data to the new slot before clearing it
            invUI.movingFromSlot.ClearSlot();

            foreach (InventorySlot childSlot in startSlot.childrenSlots) // Soft fill our child slots just in case the moving from slot is now one of these children
            {
                if (childSlot != null)
                    childSlot.SoftFillSlot(childSlot);
            }

            // We no longer have an item selected, so set the appropriate variables to null
            invUI.StopDraggingInvItem();
        }

        if (itemsTryingToReplaceCount == 1) // If we are swapping with an item
        {
            bool movingFromSlotSet = false;
            bool itemAdded = false;

            itemsList.Remove(parentSlotsTryingToReplace[0].itemData);
            
            invUI.movingFromSlot.ClearSlot();

            foreach (InventorySlot slot in slotsToFill) // For each slot we're trying to fill
            {
                if (slot.isEmpty == false && slot.parentSlot != null) // If slot isn't empty and is a child slot
                {
                    foreach (InventorySlot childSlot in slot.parentSlot.childrenSlots) // For each of this child slot's related child slot's
                    {
                        if (childSlot != null)
                        {
                            if (childSlot != startSlot)
                                childSlot.SoftClearSlot(childSlot); // Soft clear this child slot so it appears empty

                            if (movingFromSlotSet == false)
                            {
                                invUI.movingFromSlot = slot.parentSlot; // Our new movingFromSlot will be the parent slot of the item we are replacing
                                movingFromSlotSet = true;
                            }
                        }

                        if (slot == startSlot) // If this slot is the slot we clicked on
                        {
                            if (itemAdded == false && childSlot != null && slot != childSlot.parentSlot)
                            {
                                slot.AddItem(itemToAdd);
                                itemData.TransferData(itemData, startSlot.itemData);
                                
                                if (startSlot.itemData.currentStackSize > 1)
                                    slot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();
                                else
                                    startSlot.stackSizeText.text = "";

                                itemAdded = true;
                                invUI.movingFromSlot.SoftClearSlot(invUI.movingFromSlot); // Soft clear our new movingFromSlot so that it appears empty
                            }
                        }
                    }

                    invUI.currentlySelectedItem = slot.parentSlot.item;
                    invUI.currentlySelectedItemData = slot.parentSlot.itemData;
                }
                else if (slot.isEmpty == false) // If this is the parent slot of the item we're replacing
                {
                    if (slot == startSlot && itemAdded == false) // If our startSlot equals the parent slot of the item we're replacing (basically, if we clicked on a slot that is an item's parent slot)
                    {
                        invUI.tempSlot.AddItem(slot.item);
                        itemData.TransferData(slot.itemData, invUI.tempSlot.itemData);

                        foreach (InventorySlot childSlot in slot.childrenSlots)
                        {
                            if (childSlot != null)
                                childSlot.SoftClearSlot(childSlot);
                        }
                        
                        slot.ClearSlot();

                        if (movingFromSlotSet == false)
                        {
                            invUI.movingFromSlot = invUI.tempSlot;
                            movingFromSlotSet = true;
                        }

                        slot.AddItem(itemToAdd);
                        itemAdded = true;
                        itemData.TransferData(invUI.currentlySelectedItemData, slot.itemData);

                        if (slot.itemData.currentStackSize > 1)
                            slot.stackSizeText.text = slot.itemData.currentStackSize.ToString();
                        else
                            slot.stackSizeText.text = "";

                        invUI.currentlySelectedItem = invUI.tempSlot.item;
                        invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
                        //Debug.Log("Updating slot...");
                    }
                    else // If our startSlot is not the parent slot of the item we're replacing, but the parent slot is still being overlapped by our new item
                    {
                        if (slot.item != null)
                        {
                            invUI.tempSlot.AddItem(slot.item);
                            itemData.TransferData(slot.itemData, invUI.tempSlot.itemData);
                        }
                        
                        slot.ClearSlot();

                        if (movingFromSlotSet == false)
                        {
                            itemData.TransferData(invUI.currentlySelectedItemData, startSlot.itemData);
                            if (startSlot.itemData.currentStackSize > 1)
                                startSlot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();
                            else
                                startSlot.stackSizeText.text = "";

                            invUI.movingFromSlot = invUI.tempSlot;
                            movingFromSlotSet = true;
                        }
                        
                        invUI.currentlySelectedItem = invUI.tempSlot.item;
                        invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
                    }
                }
                else // If slot is empty
                {
                    if (slot == startSlot && itemAdded == false)
                    {
                        slot.AddItem(itemToAdd);
                        itemData.TransferData(itemData, startSlot.itemData);
                        if (startSlot.itemData.currentStackSize > 1)
                            startSlot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();

                        itemAdded = true;
                        //Debug.Log("Adding item to empty slot...");
                    }
                }

                foreach (InventorySlot childSlot in invUI.movingFromSlot.childrenSlots)
                {
                    if (childSlot != null && childSlot != startSlot)
                        childSlot.SoftClearSlot(childSlot);
                }

                ClearParentAndChildSlots(slot);
                ClearParentAndChildSlots(invUI.movingFromSlot);

                slot.SoftFillSlot(slot);
            }
            
            SetParentAndChildSlots(itemToAdd, startSlot, invSlots);

            foreach (InventorySlot childSlot in startSlot.childrenSlots)
            {
                if (childSlot != null)
                    childSlot.stackSizeText.text = "";
            }
        }

        foreach(InventorySlot slot in invSlots)
        {
            bool shouldSoftClear = true;
            foreach (InventorySlot childSlot in slot.childrenSlots)
            {
                if (childSlot != null)
                    shouldSoftClear = false;
            }

            if (shouldSoftClear)
            {
                if (slot.parentSlot != null)
                    shouldSoftClear = false;
            }

            if (shouldSoftClear)
                slot.SoftClearSlot(slot);
        }

        foreach (InventorySlot slot in slotsToFill)
        {
            slot.SoftFillSlot(slot);
        }

        startSlot.iconSprite.transform.localPosition = GetItemInvPosition(startSlot.item);
        itemsList.Add(startSlot.itemData);
        return true;
    }

    public InventorySlot GetSlotByCoordinates(Vector2 coordinate, InventorySlot[] slots)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.slotCoordinate == coordinate)
                return slot;
        }

        return null; // We should never get to this line.
    }

    public void ClearParentAndChildSlots(InventorySlot slot)
    {
        for (int i = 0; i < slot.childrenSlots.Length; i++)
        {
            if (slot.childrenSlots[i] != null)
            {
                slot.childrenSlots[i].parentSlot = null;
                slot.childrenSlots[i] = null;
            }
        }

        slot.parentSlot = null;
    }

    public void SetParentAndChildSlots(Item itemToAdd, InventorySlot startSlot, InventorySlot[] invSlots)
    {
        InventorySlot[] inventorySlots = new InventorySlot[itemToAdd.iconWidth * itemToAdd.iconHeight]; // Each slot that will be filled
        InventorySlot[] childSlots = new InventorySlot[(itemToAdd.iconWidth * itemToAdd.iconHeight) - 1]; // Children of the parent slot
        int invSlotCount = 0;
        int childSlotCount = 0;
        for (int x = 0; x < itemToAdd.iconWidth; x++) // Go through each slot within the item's bounds (width*height)
        {
            for (int y = 0; y < itemToAdd.iconHeight; y++)
            {
                // Get each slot, starting with the startSlot and soft fill them (not empty and has fullSlotSprite)
                inventorySlots[invSlotCount] = GetSlotByCoordinates(new Vector2(startSlot.slotCoordinate.x + x, startSlot.slotCoordinate.y + y), invSlots);
                inventorySlots[invSlotCount].SoftFillSlot(inventorySlots[invSlotCount]);

                if (inventorySlots[invSlotCount] != startSlot) // The startSlot is our parent, so add all other slots as children of the startSlot
                {
                    inventorySlots[invSlotCount].parentSlot = startSlot; // Set the startSlot as the parent of this child slot
                    childSlots[childSlotCount] = inventorySlots[invSlotCount]; // Add this slot to our childSlots array and increase the arrays index
                    childSlotCount++;
                }

                invSlotCount++; // Increase our inventorySlots index
            }
        }

        // Here we'll actually set the children slots for the parent slot
        for (int i = 0; i < childSlots.Length; i++)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                //if (slot.childrenSlots.Length == 0)
                    //slot.childrenSlots = new InventorySlot[5];
                if (slot == startSlot) // If this is the startSlot, then this is the parent slot
                {
                    slot.childrenSlots[i] = childSlots[i]; // Add the current child (from the for loop) to our parent slot's childrenSlots array
                    break; // We only need to do this for our parent slot, so once that's done, break out of the foreach loop
                }
            }
        }
    }

    public void Remove(ItemData itemData)
    {
        if (bagItems.Contains(itemData))
            bagItems.Remove(itemData);
        else if (pocketItems.Contains(itemData))
            pocketItems.Remove(itemData);
        else if (horseBagItems.Contains(itemData))
            horseBagItems.Remove(itemData);
    }

    public void SortItemsAlphabetically(List<Item> itemsList, InventorySlot[] slotsArray)
    {
        itemsList.Sort(delegate (Item i1, Item i2) { return i1.name.CompareTo(i2.name); });
        for (int i = 0; i < slotsArray.Length; i++)
        {
            if (i < itemsList.Count)
                slotsArray[i].UpdateSlot(itemsList[i]);
        }
    }

    public Vector3 GetItemInvPosition(Item item)
    {
        float addWidth = 0;
        float addHeight = 0;
        if (item.iconWidth == 2)
            addWidth = 37.5f;
        else if (item.iconWidth == 3)
            addWidth = 75f;
        if (item.iconHeight == 2)
            addHeight = -37.5f;
        else if (item.iconHeight == 3)
            addHeight = -75f;

        return new Vector3(addWidth, addHeight, 0);
    }
}
