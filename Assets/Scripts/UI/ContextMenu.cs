using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ContextMenu : MonoBehaviour, IPointerClickHandler
{
    public GameObject contextMenuButtonPrefab;

    GameObject contextMenu;
    Canvas canvas;
    PlayerMovement player;
    EquipmentManager equipmentManager;
    Inventory inv;
    InventoryUI invUI;
    InventorySlot thisInvSlot;
    EquipSlot thisEquipSlot;

    void Start()
    {
        contextMenu = GameObject.Find("Context Menu");
        canvas = GetComponentInParent<Canvas>();
        player = PlayerMovement.instance;
        equipmentManager = player.GetComponent<EquipmentManager>();
        inv = Inventory.instance;
        invUI = InventoryUI.instance;
        thisInvSlot = GetComponentInParent<InventorySlot>();
        thisEquipSlot = GetComponentInParent<EquipSlot>();
    }

    void Update()
    {
        if (contextMenu.transform.childCount > 0
            && (GameControls.gamePlayActions.menuSelect.WasPressed || GameControls.gamePlayActions.menuContext.WasPressed || Input.GetMouseButtonDown(2))
            && EventSystem.current.currentSelectedGameObject == null)
        {
            DisableContextMenu();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu.transform.childCount > 0) // If the context menu is already open
                DisableContextMenu();
            // If the context menu is not open and needs built and the slot has an item in it
            else if ((thisInvSlot != null && thisInvSlot.isEmpty == false) || (thisEquipSlot != null && thisEquipSlot.isEmpty == false))
            {
                // Set our context menu's position
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos);
                contextMenu.transform.position = canvas.transform.TransformPoint(pos) + new Vector3(1, -0.5f, 0);

                // If this slot is on the far right of the inventory menu
                if (thisInvSlot != null && thisInvSlot.slotCoordinate.x == invUI.maxInventoryWidth)
                    contextMenu.transform.position += new Vector3(-2, 0, 0);

                // If this slot is on the very bottom of the screen
                if (pos.y < -420f)
                    contextMenu.transform.position += new Vector3(0, 1.5f, 0);

                CreateTakeItemButton();
                CreateUseItemButton();
                CreateSplitStackButton();
                CreateDropItemButton();
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle)
        {
            if (contextMenu.transform.childCount > 0)
                DisableContextMenu();
        }
    }

    public void DisableContextMenu()
    {
        if (contextMenu != null)
        {
            for (int i = 0; i < contextMenu.transform.childCount; i++)
            {
                Destroy(contextMenu.transform.GetChild(i).gameObject);
            }
        }
    }

    void CreateTakeItemButton()
    {
        if (thisInvSlot != null && thisInvSlot.slotParent == invUI.containerParent)
        {
            GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);

            menuButton.name = "Take";
            menuButton.GetComponentInChildren<Text>().text = "Take Item";

            menuButton.GetComponent<Button>().onClick.AddListener(TakeItem);
        }
    }

    void CreateSplitStackButton()
    {
        if (thisInvSlot != null)
        {
            InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);

            if (parentSlot.item.isStackable && parentSlot.itemData.currentStackSize > 1)
            {
                GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);

                menuButton.name = "Split Stack";
                menuButton.GetComponentInChildren<Text>().text = "Split Stack";

                menuButton.GetComponent<Button>().onClick.AddListener(SplitStack);
            }

            invUI.quantityMenu.transform.position = parentSlot.transform.position + new Vector3(0, -2.25f, 0);

            // If the slot is on the far right of the inventory menu
            if (parentSlot.slotParent != invUI.containerParent && parentSlot.slotCoordinate.x == invUI.maxInventoryWidth)
                invUI.quantityMenu.transform.position += new Vector3(-1.25f, 0, 0);

            // Get our mouse's screen position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos);

            // If the slot is on the very bottom of the screen
            if (pos.y < -290.0f)
                invUI.quantityMenu.transform.position += new Vector3(0, 4f, 0);
        }
    }

    void CreateUseItemButton()
    {
        GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);

        if (thisInvSlot != null)
        {
            InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);

            if (parentSlot.item.itemType == ItemType.General)
            {
                menuButton.name = "Use";
                menuButton.GetComponentInChildren<Text>().text = "Use";
            }
            else if (parentSlot.item.itemType == ItemType.Consumable)
            {
                menuButton.name = "Consume";
                menuButton.GetComponentInChildren<Text>().text = "Consume";
            }
            else if (parentSlot.item.itemType == ItemType.Ammunition)
            {
                menuButton.name = "Add to Quiver";
                menuButton.GetComponentInChildren<Text>().text = "Add to Quiver";
            }
            else
            {
                menuButton.name = "Equip";
                menuButton.GetComponentInChildren<Text>().text = "Equip";
            }

            menuButton.GetComponent<Button>().onClick.AddListener(UseItem);
        }
        else if (thisEquipSlot != null)
        {
            menuButton.name = "Unequip";
            menuButton.GetComponentInChildren<Text>().text = "Unequip";

            menuButton.GetComponent<Button>().onClick.AddListener(UnequipItem);
        }
    }

    void CreateDropItemButton()
    {
        GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);
        menuButton.name = "Drop";
        menuButton.GetComponentInChildren<Text>().text = "Drop";
        
        menuButton.GetComponent<Button>().onClick.AddListener(DropItem);
    }

    void TakeItem()
    {
        InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);

        if (inv.AddToInventory(parentSlot.item, parentSlot.itemData) == false)
                Debug.Log("Not enough room in inventory.");

        if (parentSlot.itemData.currentStackSize <= 0)
        {
            invUI.currentlyActiveContainer.containerItems.Remove(parentSlot.itemData);

            foreach (GameObject obj in invUI.currentlyActiveContainer.containerObjects)
            {
                if (obj.GetComponent<ItemData>() == parentSlot.itemData)
                {
                    invUI.currentlyActiveContainer.containerObjects.Remove(obj);
                    Destroy(obj);
                    break;
                }
            }

            parentSlot.ClearSlot();
        }
        else
        {
            if (parentSlot.itemData.currentStackSize > 1)
                parentSlot.stackSizeText.text = parentSlot.itemData.currentStackSize.ToString();
            else
                parentSlot.stackSizeText.text = "";
        }

        DisableContextMenu();
    }

    void SplitStack()
    {
        InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);

        // Bring up a +/- menu ui to increase/decrease the amount to split (should never be able to take enough to deplete the stack)
        invUI.quantityMenu.gameObject.SetActive(true);
        invUI.quantityMenu.currentParentSlot = parentSlot;
        invUI.quantityMenu.currentItemData = parentSlot.itemData;

        DisableContextMenu();
    }

    void UseItem()
    {
        InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);
        
        parentSlot.item.Use(parentSlot.itemData, equipmentManager, parentSlot);

        DisableContextMenu();
    }

    void UnequipItem()
    {
        if (equipmentManager.Unequip(thisEquipSlot.equipment, thisEquipSlot.itemData, thisEquipSlot.thisWeaponSlot, thisEquipSlot.thisEquipmentSlot, true))
            thisEquipSlot.ClearSlot(thisEquipSlot);

        DisableContextMenu();
    }

    public void DropItem()
    {
        if (thisInvSlot != null) // If we clicked on an inventory slot
        {
            InventorySlot parentSlot = thisInvSlot.GetParentSlot(thisInvSlot);

            GameObject itemToDrop = Instantiate(parentSlot.item.prefab, player.transform.position, Quaternion.identity);
            parentSlot.itemData.TransferData(parentSlot.itemData, itemToDrop.GetComponent<ItemData>());
            itemToDrop.name = parentSlot.itemData.itemName;

            StartCoroutine(DelayDrop(itemToDrop));

            inv.RemoveItem(parentSlot.itemData); // Remove the item from the items list
            parentSlot.ClearSlot();
        }
        else if (thisEquipSlot != null) // If we clicked on an equipment slot
        {
            GameObject itemToDrop = Instantiate(thisEquipSlot.equipment.prefab, player.transform.position, Quaternion.identity);
            thisEquipSlot.itemData.TransferData(thisEquipSlot.itemData, itemToDrop.GetComponent<ItemData>());
            itemToDrop.name = thisEquipSlot.itemData.itemName;
            
            StartCoroutine(DelayDrop(itemToDrop));

            equipmentManager.Unequip(thisEquipSlot.equipment, thisEquipSlot.itemData, thisEquipSlot.thisWeaponSlot, thisEquipSlot.thisEquipmentSlot, false);
            thisEquipSlot.ClearSlot(thisEquipSlot);
        }
        else
            Debug.LogError("Slot is not an inv slot or an equipment slot! Fix this!");

        DisableContextMenu();
    }

    IEnumerator DelayDrop(GameObject itemToDrop)
    {
        yield return new WaitForSeconds(0.1f);
        itemToDrop.GetComponent<ItemDrop>().DropItem(true);
    }
}
