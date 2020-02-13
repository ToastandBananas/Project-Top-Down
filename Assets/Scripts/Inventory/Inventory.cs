using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int pocketsSlotCount = 10;
    public int bagSlotCount = 10;
    public int horseSlotCount = 10;

    public List<Item> pocketItems = new List<Item>();
    public List<Item> bagItems    = new List<Item>();
    public List<Item> horseItems  = new List<Item>();

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

            // Sort items alphabetically
            pocketItems.Sort
            (
                delegate (Item i1, Item i2)
                {
                    return i1.name.CompareTo(i2.name);
                }
            );

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }

        return true;
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

            // Sort items alphabetically
            bagItems.Sort
            (
                delegate (Item i1, Item i2)
                {
                    return i1.name.CompareTo(i2.name);
                }
            );

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }

        return true;
    }

    public bool AddToHorseBag(Item item)
    {
        if (item.isDefaultItem == false)
        {
            if (horseItems.Count >= horseSlotCount)
            {
                Debug.Log("Not enough room in horses bags.");
                return false;
            }

            horseItems.Add(item);

            // Sort items alphabetically
            horseItems.Sort
            (
                delegate (Item i1, Item i2)
                {
                    return i1.name.CompareTo(i2.name);
                }
            );

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }

        return true;
    }

    public void Remove(Item item)
    {
        if (bagItems.Contains(item))
            bagItems.Remove(item);
        else if (pocketItems.Contains(item))
            pocketItems.Remove(item);
        else if (horseItems.Contains(item))
            horseItems.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
