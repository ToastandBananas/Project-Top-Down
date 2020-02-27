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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu.transform.childCount > 0) // If the context menu is already open
            {
                DisableContextMenu();
            }
            // If the context menu is not open and needs built and the slot has an item in it
            else if ((thisInvSlot != null && thisInvSlot.isEmpty == false) || (thisEquipSlot != null && thisEquipSlot.isEmpty == false))
            {
                // Set our context menu's position
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos);
                contextMenu.transform.position = canvas.transform.TransformPoint(pos);

                CreateDropItemButton();
            }
        }
    }

    void DisableContextMenu()
    {
        for (int i = 0; i < contextMenu.transform.childCount; i++)
        {
            Destroy(contextMenu.transform.GetChild(i).gameObject);
        }
    }

    void CreateDropItemButton()
    {
        GameObject menuButton = Instantiate(contextMenuButtonPrefab, contextMenu.transform);
        menuButton.name = "Drop";
        menuButton.GetComponentInChildren<Text>().text = "Drop";

        if (thisInvSlot != null)
            menuButton.GetComponent<Button>().onClick.AddListener(DropItem);
        else if (thisEquipSlot != null)
            menuButton.GetComponent<Button>().onClick.AddListener(DropItem);
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

            equipmentManager.Unequip(thisEquipSlot.equipment, thisEquipSlot.itemData, thisEquipSlot.thisWeaponSlot, thisEquipSlot.thisEquipmentSlot);

            thisEquipSlot.ClearSlot(thisEquipSlot);
        }

        DisableContextMenu();
    }

    IEnumerator DelayDrop(GameObject itemToDrop)
    {
        yield return new WaitForSeconds(0.1f);
        itemToDrop.GetComponent<ItemDrop>().DropItem(true);
    }
}
