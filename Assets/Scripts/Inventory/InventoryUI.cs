using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public GameObject slotPrefab;
    public GameObject inventoryGO;

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

    [HideInInspector] public Item currentlySelectedItem;
    [HideInInspector] public InventorySlot movingFromSlot;

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

        // inventory.onItemChangedCallback += UpdateUI; // Call UpdateUI whenever the onItemChangedCallback delegate is called

        pocketsSlots = pocketsParent.GetComponentsInChildren<InventorySlot>();
        bagSlots = bagParent.GetComponentsInChildren<InventorySlot>();
        horseBagSlots = horseBagParent.GetComponentsInChildren<InventorySlot>();

        if (player.isMounted == false)
            ShowHorseSlots(false);
    }
    
    void Update()
    {
        if (GameControls.gamePlayActions.playerInventory.WasPressed)
            inventoryGO.SetActive(!inventoryGO.activeSelf);
    }

    /*void UpdateUI()
    {
        for (int i = 0; i < pocketsSlots.Length; i++)
        {
            //if (i < inventory.pocketItems.Count)
               //pocketsSlots[i].AddItem(inventory.pocketItems[i]);
            //else
                //pocketsSlots[i].ClearSlot();
        }

        for (int i = 0; i < bagSlots.Length; i++)
        {
            //if (i < inventory.bagItems.Count)
                //bagSlots[i].AddItem(inventory.bagItems[i]);
            //else
                //bagSlots[i].ClearSlot();
        }

        for (int i = 0; i < horseBagSlots.Length; i++)
        {
            //if (i < inventory.horseItems.Count)
                //horseBagSlots[i].AddItem(inventory.horseItems[i]);
            //else
                //horseBagSlots[i].ClearSlot();
        }
    }*/

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
        movingFromSlot = null;
    }

    void ShowHorseSlots(bool showSlots)
    {
        horseBagParent.gameObject.SetActive(showSlots);
        horseBagTitle.gameObject.SetActive(showSlots);
    }
}
