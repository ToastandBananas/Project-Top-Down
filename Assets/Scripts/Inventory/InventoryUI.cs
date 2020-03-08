using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public GameObject slotPrefab;
    public Transform menusParent;
    public GameObject inventoryGO;
    public GameObject playerEquipmentMenuGO;
    public GameObject containerMenuGO;
    public GameObject floatingTextPrefab;

    [Header("Parents")]
    public Transform invItemsParent;
    public Transform containerItemsParent;
    public Transform pocketsParent;
    public Transform bagParent;
    public Transform horseBagParent;
    public Transform containerParent;

    [Header("Titles")]
    public Transform pocketsTitle;
    public Transform bagTitle;
    public Transform horseBagTitle;

    [Header("Inventory Slots")]
    public InventorySlot tempSlot;
    public List<InventorySlot> pocketsSlots = new List<InventorySlot>();
    public List<InventorySlot> bagSlots = new List<InventorySlot>();
    public List<InventorySlot> horseBagSlots = new List<InventorySlot>();
    public List<InventorySlot> containerSlots = new List<InventorySlot>();

    [Header("Equip Slots")]
    public EquipSlot[] weaponSlots = new EquipSlot[3];
    public EquipSlot leftWeaponSlot = null;
    public EquipSlot rightWeaponSlot = null;
    public EquipSlot rangedWeaponSlot = null;
    public EquipSlot[] equipSlots;

    [Header("Selected Item Info")]
    [HideInInspector] public Item currentlySelectedItem;
    public ItemData currentlySelectedItemData;
    [HideInInspector] public InventorySlot invSlotMovingFrom;
    [HideInInspector] public EquipSlot equipSlotMovingFrom;
    [HideInInspector] public Container currentlyActiveContainer;

    [Header("Tooltips")]
    public Tooltip invTooltip;
    public Tooltip equipTooltip1;
    public Tooltip equipTooltip2;

    int maxInventoryWidth = 8;
    int maxContainerWidth = 6;

    Inventory inventory;
    PlayerMovement player;

    void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one InventoryUI script is active.");
            Destroy(gameObject);
        }
        #endregion

        inventory = Inventory.instance;
        player = PlayerMovement.instance;

        CreateSlots(inventory.pocketsSlotCount, pocketsParent, pocketsSlots, false);
        CreateSlots(inventory.bagSlotCount, bagParent, bagSlots, false);
        CreateSlots(inventory.horseBagSlotCount, horseBagParent, horseBagSlots, false);

        tempSlot = Instantiate(slotPrefab, transform).GetComponent<InventorySlot>();
        tempSlot.transform.localPosition += new Vector3(10000f, 10000f, 0);
        tempSlot.name = "Temp Slot";

        foreach(InventorySlot slot in pocketsParent.GetComponentsInChildren<InventorySlot>())
        {
            pocketsSlots.Add(slot);
        }
        foreach (InventorySlot slot in bagParent.GetComponentsInChildren<InventorySlot>())
        {
            bagSlots.Add(slot);
        }
        foreach (InventorySlot slot in horseBagParent.GetComponentsInChildren<InventorySlot>())
        {
            horseBagSlots.Add(slot);
        }

        weaponSlots = GameObject.Find("Weapons").GetComponentsInChildren<EquipSlot>();
        foreach (EquipSlot equipSlot in weaponSlots)
        {
            if (equipSlot.thisWeaponSlot == WeaponSlot.WeaponLeft)
                leftWeaponSlot = equipSlot;
            if (equipSlot.thisWeaponSlot == WeaponSlot.WeaponRight)
                rightWeaponSlot = equipSlot;
            if (equipSlot.thisWeaponSlot == WeaponSlot.Ranged)
                rangedWeaponSlot = equipSlot;
        }

        equipSlots = GameObject.Find("Equipment").GetComponentsInChildren<EquipSlot>();

        if (player.isMounted == false)
            ShowHorseSlots(false);
    }

    void Update()
    {
        if (GameControls.gamePlayActions.playerInventory.WasPressed)
        {
            if (currentlySelectedItem != null)
            {
                if (invSlotMovingFrom != null)
                {
                    invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                    invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
                }
                else if (equipSlotMovingFrom != null)
                {
                    equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                    equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
                }

                StopDraggingInvItem();
            }

            StartCoroutine(ToggleMenus());
        }

        // If we have a selected item and we click outside of a menu, drop the item
        if (GameControls.gamePlayActions.menuSelect.WasPressed && currentlySelectedItem != null && EventSystem.current.currentSelectedGameObject == null)
        {
            if (invSlotMovingFrom != null)
            {
                invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
            }
            else if (equipSlotMovingFrom != null)
            {
                equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
                equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DisableContextMenu();
            }
            
            StopDraggingInvItem();
        }
    }

    public void CreateSlots(int slotCount, Transform slotsParent, List<InventorySlot> slots, bool isContainer)
    {
        //if (slots.Length != slotCount)
            //slots = new InventorySlot[slotCount]; // Reset our slots array to the appropriate size

        int currentXCoord = 1;
        int currentYCoord = 1;
        for (int i = 1; i < slotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if ((isContainer == false && currentXCoord == maxInventoryWidth + 1) || (isContainer && currentXCoord == maxContainerWidth + 1))
            {
                currentXCoord = 1;
                currentYCoord++;
            }

            slot.GetComponent<InventorySlot>().slotParent = slotsParent;
            if (slotsParent == pocketsParent)
                slot.name = "Pocket Slot " + i;
            else if (slotsParent == bagParent)
                slot.name = "Bag Slot " + i;
            else if (slotsParent == horseBagParent)
                slot.name = "Horse Bag Slot " + i;
            else if (slotsParent == containerParent)
                slot.name = "Container Slot " + i;
        }

        if (isContainer)
            StartCoroutine(CalculateItemsParentHeight(containerItemsParent));
        else
            StartCoroutine(CalculateItemsParentHeight(invItemsParent));
    }

    public void StopDraggingInvItem()
    {
        currentlySelectedItem = null;
        currentlySelectedItemData = null;
        invSlotMovingFrom = null;
        equipSlotMovingFrom = null;
        Cursor.visible = true;
    }

    public void ShowHorseSlots(bool showSlots)
    {
        horseBagParent.gameObject.SetActive(showSlots);
        horseBagTitle.gameObject.SetActive(showSlots);
    }

    public void ToggleInventory()
    {
        inventoryGO.SetActive(!inventoryGO.activeSelf);
    }

    public void ToggleEquipmentMenu()
    {
        playerEquipmentMenuGO.SetActive(!playerEquipmentMenuGO.activeSelf);
    }

    public void ToggleContainerMenu()
    {
        containerMenuGO.SetActive(!containerMenuGO.activeSelf);
    }

    public void TurnOffHighlighting()
    {
        foreach(InventorySlot slot in pocketsSlots)
        {
            slot.slotBackgroundImage.color = Color.white;
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }
        foreach (InventorySlot slot in bagSlots)
        {
            slot.slotBackgroundImage.color = Color.white;
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }
        foreach (InventorySlot slot in horseBagSlots)
        {
            slot.slotBackgroundImage.color = Color.white;
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }
        foreach (InventorySlot slot in containerSlots)
        {
            slot.slotBackgroundImage.color = Color.white;
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }
    }

    IEnumerator ToggleMenus()
    {
        yield return new WaitForSeconds(0.15f);
        if (inventoryGO.activeSelf == true)
            TurnOffHighlighting();
        ToggleInventory();
        ToggleEquipmentMenu();
        if (containerMenuGO.activeSelf == true)
            ToggleContainerMenu();
    }

    IEnumerator CalculateItemsParentHeight(Transform itemsParent)
    {
        yield return new WaitForSeconds(0.1f);

        int heightAddOn = 0;

        if (itemsParent == containerItemsParent)
        {
            heightAddOn += 50; // For the title
            if ((containerSlots.Count % maxContainerWidth) > 0)
                heightAddOn += 75;
            heightAddOn += ((containerSlots.Count / maxContainerWidth) * 75);
            itemsParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsParent.GetComponent<RectTransform>().sizeDelta.x, heightAddOn);
        }
        else // if itemsParent == invItemsParent
        {
            if (pocketsParent.gameObject.activeSelf == true)
            {
                heightAddOn += 50; // For the title
                if ((pocketsSlots.Count % maxInventoryWidth) > 0)
                    heightAddOn += 75;
                heightAddOn += ((pocketsSlots.Count / maxInventoryWidth) * 75);
            }
            if (bagParent.gameObject.activeSelf == true)
            {
                heightAddOn += 50; // For the title
                if ((bagSlots.Count % maxInventoryWidth) > 0)
                    heightAddOn += 75;
                heightAddOn += ((bagSlots.Count / maxInventoryWidth) * 75);
            }
            if (horseBagParent.gameObject.activeSelf == true)
            {
                heightAddOn += 50; // For the title
                if ((horseBagSlots.Count % maxInventoryWidth) > 0)
                    heightAddOn += 75;
                heightAddOn += ((horseBagSlots.Count / maxInventoryWidth) * 75);
            }

            itemsParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsParent.GetComponent<RectTransform>().sizeDelta.x, heightAddOn);
        }
    }
}
