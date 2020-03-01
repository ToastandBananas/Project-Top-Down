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
    InventoryUI invUI;
    InventorySlot thisInvSlot;
    EquipSlot thisEquipSlot;

    void Start()
    {
        contextMenu = GameObject.Find("Context Menu");
        canvas = GetComponentInParent<Canvas>();
        player = PlayerMovement.instance;
        equipmentManager = player.GetComponent<EquipmentManager>();
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
                contextMenu.transform.position = canvas.transform.TransformPoint(pos) + (Vector3.down / 2) + Vector3.right;

                CreateUseItemButton();
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
        for (int i = 0; i < contextMenu.transform.childCount; i++)
        {
            Destroy(contextMenu.transform.GetChild(i).gameObject);
        }
    }

    void CreateUseItemButton()
    {
        GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);

        if (thisInvSlot != null)
        {
            InventorySlot parentSlot;
            if (thisInvSlot.parentSlot != null)
                parentSlot = thisInvSlot.parentSlot;
            else
                parentSlot = thisInvSlot;

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

    void UseItem()
    {
        InventorySlot parentSlot;
        if (thisInvSlot.parentSlot != null)
            parentSlot = thisInvSlot.parentSlot;
        else
            parentSlot = thisInvSlot;
        
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
            InventorySlot parentSlot;
            if (thisInvSlot.parentSlot != null)
                parentSlot = thisInvSlot.parentSlot;
            else
                parentSlot = thisInvSlot;

            GameObject itemToDrop = Instantiate(parentSlot.item.prefab, player.transform.position, Quaternion.identity);
            parentSlot.itemData.TransferData(parentSlot.itemData, itemToDrop.GetComponent<ItemData>());
            itemToDrop.name = parentSlot.itemData.itemName;

            StartCoroutine(DelayDrop(itemToDrop));

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
