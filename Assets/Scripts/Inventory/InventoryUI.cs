using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    
    [Header("GameObject References")]
    public GameObject inventoryMenu;
    public GameObject playerEquipmentMenu;
    public GameObject containerMenu;
    public Text playerGoldText;
    public Text containerMenuGoldText;
    public QuantityMenu quantityMenu;

    [Header("Prefabs")]
    public GameObject slotPrefab;
    public GameObject floatingTextPrefab;

    [Header("Parents")]
    public Transform menusParent;
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
    [HideInInspector] public int maxInventoryWidth = 8;
    [HideInInspector] public int maxContainerWidth = 6;
    [HideInInspector] public int overallInventoryHeight = 0;
    [HideInInspector] public int maxOverallInventoryHeight = 12;
    [HideInInspector] public int pocketsHeight;
    [HideInInspector] public int bagHeight;
    [HideInInspector] public int horseBagHeight;
    [HideInInspector] public int containerHeight;

    [Header("Equip Slots")]
    [HideInInspector] public EquipSlot[] weaponSlots = new EquipSlot[3];
    public EquipSlot leftWeaponSlot = null;
    public EquipSlot rightWeaponSlot = null;
    public EquipSlot rangedWeaponSlot = null;
    public EquipSlot[] equipSlots;

    [Header("Selected Item Info")]
    [HideInInspector] public Item currentlySelectedItem;
    [HideInInspector] public ItemData currentlySelectedItemData;
    [HideInInspector] public InventorySlot invSlotMovingFrom;
    [HideInInspector] public EquipSlot equipSlotMovingFrom;
    [HideInInspector] public Container currentlyActiveContainer;

    [Header("Tooltips")]
    public Tooltip invTooltip;
    public Tooltip equipTooltip1;
    public Tooltip equipTooltip2;

    Inventory inv;
    PlayerMovement playerMovement;
    GameManager gm;

    void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one InventoryUI script is active.");
            Destroy(this);
        }
        #endregion

        inventoryMenu.SetActive(true);
        playerEquipmentMenu.SetActive(true);

        inv = Inventory.instance;

        if (GameManager.instance != null && GameManager.instance.GetComponent<SaveLoad>().isLoading == false)
        {
            CreateSlots(inv.pocketsSlotCount, pocketsParent, pocketsSlots, false);
            CreateSlots(inv.bagSlotCount, bagParent, bagSlots, false);
            CreateSlots(inv.horseBagSlotCount, horseBagParent, horseBagSlots, false);

            CreateTempSlot();

            SetInventorySlotLists();
        }

        SetEquipmentSlotLists();

        inventoryMenu.SetActive(false);
        playerEquipmentMenu.SetActive(false);
    }

    void Start()
    {
        gm = GameManager.instance;
        playerMovement = PlayerMovement.instance;
        if (playerMovement.isMounted == false)
            ShowHorseSlots(false);
    }

    void Update()
    {
        // If the inventory button/key is pressed
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

            if (gm.pauseMenu.activeSelf == true)
                gm.TogglePauseMenu();

            StartCoroutine(ToggleMenus());
        }

        // If we have a selected item and we click outside of a menu, drop the item
        if (GameControls.gamePlayActions.menuSelect.WasPressed && currentlySelectedItem != null && EventSystem.current.currentSelectedGameObject == null)
        {
            if (invSlotMovingFrom != null)
            {
                if (invSlotMovingFrom.slotParent == containerParent)
                {
                    for (int i = 0; i < currentlyActiveContainer.itemsParent.childCount; i++)
                    {
                        if (currentlyActiveContainer.itemsParent.GetChild(i).GetComponent<ItemData>() == currentlySelectedItemData)
                            Destroy(currentlyActiveContainer.itemsParent.GetChild(i).gameObject);
                    }
                }

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
        if (slotsParent == pocketsParent)
            pocketsHeight = 0;
        else if (slotsParent == bagParent)
            bagHeight = 0;
        else if (slotsParent == horseBagParent)
            horseBagHeight = 0;
        else if (slotsParent == containerParent)
            containerHeight = 0;

        int currentXCoord = 1;
        int currentYCoord = 1;
        overallInventoryHeight++;

        for (int i = 1; i < slotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if ((isContainer == false && currentXCoord == maxInventoryWidth + 1) || (isContainer && currentXCoord == maxContainerWidth + 1))
            {
                currentXCoord = 1;
                currentYCoord++;

                if (slotsParent == pocketsParent)
                    pocketsHeight = currentYCoord;
                else if (slotsParent == bagParent)
                    bagHeight = currentYCoord;
                else if (slotsParent == horseBagParent)
                    horseBagHeight = currentYCoord;
                else if (slotsParent == containerParent)
                    containerHeight = currentYCoord;

                overallInventoryHeight++;
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

        if (slotsParent == pocketsParent)
        {
            if (slotCount % maxInventoryWidth == 0)
                pocketsHeight--;
        }
        else if (slotsParent == bagParent)
        {
            if (slotCount % maxInventoryWidth == 0)
                bagHeight--;
        }
        else if (slotsParent == horseBagParent)
        {
            if (slotCount % maxInventoryWidth == 0)
                horseBagHeight--;
        }
        else if (slotsParent == containerParent)
        {
            if (slotCount % maxInventoryWidth == 0)
                containerHeight--;
        }

        if (isContainer)
            StartCoroutine(CalculateItemsParentHeight(containerItemsParent));
        else
            StartCoroutine(CalculateItemsParentHeight(invItemsParent));
    }

    void CreateTempSlot()
    {
        tempSlot = Instantiate(slotPrefab, transform).GetComponent<InventorySlot>();
        tempSlot.transform.localPosition += new Vector3(10000f, 10000f, 0);
        tempSlot.name = "Temp Slot";
    }

    public void SetInventorySlotLists()
    {
        foreach (InventorySlot slot in pocketsParent.GetComponentsInChildren<InventorySlot>())
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
    }

    void SetEquipmentSlotLists()
    {
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
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        if (inventoryMenu.activeSelf)
            gm.menuOpen = true;
        else
            gm.menuOpen = false;
    }

    public void ToggleEquipmentMenu()
    {
        playerEquipmentMenu.SetActive(!playerEquipmentMenu.activeSelf);
    }

    public void ToggleContainerMenu()
    {
        containerMenu.SetActive(!containerMenu.activeSelf);
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
        if (inventoryMenu.activeSelf)
            TurnOffHighlighting();
        ToggleInventory();
        ToggleEquipmentMenu();
        if (containerMenu.activeSelf)
            ToggleContainerMenu();
        if (quantityMenu.gameObject.activeSelf)
            quantityMenu.CloseQuantityMenu();

        if (gm.isUsingController)
        {
            if (inventoryMenu.activeSelf)
                UIControllerNavigation.instance.FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, pocketsSlots), 0);
            else
                UIControllerNavigation.instance.ClearCurrentlySelected();
        }
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
