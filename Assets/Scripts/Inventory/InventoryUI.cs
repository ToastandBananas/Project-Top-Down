using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public GameObject slotPrefab;
    public GameObject inventoryGO;

    public Transform pocketsParent;
    public Transform bagParent;
    public Transform horseBagParent;

    public Transform pocketsTitle;
    public Transform bagTitle;
    public Transform horseBagTitle;

    int maxInventoryWidth = 8;

    Inventory inventory;
    PlayerMovement player;

    public InventorySlot[] pocketsSlots;
    public InventorySlot[] bagSlots;
    public InventorySlot[] horseBagSlots;

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

        CreateSlots();

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
        {
            inventoryGO.SetActive(!inventoryGO.activeSelf);
        }
    }

    void FixedUpdate()
    {
        RepositionBagTitles();
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

    void CreateSlots()
    {
        int currentXCoord = 1;
        int currentYCoord = 1;
        for (int i = 1; i < inventory.pocketsSlotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, pocketsParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth + 1)
            {
                currentXCoord = 1;
                currentYCoord++;
            }

            slot.name = "Pocket Slot " + i;
        }

        currentXCoord = 1;
        currentYCoord = 1;
        for (int i = 1; i < inventory.bagSlotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, bagParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth + 1)
            {
                currentXCoord = 1;
                currentYCoord++;
            }

            slot.name = "Bag Slot " + i;
        }

        currentXCoord = 1;
        currentYCoord = 1;
        for (int i = 1; i < inventory.horseBagSlotCount + 1; i++)
        {
            GameObject slot = Instantiate(slotPrefab, horseBagParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth + 1)
            {
                currentXCoord = 1;
                currentYCoord++;
            }

            slot.name = "Horse Bag Slot " + i;
        }
    }

    void RepositionBagTitles()
    {
        pocketsTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, pocketsParent.GetComponent<RectTransform>().anchoredPosition.y);
        bagTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, bagParent.GetComponent<RectTransform>().anchoredPosition.y);
        horseBagTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, horseBagParent.GetComponent<RectTransform>().anchoredPosition.y);
    }

    void ShowHorseSlots(bool showSlots)
    {
        horseBagParent.gameObject.SetActive(showSlots);
        horseBagTitle.gameObject.SetActive(showSlots);
    }
}
