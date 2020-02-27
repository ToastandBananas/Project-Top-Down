using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public GameObject slotPrefab;
    public GameObject inventoryGO;
    public GameObject playerEquipmentMenuGO;

    [Header("Parents")]
    public Transform pocketsParent;
    public Transform bagParent;
    public Transform horseBagParent;

    [Header("Titles")]
    public Transform pocketsTitle;
    public Transform bagTitle;
    public Transform horseBagTitle;

    int maxInventoryWidth = 8;

    Inventory inventory;
    PlayerMovement player;

    [Header("Slots")]
    public InventorySlot tempSlot;
    public InventorySlot[] pocketsSlots;
    public InventorySlot[] bagSlots;
    public InventorySlot[] horseBagSlots;

    [Header("Selected Item Info")]
    [HideInInspector] public Item currentlySelectedItem;
    [HideInInspector] public ItemData currentlySelectedItemData;
    [HideInInspector] public InventorySlot invSlotMovingFrom;
    [HideInInspector] public EquipSlot equipSlotMovingFrom;

    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one InventoryUI script is active.");
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerMovement.instance;

        CreateSlots(inventory.pocketsSlotCount, pocketsParent, pocketsSlots);
        CreateSlots(inventory.bagSlotCount, bagParent, bagSlots);
        CreateSlots(inventory.horseBagSlotCount, horseBagParent, horseBagSlots);

        tempSlot = Instantiate(slotPrefab, transform).GetComponent<InventorySlot>();
        tempSlot.transform.localPosition += new Vector3(10000f, 10000f, 0);
        tempSlot.name = "Temp Slot";

        pocketsSlots = pocketsParent.GetComponentsInChildren<InventorySlot>();
        bagSlots = bagParent.GetComponentsInChildren<InventorySlot>();
        horseBagSlots = horseBagParent.GetComponentsInChildren<InventorySlot>();

        if (player.isMounted == false)
            ShowHorseSlots(false);
    }
    
    void Update()
    {
        if (GameControls.gamePlayActions.playerInventory.WasPressed)
        {
            inventoryGO.SetActive(!inventoryGO.activeSelf);
            playerEquipmentMenuGO.SetActive(!playerEquipmentMenuGO.activeSelf);
        }

        if (GameControls.gamePlayActions.playerLeftAttack.WasPressed && currentlySelectedItem != null && EventSystem.current.currentSelectedGameObject == null)
        {
            if (invSlotMovingFrom != null)
                invSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
            else if (equipSlotMovingFrom != null)
                equipSlotMovingFrom.GetComponentInChildren<ContextMenu>().DropItem();
            
            StopDraggingInvItem();
        }
    }

    void CreateSlots(int slotCount, Transform slotsParent, InventorySlot[] slots)
    {
        //if (slots.Length > slotCount || slots.Length < slotCount)
            //slots = new InventorySlot[slotCount]; // Reset our slots array to the appropriate size

        int currentXCoord = 1;
        int currentYCoord = 1;
        for (int i = 1; i < slotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth + 1)
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
        }

        slots = slotsParent.GetComponentsInChildren<InventorySlot>(); // Add the new slots to our slots array
    }

    public void StopDraggingInvItem()
    {
        currentlySelectedItem = null;
        currentlySelectedItemData = null;
        invSlotMovingFrom = null;
        equipSlotMovingFrom = null;
        Cursor.visible = true;
    }

    void ShowHorseSlots(bool showSlots)
    {
        horseBagParent.gameObject.SetActive(showSlots);
        horseBagTitle.gameObject.SetActive(showSlots);
    }
}
