using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public string containerName = "Container";

    [Header("Contents")]
    public List<GameObject> containerObjects = new List<GameObject>();
    public List<ItemData> containerItems = new List<ItemData>();

    [Header("Slots")]
    //public GameObject containerPrefab;
    public Transform slotsParent;
    public int slotCount = 30;

    InventoryUI invUI;
    Inventory inv;

    Item itemToAdd;
    bool inContainerRange;

    void Start()
    {
        invUI = InventoryUI.instance;
        inv = Inventory.instance;
        
        for (int i = 0; i < containerObjects.Count; i++)
        {
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.AddComponent<ItemData>();
            ItemData itemData = newObj.GetComponent<ItemData>();
            itemData.TransferData(containerObjects[i].GetComponent<ItemData>(), itemData);
            newObj.name = itemData.itemName;
            containerObjects[i] = newObj;
        }
    }

    void Update()
    {
        if (inContainerRange && GameControls.gamePlayActions.playerInteract.WasPressed)
        {
            if (invUI.containerMenuGO.activeSelf == false) // If the container menu is not open
            {
                if (invUI.currentlyActiveContainer == null || invUI.currentlyActiveContainer != this)
                {
                    // Create the container's slots if need be
                    if (slotCount != invUI.containerSlots.Count)
                    {
                        invUI.containerSlots.Clear();

                        foreach(InventorySlot slot in invUI.containerMenuGO.GetComponentsInChildren<InventorySlot>())
                        {
                            Destroy(slot.gameObject);
                        }

                        StartCoroutine(CreateSlots());
                        // Delay adding the items so that each slot has time to run its Awake()
                        StartCoroutine(DelayAddContainerItems());
                    }
                    else // Otherwise, don't create any slots and add the items without a delay
                        AddContainerItems();

                    // Set this container to be the currently active container so we can easily reference it in other scripts
                    invUI.currentlyActiveContainer = this;
                }

                OpenMenus();
            }
            else // If the container menu is already open, close it
                StartCoroutine(CloseMenus());
        }
    }

    void AddContainerItems()
    {
        ClearContainerSlots(); // Clear out the slots in the case we had a different container open previously

        if (containerItems.Count > 0)
            containerItems.Clear();

        // Place each item in the slot
        for (int i = 0; i < containerObjects.Count; i++)
        {
            // Set the item data for use in the CalculateItemInvPositionFromPickup function below
            ItemData itemData = containerObjects[i].GetComponent<ItemData>();
            itemToAdd = itemData.item;

            // Add each object to the container's slots
            inv.CalculateItemInvPositionFromPickup(itemToAdd, itemData, invUI.containerSlots, containerItems);
        }

        for (int i = containerObjects.Count - 1; i > 0; i--)
        {
            ItemData itemData = containerObjects[i].GetComponent<ItemData>();
            if (itemData.currentStackSize <= 0)
            {
                invUI.currentlyActiveContainer.containerObjects.Remove(itemData.gameObject);
                invUI.currentlyActiveContainer.containerItems.Remove(itemData);
                Destroy(itemData.gameObject);
            }
        }
    }

    IEnumerator CreateSlots()
    {
        yield return new WaitForSeconds(0.025f);
        invUI.CreateSlots(slotCount, invUI.containerParent, invUI.containerSlots, true);
        foreach (InventorySlot slot in invUI.containerParent.GetComponentsInChildren<InventorySlot>())
            invUI.containerSlots.Add(slot);
    }

    IEnumerator DelayAddContainerItems()
    {
        yield return new WaitForSeconds(0.05f);
        AddContainerItems();
    }

    void ClearContainerSlots()
    {
        foreach (InventorySlot slot in invUI.containerSlots)
        {
            if (slot.itemData != null)
                slot.ClearSlot();
        }
    }

    void OpenMenus()
    {
        if (invUI.containerMenuGO.activeSelf == false)
            invUI.ToggleContainerMenu();
        if (invUI.inventoryGO.activeSelf == false)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenuGO.activeSelf == false)
            invUI.ToggleEquipmentMenu();
    }

    IEnumerator CloseMenus()
    {
        if (invUI.pocketsParent.childCount > 0)
            invUI.pocketsSlots[0].GetComponentInChildren<ContextMenu>().DisableContextMenu();
        if (invUI.equipTooltip1.gameObject.activeSelf == true)
            invUI.equipTooltip1.ClearTooltip();
        if (invUI.equipTooltip2.gameObject.activeSelf == true)
            invUI.equipTooltip2.ClearTooltip();
        if (invUI.invTooltip.gameObject.activeSelf == true)
            invUI.invTooltip.ClearTooltip();

        yield return new WaitForSeconds(0.15f);

        invUI.TurnOffHighlighting();
        if (invUI.containerMenuGO.activeSelf == true)
            invUI.ToggleContainerMenu();
        if (invUI.inventoryGO.activeSelf == true)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenuGO.activeSelf == true)
            invUI.ToggleEquipmentMenu();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            inContainerRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inContainerRange = false;

            if (invUI.currentlySelectedItem != null)
            {
                if (invUI.invSlotMovingFrom != null)
                {
                    invUI.invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                    invUI.invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
                }
                else if (invUI.equipSlotMovingFrom != null)
                {
                    invUI.equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                    invUI.equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
                }

                invUI.StopDraggingInvItem();
            }

            StartCoroutine(CloseMenus());
        }
    }
}
