using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public string containerName = "Container";
    public bool randomizeItems = true;
    public Transform itemsParent;

    [Header("Gold")]
    public bool randomizeGold = true;
    public int percentChanceNoGold = 20;
    public int minGoldPossible = 1;
    public int maxGoldPossible = 10;
    public int gold;

    [Header("Contents")]
    public List<GameObject> containerObjects = new List<GameObject>();
    public List<ItemData> containerItems = new List<ItemData>();

    [Header("Slots")]
    public Transform slotsParent;
    public int slotCount = 30;

    InventoryUI invUI;
    Inventory inv;
    AudioManager audioManager;
    GameManager gm;

    Item itemToAdd;
    bool inContainerRange;

    void Start()
    {
        invUI = InventoryUI.instance;
        inv = Inventory.instance;
        audioManager = AudioManager.instance;
        gm = GameManager.instance;
        itemsParent = transform.Find("Items");

        InitializeData();
    }

    void Update()
    {
        if (inContainerRange && GameControls.gamePlayActions.playerInteract.WasPressed)
        {
            if (invUI.containerMenu.activeSelf == false) // If the container menu is not open
            {
                if (invUI.currentlyActiveContainer == null || invUI.currentlyActiveContainer != this)
                {
                    // Create the container's slots if need be
                    if (slotCount != invUI.containerSlots.Count)
                    {
                        invUI.containerSlots.Clear();

                        foreach (InventorySlot slot in invUI.containerMenu.GetComponentsInChildren<InventorySlot>())
                        {
                            Destroy(slot.gameObject);
                        }

                        StartCoroutine(CreateSlots());

                        // Delay adding the items so that each slot has time to run its Awake()
                        StartCoroutine(DelayAddContainerItems());
                    }
                    else // Otherwise, don't create any slots and add the items without a delay
                        AddContainerItems();

                    invUI.containerMenuGoldText.text = gold.ToString();

                    // Set this container to be the currently active container so we can easily reference it in other scripts
                    invUI.currentlyActiveContainer = this;
                }

                if (transform.parent.name == "Containers")
                    audioManager.PlayRandomSound(audioManager.openDoorSounds, transform.position);
                else if (transform.parent.name == "NPCs")
                    audioManager.PlayRandomSound(audioManager.searchBodySounds, transform.position);

                OpenMenus();
            }
        }
    }

    void RandomizeGold()
    {
        int randomNum = Random.Range(1, 101);
        if (randomNum <= percentChanceNoGold) // Chance for 0 gold
            gold = 0;
        else
            gold = Random.Range(minGoldPossible, maxGoldPossible + 1);
    }

    public void InitializeData()
    {
        for (int i = 0; i < containerObjects.Count; i++)
        {
            GameObject hierarchyObj = new GameObject();
            hierarchyObj.transform.SetParent(itemsParent);
            hierarchyObj.AddComponent<ItemData>();

            ItemData hierarchyObjItemData = hierarchyObj.GetComponent<ItemData>();
            ItemData containerObjItemData = containerObjects[i].GetComponent<ItemData>();

            StartCoroutine(SetupContainerObjects(hierarchyObj, containerObjects[i], hierarchyObjItemData, containerObjItemData));
            StartCoroutine(SetLists());
        }

        if (randomizeGold)
            RandomizeGold();

        randomizeItems = false;
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

        if (gm.isUsingController)
        {
            UIControllerNavigation.instance.FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.containerSlots), 0, 0);
            UIControllerNavigation.instance.currentXCoord = 1;
            UIControllerNavigation.instance.currentOverallYCoord = 1;
        }
    }

    IEnumerator SetupContainerObjects(GameObject hierarchyObj, GameObject containerObj, ItemData hierarchyObjItemData, ItemData containerObjItemData)
    {
        yield return new WaitForSeconds(0.1f);
        
        hierarchyObjItemData.item = containerObjItemData.item;
        hierarchyObjItemData.equipment = containerObjItemData.equipment;
        hierarchyObjItemData.consumable = containerObjItemData.consumable;

        if (containerObjItemData.hasBeenRandomized == false)
            hierarchyObjItemData.RandomizeData();
        else
            containerObjItemData.TransferData(containerObjItemData, hierarchyObjItemData);

        hierarchyObj.name = hierarchyObjItemData.itemName;
    }

    IEnumerator SetLists()
    {
        yield return new WaitForSeconds(0.15f);

        containerObjects.Clear();
        containerItems.Clear();

        for (int i = 0; i < itemsParent.childCount; i++)
        {
            containerObjects.Add(itemsParent.GetChild(i).gameObject);
            containerItems.Add(itemsParent.GetChild(i).GetComponent<ItemData>());
        }
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
        if (invUI.containerMenu.activeSelf == false)
            invUI.ToggleContainerMenu();
        if (invUI.inventoryMenu.activeSelf == false)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenu.activeSelf == false)
            invUI.ToggleEquipmentMenu();

        if (gm.isUsingController)
            UIControllerNavigation.instance.FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.containerSlots), 0, 0);
    }

    IEnumerator CloseMenu()
    {
        UIControllerNavigation.instance.DisableContextMenu();
        invUI.ClearAllTooltips();

        yield return new WaitForSeconds(0.15f);

        invUI.TurnOffHighlighting();
        if (invUI.containerMenu.activeSelf)
            invUI.ToggleContainerMenu();

        if (gm.isUsingController)
            UIControllerNavigation.instance.ClearCurrentlySelected();
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

            if (invUI.containerMenu.activeSelf == true)
                StartCoroutine(CloseMenu());
        }
    }
}
