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

    public List<Item> pocketItems   = new List<Item>();
    public List<Item> bagItems      = new List<Item>();
    public List<Item> horseBagItems = new List<Item>();

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

    public bool AddToPockets(Item itemToAdd)
    {
        if (itemToAdd.isPickupable)
        {
            if (pocketItems.Count >= pocketsSlotCount) // Only ever used if your inventory is completely full of items 1x1 in size
            {
                Debug.Log("Not enough room in pockets.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.pocketsSlots, pocketItems) == true)
                return true;
        }

        // Return false if we can't find room for our item (it probably is too large)
        Debug.Log("Not enough room in our pockets (or item is not pickupable.");
        return false;
    }

    public bool AddToBag(Item itemToAdd)
    {
        if (itemToAdd.isPickupable == false)
        {
            if (bagItems.Count >= bagSlotCount)
            {
                Debug.Log("Not enough room in bag.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.bagSlots, bagItems) == true)
                return true;
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToBag code. We should either return true or false before this point.");
        return false;
    }

    public bool AddToHorseBag(Item itemToAdd)
    {
        if (itemToAdd.isPickupable == false)
        {
            if (horseBagItems.Count >= horseBagSlotCount)
            {
                Debug.Log("Not enough room in horse bags.");
                return false;
            }

            if (CalculateItemInvPositionFromPickup(itemToAdd, invUI.horseBagSlots, horseBagItems) == true)
                return true;
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToHorseBag code. We should either return true or false before this point.");
        return false;
    }

    public bool CalculateItemInvPositionFromPickup(Item itemToAdd, InventorySlot[] invSlots, List<Item> itemsList)
    {
        int totalSlotsToCheck = (itemToAdd.iconWidth * itemToAdd.iconHeight) - 1;
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];
        int currentSlotsToFillIndex = 0;

        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i].isEmpty)
            {
                foreach (InventorySlot slot in invSlots)
                {
                    if ((invSlots[i].slotCoordinate.x + itemToAdd.iconWidth - 1 == slot.slotCoordinate.x
                        && invSlots[i].slotCoordinate.y + itemToAdd.iconHeight - 1 == slot.slotCoordinate.y)
                        ||
                        (itemToAdd.iconWidth > 1 && invSlots[i].slotCoordinate.x + 1 == slot.slotCoordinate.x
                        && invSlots[i].slotCoordinate.y == slot.slotCoordinate.y)
                        ||
                        (itemToAdd.iconHeight > 1 && invSlots[i].slotCoordinate.y + 1 == slot.slotCoordinate.y
                        && invSlots[i].slotCoordinate.x == slot.slotCoordinate.x)) // If the slot we're checking's x pos = the x pos of the slot below us
                    {
                        if (slot.isEmpty)
                        {
                            slotsToFill[currentSlotsToFillIndex] = slot;
                            currentSlotsToFillIndex++;
                        }
                    }
                }

                if (slotsToFill.Length == totalSlotsToCheck) // We found a valid slot to put our item!
                {
                    invSlots[i].AddItem(itemToAdd);
                    for (int j = 0; j < slotsToFill.Length; j++)
                    {
                        invSlots[i].childrenSlots[j] = slotsToFill[j];
                        slotsToFill[j].isEmpty = false;
                        slotsToFill[j].slotBackgroundImage.sprite = slotsToFill[j].fullSlotSprite;
                        slotsToFill[j].parentSlot = invSlots[i];
                    }

                    itemsList.Add(itemToAdd);
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

    public bool DetermineIfValidInventoryPosition(Item itemToAdd, InventorySlot startSlot, InventorySlot[] invSlots, List<Item> itemsList)
    {
        int totalSlotsToCheck = itemToAdd.iconWidth * itemToAdd.iconHeight;
        InventorySlot[] slotsToFill = new InventorySlot[totalSlotsToCheck];
        int currentSlotsToFillIndex = 0;

        for (int x = 0; x < itemToAdd.iconWidth; x++)
        {
            for (int y = 0; y < itemToAdd.iconHeight; y++)
            {
                InventorySlot slotToCheck = GetSlotByCoordinates(new Vector2(startSlot.slotCoordinate.x + x, startSlot.slotCoordinate.y + y), invSlots);
                if (slotToCheck == null)
                {
                    Debug.Log("You're trying to place item in an invalid position.");
                    return false;
                }
                else
                {
                    slotsToFill[currentSlotsToFillIndex] = slotToCheck;
                    currentSlotsToFillIndex++;
                }
            }
        } 

        InventorySlot[] parentSlotsTryingToReplace = new InventorySlot[2];
        int itemsTryingToReplaceCount = 0;
        foreach (InventorySlot slot in slotsToFill)
        {
            // Debug.Log(slot.slotCoordinate);
            if (itemsTryingToReplaceCount <= 1) // If we're trying to replace more than one item, then this position is invalid and we will return false
            {
                if (slot.isEmpty == false)
                {
                    if (slot.parentSlot != null)
                    {
                        Debug.Log("Going through slots to fill");
                        if (itemsTryingToReplaceCount > 0 && slot.parentSlot != parentSlotsTryingToReplace[0]) // If we already found one parent
                        {
                            parentSlotsTryingToReplace[1] = slot.parentSlot;
                            itemsTryingToReplaceCount++;
                        }
                        else if (itemsTryingToReplaceCount == 0)
                        {
                            parentSlotsTryingToReplace[0] = slot.parentSlot;
                            itemsTryingToReplaceCount++;
                        }
                    }
                    else // If this is a parent slot
                    {
                        Debug.Log("Going through slots to fill");
                        if (itemsTryingToReplaceCount > 0 && slot != parentSlotsTryingToReplace[0])
                        {
                            parentSlotsTryingToReplace[1] = slot;
                            itemsTryingToReplaceCount++;
                        }
                        else if (itemsTryingToReplaceCount == 0)
                        {
                            parentSlotsTryingToReplace[0] = slot;
                            itemsTryingToReplaceCount++;
                        }
                    }
                }
            }
            else
                return false;
        }

        if (itemsTryingToReplaceCount == 0)
        {
            startSlot.AddItem(itemToAdd);

            InventorySlot[] inventorySlots = new InventorySlot[itemToAdd.iconWidth * itemToAdd.iconHeight];
            InventorySlot[] childSlots = new InventorySlot[(itemToAdd.iconWidth * itemToAdd.iconHeight) - 1];
            int invSlotCount = 0;
            int childSlotCount = 0;
            for (int x = 0; x < itemToAdd.iconWidth; x++)
            {
                for (int y = 0; y < itemToAdd.iconHeight; y++)
                {
                    inventorySlots[invSlotCount] = GetSlotByCoordinates(new Vector2(startSlot.slotCoordinate.x + x, startSlot.slotCoordinate.y + y), invSlots);
                    inventorySlots[invSlotCount].isEmpty = false;
                    inventorySlots[invSlotCount].slotBackgroundImage.sprite = inventorySlots[invSlotCount].fullSlotSprite;

                    if (inventorySlots[invSlotCount] != startSlot)
                    {
                        inventorySlots[invSlotCount].parentSlot = startSlot;
                        childSlots[childSlotCount] = inventorySlots[invSlotCount];
                        childSlotCount++;
                    }

                    invSlotCount++;
                }
            }

            for (int i = 0; i < childSlots.Length; i++)
            {
                foreach (InventorySlot slot in inventorySlots)
                {
                    if (slot == startSlot)
                    {
                        slot.childrenSlots[i] = childSlots[i];
                        break;
                    }
                }
            }

            for (int i = 0; i < invUI.movingFromSlot.childrenSlots.Length; i++)
            {
                if (invUI.movingFromSlot.childrenSlots[i] != null)
                {
                    invUI.movingFromSlot.childrenSlots[i].parentSlot = null;
                    invUI.movingFromSlot.childrenSlots[i] = null;
                }
            }

            invUI.movingFromSlot.ClearSlot();
            invUI.movingFromSlot = null;
            invUI.currentlySelectedItem = null;
        }

        if (itemsTryingToReplaceCount == 1)
        {
            Debug.Log("Adding to or updating slot...");
            if (startSlot.item != null)
                startSlot.UpdateSlot(itemToAdd);
            else
                startSlot.AddItem(itemToAdd);

            invUI.currentlySelectedItem = parentSlotsTryingToReplace[0].item;

            foreach (InventorySlot slot in slotsToFill)
            {
                if (slot.isEmpty == false && slot.parentSlot != null)
                {
                    foreach (InventorySlot childSlot in slot.parentSlot.childrenSlots)
                    {
                        if (childSlot != null)
                        {
                            foreach (InventorySlot parentsChildSlot in childSlot.parentSlot.childrenSlots)
                            {
                                if (parentsChildSlot != null)
                                    parentsChildSlot.SoftClearSlot(parentsChildSlot);
                            }

                            invUI.movingFromSlot = childSlot.parentSlot;
                        }
                    }
                }
            }
        }

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

    public void Remove(Item item)
    {
        if (bagItems.Contains(item))
            bagItems.Remove(item);
        else if (pocketItems.Contains(item))
            pocketItems.Remove(item);
        else if (horseBagItems.Contains(item))
            horseBagItems.Remove(item);
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
}
