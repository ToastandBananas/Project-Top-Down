using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    InventoryUI inventoryUI;

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
        inventoryUI = InventoryUI.instance;
    }

    public bool AddToPockets(Item item)
    {
        if (item.isDefaultItem == false)
        {
            if (pocketItems.Count >= pocketsSlotCount)
            {
                Debug.Log("Not enough room in pockets.");
                return false;
            }
            
            pocketItems.Add(item);

            for (int i = 0; i < inventoryUI.pocketsSlots.Length; i++)
            {
                if (inventoryUI.pocketsSlots[i].iconImage == null)
                {
                    inventoryUI.pocketsSlots[i].AddItem(item);
                    return true;
                }
            }
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToPockets code. We should either return true or false before this point.");
        return false;
    }

    public bool AddToBag(Item item)
    {
        if (item.isDefaultItem == false)
        {
            if (bagItems.Count >= bagSlotCount)
            {
                Debug.Log("Not enough room in bag.");
                return false;
            }

            bagItems.Add(item);

            for (int i = 0; i < inventoryUI.bagSlots.Length; i++)
            {
                if (inventoryUI.bagSlots[i].iconImage == null)
                {
                    inventoryUI.bagSlots[i].AddItem(item);
                    return true;
                }
            }
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToBag code. We should either return true or false before this point.");
        return false;
    }

    public bool AddToHorseBag(Item item)
    {
        if (item.isDefaultItem == false)
        {
            if (horseBagItems.Count >= horseBagSlotCount)
            {
                Debug.Log("Not enough room in horse bags.");
                return false;
            }

            horseBagItems.Add(item);

            for (int i = 0; i < inventoryUI.horseBagSlots.Length; i++)
            {
                if (inventoryUI.horseBagSlots[i].iconImage == null)
                {
                    inventoryUI.horseBagSlots[i].AddItem(item);
                    return true;
                }
            }
        }

        // We should never get here...if we do there's an error in our code
        Debug.LogWarning("Warning: Something is wrong with our AddToHorseBag code. We should either return true or false before this point.");
        return false;
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
