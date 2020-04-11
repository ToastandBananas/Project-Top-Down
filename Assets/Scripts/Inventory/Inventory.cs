using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    InventoryUI invUI;
    AudioManager audioManager;
    PlayerMovement playerMovement;
    BasicStats playerBasicStats;

    public int pocketsSlotCount = 10;
    public int bagSlotCount = 10;
    public int horseBagSlotCount = 10;

    public List<ItemData> pocketItems   = new List<ItemData>();
    public List<ItemData> bagItems      = new List<ItemData>();
    public List<ItemData> horseBagItems = new List<ItemData>();

    InventorySlot[] parentSlotsTryingToReplace;
    int itemsTryingToReplaceCount;

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
        audioManager = AudioManager.instance;
        playerMovement = PlayerMovement.instance;
        playerBasicStats = playerMovement.GetComponent<BasicStats>();
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

            if (CalculateItemInvPositionFromPickup(itemToAdd, itemData, invUI.pocketsSlots, pocketItems) == true)
                return true;
        }

        // Return false if we can't find room for our item (it probably is too large)
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

            if (CalculateItemInvPositionFromPickup(itemToAdd, itemData, invUI.bagSlots, bagItems) == true)
                return true;
        }

        // We should never get here...if we do there's an error in our code
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

            if (CalculateItemInvPositionFromPickup(itemToAdd, itemData, invUI.horseBagSlots, horseBagItems) == true)
                return true;
        }
        
        return false;
    }

    public bool AddToInventory(Item itemToAdd, ItemData itemData)
    {
        if (AddToPockets(itemToAdd, itemData))
        {
            audioManager.PlayPickUpItemSound(itemToAdd);
            return true;
        }
        else if (AddToBag(itemToAdd, itemData))
        {
            audioManager.PlayPickUpItemSound(itemToAdd);
            return true;
        }
        else if (playerMovement.isMounted && AddToHorseBag(itemToAdd, itemData))
        {
            audioManager.PlayPickUpItemSound(itemToAdd);
            return true;
        }

        return false;
    }

    public bool CalculateItemInvPositionFromPickup(Item itemToAdd, ItemData itemData, List<InventorySlot> invSlots, List<ItemData> itemsList)
    {
        int totalSlotsToCheck = (itemToAdd.iconWidth * itemToAdd.iconHeight);
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];
        int currentSlotsToFillIndex = 0;
        
        if (itemToAdd.itemType == ItemType.Ammunition)
        {
            for (int i = 0; i < invSlots.Count; i++)
            {
                if (itemData.currentStackSize <= 0)
                    break;

                if (invSlots[i].isEmpty == false)
                {
                    InventorySlot parentSlot = invSlots[i].GetParentSlot(invSlots[i]);
                    if (parentSlot.item.name == itemToAdd.name)
                    {
                        for (int j = 0; j < itemData.currentStackSize + 1; j++)
                        {
                            if (parentSlot.itemData.currentStackSize < parentSlot.item.maxStackSize)
                            {
                                parentSlot.itemData.currentStackSize++;
                                parentSlot.stackSizeText.text = parentSlot.itemData.currentStackSize.ToString();
                                itemData.currentStackSize--;
                            }
                            else
                                break;
                        }
                    }

                    parentSlot.SetAmmoSprites();
                }
            }
        }

        for (int i = 0; i < invSlots.Count; i++)
        {
            if (invSlots[i].isEmpty) // This slot is empty, so no need to check it with our GetSlotByCoordinates function
            {
                for (int x = 0; x < itemToAdd.iconWidth; x++)
                {
                    for (int y = 0; y < itemToAdd.iconHeight; y++) // Find an appropriate spot in our inv for our item after picking it up
                    {
                        InventorySlot slotToCheck = GetSlotByCoordinates(new Vector2(invSlots[i].slotCoordinate.x + x, invSlots[i].slotCoordinate.y + y), invSlots);
                        if (slotToCheck != null && slotToCheck.isEmpty)
                        {
                            slotsToFill[currentSlotsToFillIndex] = slotToCheck;
                            currentSlotsToFillIndex++;
                        }
                    }
                }

                if (currentSlotsToFillIndex == totalSlotsToCheck) // We found a valid slot to put our item!
                {
                    invSlots[i].AddItem(itemToAdd, itemData);

                    itemData.TransferData(itemData, invSlots[i].itemData); // Transfer our item data from the gameobject to the inventory object

                    if (itemData.item.itemType == ItemType.Ammunition && itemData.currentStackSize > 1)
                        invSlots[i].stackSizeText.text = invSlots[i].itemData.currentStackSize.ToString(); // Set our stack size text for the slot
                    else if (itemData.item.itemType == ItemType.Quiver && itemData.currentAmmoCount > 0)
                        invSlots[i].stackSizeText.text = invSlots[i].itemData.currentAmmoCount.ToString();

                    invSlots[i].SetAmmoSprites();

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

                    if (itemsList != null)
                    {
                        if (itemsList == pocketItems || itemsList == bagItems || itemsList == horseBagItems)
                        {
                            itemData.currentStackSize = 0;
                            itemsList.Add(slotsToFill[0].itemData);
                        }
                        else // if container list
                        {
                            itemsList.Add(itemData);
                            slotsToFill[0].itemData = itemData;
                        }
                    }
                    else
                        slotsToFill[0].itemData = itemData;

                    // Get rid of the item if it has a stack size of 0 (after a stackable item such as arrows distributes itself throughout the inventory)
                    if (slotsToFill[0].itemData.currentStackSize <= 0)
                        slotsToFill[0].ClearSlot();

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

    public bool DetermineIfValidInventoryPosition(Item itemToAdd, ItemData itemData, InventorySlot startSlot, List<InventorySlot> invSlots, List<ItemData> itemsList)
    {
        // If we're trying to place the item back in the same spot and we previously weren't replacing an item
        if (startSlot == invUI.invSlotMovingFrom && itemsTryingToReplaceCount == 0)
        {
            startSlot.UpdateSlot(itemToAdd);
            foreach (InventorySlot slot in invUI.invSlotMovingFrom.childrenSlots)
            {
                if (slot != null)
                    slot.SoftFillSlot(slot);
            }

            if (startSlot.item.itemType == ItemType.Ammunition && startSlot.itemData.currentStackSize > 1)
                startSlot.stackSizeText.text = itemData.currentStackSize.ToString();
            else if (startSlot.item.itemType == ItemType.Quiver && startSlot.itemData.currentAmmoCount > 0)
                startSlot.stackSizeText.text = itemData.currentAmmoCount.ToString();

            invUI.StopDraggingInvItem();
            startSlot.iconImage.transform.SetParent(startSlot.GetBottomRightChildSlot(itemToAdd, startSlot).transform);
            startSlot.iconImage.transform.localPosition = GetItemInvPositionOffset(startSlot.item);

            startSlot.SetAmmoSprites();

            // Add ItemDatas/GameObjects to the appropriate lists
            if (invUI.currentlyActiveContainer == null || itemsList != invUI.currentlyActiveContainer.containerItems)
                itemsList.Add(itemData);
            else if (invUI.currentlyActiveContainer != null && itemsList == invUI.currentlyActiveContainer.containerItems)
            {
                itemsList.Add(itemData);
                invUI.currentlyActiveContainer.containerObjects.Add(itemData.gameObject);
            }

            audioManager.PlayPickUpItemSound(itemToAdd);
            return true;
        }

        int totalSlotsToCheck = itemToAdd.iconWidth * itemToAdd.iconHeight;
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];

        itemsTryingToReplaceCount = GetOverlappingItemCount(itemToAdd, startSlot, slotsToFill, invSlots);

        if (itemsTryingToReplaceCount == 2)
            return false;

        if (itemsTryingToReplaceCount == 0) // If we're not going to replace any items
        {
            ClearParentAndChildSlots(startSlot);
            startSlot.AddItem(itemToAdd, itemData); // Add the item to the slot we clicked on

            // Clear out the parent/children slots of the slot we're moving the item from
            if (invUI.invSlotMovingFrom != null)
            {
                for (int i = 0; i < invUI.invSlotMovingFrom.childrenSlots.Length; i++)
                {
                    if (invUI.invSlotMovingFrom.childrenSlots[i] != null)
                    {
                        invUI.invSlotMovingFrom.childrenSlots[i].parentSlot = null;
                        invUI.invSlotMovingFrom.childrenSlots[i] = null;
                    }
                }
            }

            itemData.TransferData(itemData, startSlot.itemData); // Transfer data to the new slot before clearing ititemslist.remove
            if (invUI.invSlotMovingFrom != null)
                invUI.invSlotMovingFrom.ClearSlot();
            else
                invUI.equipSlotMovingFrom.ClearSlot(invUI.equipSlotMovingFrom);

            if (itemData.item.itemType == ItemType.Ammunition && itemData.currentStackSize > 1)
                startSlot.stackSizeText.text = itemData.currentStackSize.ToString();
            else if (itemData.item.itemType == ItemType.Quiver && itemData.currentAmmoCount > 0)
                startSlot.stackSizeText.text = itemData.currentAmmoCount.ToString();
            else
                startSlot.stackSizeText.text = "";

            startSlot.SetAmmoSprites();

            foreach (InventorySlot childSlot in startSlot.childrenSlots) // Soft fill our child slots just in case the moving from slot is now one of these children
            {
                if (childSlot != null)
                    childSlot.SoftFillSlot(childSlot);
            }

            if (invUI.currentlyActiveContainer != null && invUI.invSlotMovingFrom != null && invUI.invSlotMovingFrom.slotParent == invUI.containerParent)
            {
                // Only will be used if this container is a dead body
                EquipmentManager deadBodyEquipmentManager = invUI.currentlyActiveContainer.GetComponent<EquipmentManager>();
                if (deadBodyEquipmentManager != null && itemData.equipment != null)
                    deadBodyEquipmentManager.currentEquipment[(int)itemData.equipment.equipmentSlot] = null;

                Destroy(itemData.gameObject);
            }

            // We no longer have an item selected, so set the appropriate variables to null
            invUI.StopDraggingInvItem();
        }
        
        if (itemsTryingToReplaceCount == 1) // If we are swapping with an item
        {
            bool movingFromSlotSet = false;
            bool itemAdded = false;

            // This is so that we can still access the slot we're moving from after we change invUI.invSlotMovingFrom
            InventorySlot oldInvSlotMovingFrom = null;
            if (invUI.invSlotMovingFrom != null)
                oldInvSlotMovingFrom = invUI.invSlotMovingFrom;

            itemsList.Remove(parentSlotsTryingToReplace[0].itemData);
            if (invUI.currentlyActiveContainer != null && itemsList == invUI.currentlyActiveContainer.containerItems)
            {
                foreach (GameObject obj in invUI.currentlyActiveContainer.containerObjects)
                {
                    if (obj.GetComponent<ItemData>() == parentSlotsTryingToReplace[0].itemData)
                    {
                        invUI.currentlyActiveContainer.containerObjects.Remove(obj);
                        break;
                    }
                }
            }

            if (invUI.invSlotMovingFrom != null)
                invUI.invSlotMovingFrom.ClearSlot();
            else
                invUI.equipSlotMovingFrom.ClearSlot(invUI.equipSlotMovingFrom);

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
                                invUI.invSlotMovingFrom = slot.parentSlot; // Our new movingFromSlot will be the parent slot of the item we are replacing
                                slot.parentSlot.iconImage.transform.SetParent(invUI.menusParent);
                                movingFromSlotSet = true;
                            }
                        }

                        if (slot == startSlot) // If this slot is the slot we clicked on
                        {
                            if (itemAdded == false && childSlot != null && slot != childSlot.parentSlot)
                            {
                                slot.AddItem(itemToAdd, itemData);
                                itemData.TransferData(itemData, startSlot.itemData);

                                if (startSlot.item.itemType == ItemType.Ammunition && startSlot.itemData.currentStackSize > 1)
                                    slot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();
                                else if (startSlot.item.itemType == ItemType.Quiver && startSlot.itemData.currentAmmoCount > 0)
                                    slot.stackSizeText.text = startSlot.itemData.currentAmmoCount.ToString();
                                else
                                    startSlot.stackSizeText.text = "";

                                slot.SetAmmoSprites();

                                itemAdded = true;

                                if (invUI.invSlotMovingFrom != null)
                                    invUI.invSlotMovingFrom.SoftClearSlot(invUI.invSlotMovingFrom); // Soft clear our new movingFromSlot so that it appears empty
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
                        // If this is ammunition of the same type, damage and value
                        if (invUI.currentlySelectedItem.itemType == ItemType.Ammunition
                            && ((slot.item.itemType == ItemType.Ammunition && invUI.currentlySelectedItemData.itemName == slot.itemData.itemName) || slot.item.itemType == ItemType.Quiver))
                        {
                            int currentStackSize = invUI.currentlySelectedItemData.currentStackSize;
                            for (int i = 0; i < currentStackSize; i++)
                            {
                                Equipment slotEquipment = (Equipment)slot.item;

                                if ((slot.item.itemType == ItemType.Ammunition && slot.itemData.currentStackSize < slot.item.maxStackSize)
                                    || (slot.item.itemType == ItemType.Quiver && slot.itemData.currentAmmoCount < slotEquipment.maxAmmo))
                                {
                                    if (slot.item.itemType == ItemType.Ammunition)
                                    {
                                        slot.itemData.currentStackSize++;
                                        slot.stackSizeText.text = slot.itemData.currentStackSize.ToString();
                                    }
                                    else if (slot.item.itemType == ItemType.Quiver)
                                    {
                                        slot.itemData.currentAmmoCount++;
                                        slot.stackSizeText.text = slot.itemData.currentAmmoCount.ToString();
                                        slot.itemData.ammoTypePrefab = invUI.currentlySelectedItem.prefab;
                                    }

                                    slot.SetAmmoSprites();

                                    invUI.currentlySelectedItemData.currentStackSize--;

                                    // If we transferred over all of our currently selected ammo
                                    if (invUI.currentlySelectedItemData.currentStackSize <= 0)
                                    {
                                        if (invUI.invSlotMovingFrom != null && invUI.invSlotMovingFrom.slotParent == invUI.containerParent)
                                        {
                                            invUI.invSlotMovingFrom.ClearSlot();
                                            Destroy(invUI.currentlySelectedItemData.gameObject);
                                        }

                                        itemsTryingToReplaceCount = 0;
                                        invUI.StopDraggingInvItem();
                                        audioManager.PlayPickUpItemSound(itemToAdd);
                                        return true;
                                    }
                                }
                                else if (slot.item.itemType == ItemType.Ammunition)
                                {
                                    // Swap stack sizes
                                    int intA = invUI.currentlySelectedItemData.currentStackSize;
                                    int intB = slot.itemData.currentStackSize;
                                    invUI.currentlySelectedItemData.currentStackSize = intB;
                                    slot.itemData.currentStackSize = intA;
                                    Destroy(slot.itemData.gameObject);
                                    break;
                                }
                            }
                        }
                        
                        invUI.tempSlot.AddItem(slot.item, slot.itemData);
                        itemData.TransferData(slot.itemData, invUI.tempSlot.itemData);
                        invUI.tempSlot.SetAmmoSprites();

                        foreach (InventorySlot childSlot in slot.childrenSlots)
                        {
                            if (childSlot != null)
                                childSlot.SoftClearSlot(childSlot);
                        }

                        slot.ClearSlot();

                        if (movingFromSlotSet == false)
                        {
                            invUI.invSlotMovingFrom = invUI.tempSlot;
                            movingFromSlotSet = true;
                        }

                        slot.AddItem(itemToAdd, invUI.currentlySelectedItemData);
                        itemAdded = true;
                        itemData.TransferData(invUI.currentlySelectedItemData, slot.itemData);

                        if (slot.item.itemType == ItemType.Ammunition && slot.itemData.currentStackSize > 1)
                            slot.stackSizeText.text = slot.itemData.currentStackSize.ToString();
                        else if (slot.item.itemType == ItemType.Quiver && slot.itemData.currentAmmoCount > 0)
                            slot.stackSizeText.text = slot.itemData.currentAmmoCount.ToString();
                        else
                            slot.stackSizeText.text = "";

                        slot.SetAmmoSprites();

                        invUI.currentlySelectedItem = invUI.tempSlot.item;
                        invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
                    }
                    else // If our startSlot is not the parent slot of the item we're replacing, but the parent slot is still being overlapped by our new item
                    {
                        if (slot.item != null)
                        {
                            invUI.tempSlot.AddItem(slot.item, slot.itemData);
                            itemData.TransferData(slot.itemData, invUI.tempSlot.itemData);
                            invUI.tempSlot.SetAmmoSprites();
                        }
                        
                        slot.ClearSlot();

                        if (movingFromSlotSet == false)
                        {
                            itemData.TransferData(invUI.currentlySelectedItemData, startSlot.itemData);
                            if (startSlot.item.itemType == ItemType.Ammunition && startSlot.itemData.currentStackSize > 1)
                                startSlot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();
                            else if (startSlot.item.itemType == ItemType.Quiver && startSlot.itemData.currentAmmoCount > 0)
                                startSlot.stackSizeText.text = startSlot.itemData.currentAmmoCount.ToString();
                            else
                                startSlot.stackSizeText.text = "";

                            startSlot.SetAmmoSprites();

                            invUI.invSlotMovingFrom = invUI.tempSlot;
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
                        slot.AddItem(itemToAdd, itemData);
                        itemData.TransferData(itemData, startSlot.itemData);
                        if (startSlot.item.itemType == ItemType.Ammunition && startSlot.itemData.currentStackSize > 1)
                            startSlot.stackSizeText.text = startSlot.itemData.currentStackSize.ToString();
                        else if (startSlot.item.itemType == ItemType.Quiver && startSlot.itemData.currentAmmoCount > 0)
                            startSlot.stackSizeText.text = startSlot.itemData.currentAmmoCount.ToString();

                        startSlot.SetAmmoSprites();

                        itemAdded = true;
                    }
                }

                if (invUI.invSlotMovingFrom != null)
                {
                    foreach (InventorySlot childSlot in invUI.invSlotMovingFrom.childrenSlots)
                    {
                        if (childSlot != null && childSlot != startSlot)
                            childSlot.SoftClearSlot(childSlot);
                    }
                }

                ClearParentAndChildSlots(slot);

                if (invUI.invSlotMovingFrom != null)
                    ClearParentAndChildSlots(invUI.invSlotMovingFrom);

                slot.SoftFillSlot(slot);
            }

            invUI.equipSlotMovingFrom = null;

            foreach (InventorySlot childSlot in startSlot.childrenSlots)
            {
                if (childSlot != null)
                    childSlot.stackSizeText.text = "";
            }

            if (invUI.currentlyActiveContainer != null && oldInvSlotMovingFrom != null && oldInvSlotMovingFrom.slotParent == invUI.containerParent)
            {
                // Only will be used if this container is a dead body
                EquipmentManager deadBodyEquipmentManager = invUI.currentlyActiveContainer.GetComponent<EquipmentManager>();
                if (deadBodyEquipmentManager != null && itemData.equipment != null)
                    deadBodyEquipmentManager.currentEquipment[(int)itemData.equipment.equipmentSlot] = null;

                Destroy(itemData.gameObject);
            }
        }

        // Determine the parent and child slots
        SetParentAndChildSlots(itemToAdd, startSlot, invSlots);

        // Soft clear the necessary slots
        foreach (InventorySlot slot in invSlots)
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

        // Soft fill the necessary slots
        foreach (InventorySlot slot in slotsToFill)
        {
            slot.SoftFillSlot(slot);
        }

        if (invUI.currentlyActiveContainer == null || itemsList != invUI.currentlyActiveContainer.containerItems)
            itemsList.Add(startSlot.itemData);
        else if (invUI.currentlyActiveContainer != null && itemsList == invUI.currentlyActiveContainer.containerItems)
        {
            // Create a gameobject with ItemData
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(invUI.currentlyActiveContainer.itemsParent);
            newObj.AddComponent<ItemData>();

            // Transfer data over to the new ItemData
            ItemData data = newObj.GetComponent<ItemData>();
            data.TransferData(itemData, data);
            newObj.name = data.itemName;

            // Add this object and it's ItemData to the appropriate container lists
            invUI.currentlyActiveContainer.containerObjects.Add(newObj);
            itemsList.Add(newObj.GetComponent<ItemData>());
            startSlot.itemData = newObj.GetComponent<ItemData>();
        }

        audioManager.PlayPickUpItemSound(itemToAdd);
        return true;
    }

    public InventorySlot GetSlotByCoordinates(Vector2 coordinate, List<InventorySlot> slots)
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

    public void SetParentAndChildSlots(Item itemToAdd, InventorySlot startSlot, List<InventorySlot> invSlots)
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
                if (slot == startSlot) // If this is the startSlot, then this is the parent slot
                {
                    slot.childrenSlots[i] = childSlots[i]; // Add the current child (from the for loop) to our parent slot's childrenSlots array
                    break; // We only need to do this for our parent slot, so once that's done, break out of the foreach loop
                }
            }
        }
    }

    public int GetOverlappingItemCount(Item itemToAdd, InventorySlot startSlot, InventorySlot[] slotsToFill, List<InventorySlot> invSlots)
    {
        int currentSlotsToFillIndex = 0;
        bool invalidPosition = false;

        for (int x = 0; x < itemToAdd.iconWidth; x++)
        {
            for (int y = 0; y < itemToAdd.iconHeight; y++)
            {
                // Get each slot within our items bounds (width * height) and add it to our slotsToFill array if it's a valid slot
                InventorySlot slotToCheck = GetSlotByCoordinates(new Vector2(startSlot.slotCoordinate.x + x, startSlot.slotCoordinate.y + y), invSlots);
                if (slotToCheck == null) // If the slot doesn't exist (happens when trying to place a large item at the very bottom or far right side of the inventory)
                {
                    // Debug.Log("You're trying to place item in an invalid position.");
                    invalidPosition = true;
                }
                else
                {
                    slotsToFill[currentSlotsToFillIndex] = slotToCheck; // Add the slot to our slotsToFill array and increase the array's index
                    currentSlotsToFillIndex++;
                }
            }
        }

        if (invalidPosition)
            return 2;

        int itemsTryingToReplaceCount = 0; // Index of parentSlotsTryingToReplace array...will increase every time an item is pushed to the array
        parentSlotsTryingToReplace = new InventorySlot[2]; // Max array size of 2 because we can only replace 1 item

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
                return itemsTryingToReplaceCount; // If we're trying to replace more than one item, then this position is invalid
        }
        
        return itemsTryingToReplaceCount;
    }

    public void RemoveItem(ItemData itemData)
    {
        if (bagItems.Contains(itemData))
            bagItems.Remove(itemData);
        else if (pocketItems.Contains(itemData))
            pocketItems.Remove(itemData);
        else if (horseBagItems.Contains(itemData))
            horseBagItems.Remove(itemData);
        else if (invUI.currentlyActiveContainer != null && invUI.currentlyActiveContainer.containerItems.Contains(itemData))
        {
            invUI.currentlyActiveContainer.containerItems.Remove(itemData);
            foreach (GameObject obj in invUI.currentlyActiveContainer.containerObjects)
            {
                if (obj.GetComponent<ItemData>() == itemData)
                {
                    invUI.currentlyActiveContainer.containerObjects.Remove(obj);
                    break;
                }
            }
        }
    }

    public void TakeGold()
    {
        audioManager.PlayPickUpGoldSound(invUI.currentlyActiveContainer.gold);
        playerBasicStats.gold += invUI.currentlyActiveContainer.gold;
        invUI.playerGoldText.text = playerBasicStats.gold.ToString();
        invUI.currentlyActiveContainer.gold = 0;
        invUI.containerMenuGoldText.text = 0.ToString();
    }

    public void TakeAll()
    {
        if (invUI.currentlyActiveContainer.gold > 0)
            TakeGold();

        ItemData[] itemsTaken = new ItemData[invUI.currentlyActiveContainer.containerObjects.Count];
        for (int i = 0; i < invUI.currentlyActiveContainer.containerObjects.Count; i++)
        {
            ItemData objItemData = invUI.currentlyActiveContainer.containerObjects[i].GetComponent<ItemData>();
            if (AddToInventory(objItemData.item, objItemData) == false)
            {
                Debug.Log("Not enough room in your inventory for all of the items in the container.");
                break;
            }
            else
                itemsTaken[i] = objItemData;
        }

        foreach (ItemData itemData in itemsTaken)
        {
            if (itemData != null)
            {
                foreach (GameObject obj in invUI.currentlyActiveContainer.containerObjects)
                {
                    ItemData objItemData = obj.GetComponent<ItemData>();
                    foreach (InventorySlot invSlot in invUI.containerSlots)
                    {
                        if (invSlot.itemData == objItemData)
                        {
                            invSlot.ClearSlot();
                            break;
                        }
                    }

                    if (objItemData == itemData)
                    {
                        invUI.currentlyActiveContainer.containerItems.Remove(objItemData);
                        invUI.currentlyActiveContainer.containerObjects.Remove(obj);
                        break;
                    }
                }
            }
        }
    }

    public Vector3 GetItemInvPositionOffset(Item item)
    {
        float addWidth = 0;
        float addHeight = 0;
        if (item.iconWidth == 2)
            addWidth = -37.5f;
        else if (item.iconWidth == 3)
            addWidth = -75f;
        if (item.iconHeight == 2)
            addHeight = 37.5f;
        else if (item.iconHeight == 3)
            addHeight = 75f;

        return new Vector3(addWidth, addHeight, 0);
    }
}
