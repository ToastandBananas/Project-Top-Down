using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject inventoryGO;

    public Transform pocketsParent;
    public Transform bagParent;
    public Transform horseBagParent;

    public Transform pocketsTitle;
    public Transform bagTitle;
    public Transform horseBagTitle;

    int maxInventoryWidth = 10;

    Inventory inventory;
    PlayerMovement player;

    InventorySlot[] pocketsSlots;
    InventorySlot[] bagSlots;
    InventorySlot[] horseBagSlots;

    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerMovement.instance;

        CreateSlots();

        inventory.onItemChangedCallback += UpdateUI; // Call UpdateUI whenever the onItemChangedCallback delegate is called

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

    void UpdateUI()
    {
        for (int i = 0; i < pocketsSlots.Length; i++)
        {
            if (i < inventory.pocketItems.Count)
                pocketsSlots[i].AddItem(inventory.pocketItems[i]);
            else
                pocketsSlots[i].ClearSlot();
        }

        for (int i = 0; i < bagSlots.Length; i++)
        {
            if (i < inventory.bagItems.Count)
                bagSlots[i].AddItem(inventory.bagItems[i]);
            else
                bagSlots[i].ClearSlot();
        }

        for (int i = 0; i < horseBagSlots.Length; i++)
        {
            if (i < inventory.horseItems.Count)
                horseBagSlots[i].AddItem(inventory.horseItems[i]);
            else
                horseBagSlots[i].ClearSlot();
        }
    }

    void CreateSlots()
    {
        int currentXCoord = 0;
        int currentYCoord = 0;
        for (int i = 0; i < inventory.pocketsSlotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, pocketsParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth)
            {
                currentXCoord = 0;
                currentYCoord++;
            }
        }

        currentXCoord = 0;
        currentYCoord = 0;
        for (int i = 0; i < inventory.bagSlotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, bagParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth)
            {
                currentXCoord = 0;
                currentYCoord++;
            }
        }

        currentXCoord = 0;
        currentYCoord = 0;
        for (int i = 0; i < inventory.horseSlotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, horseBagParent);
            slot.GetComponent<InventorySlot>().slotCoordinate = new Vector2(currentXCoord, currentYCoord);
            currentXCoord++;

            if (currentXCoord == maxInventoryWidth)
            {
                currentXCoord = 0;
                currentYCoord++;
            }
        }
    }

    void RepositionBagTitles()
    {
        pocketsTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(23, pocketsParent.GetComponent<RectTransform>().anchoredPosition.y);
        bagTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(23, bagParent.GetComponent<RectTransform>().anchoredPosition.y);
        horseBagTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(23, horseBagParent.GetComponent<RectTransform>().anchoredPosition.y);
    }

    void ShowHorseSlots(bool showSlots)
    {
        horseBagParent.gameObject.SetActive(showSlots);
        horseBagTitle.gameObject.SetActive(showSlots);
    }
}
