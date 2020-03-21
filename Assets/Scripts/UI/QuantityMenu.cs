using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuantityMenu : MonoBehaviour
{
    public InventorySlot currentParentSlot;
    public ItemData currentItemData;
    public int currentQuantity = 1;

    Text quantityText;
    InventoryUI invUI;

    void Awake()
    {
        quantityText = transform.Find("Quantity Text").GetComponent<Text>();
        invUI = InventoryUI.instance;
    }

    void OnEnable()
    {
        ResetCurrentQuantity();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);

        if ((GameControls.gamePlayActions.menuSelect.WasPressed || GameControls.gamePlayActions.menuContext.WasPressed || Input.GetMouseButtonDown(2))
            && (EventSystem.current.currentSelectedGameObject == null 
            || (EventSystem.current.currentSelectedGameObject != gameObject
                && EventSystem.current.currentSelectedGameObject != transform.GetChild(0).gameObject && EventSystem.current.currentSelectedGameObject != transform.GetChild(1).gameObject)
                && EventSystem.current.currentSelectedGameObject != transform.GetChild(2).gameObject && EventSystem.current.currentSelectedGameObject != transform.GetChild(3).gameObject))
        {
            CloseQuantityMenu();
        }
    }

    public void ResetCurrentQuantity()
    {
        currentQuantity = 1;
        quantityText.text = currentQuantity.ToString();
    }

    public void IncreaseQuantity()
    {
        if (currentQuantity < currentItemData.currentStackSize - 1)
        {
            currentQuantity++;
            quantityText.text = currentQuantity.ToString();
        }
    }

    public void DecreaseQuantity()
    {
        if (currentQuantity > 1)
        {
            currentQuantity--;
            quantityText.text = currentQuantity.ToString();
        }
    }

    public void Submit()
    {
        // Create a new icon, place it in the temp inv slot and transfer data to it
        invUI.tempSlot.AddItem(currentItemData.item);
        ItemData tempSlotItemData = invUI.tempSlot.itemData;
        currentItemData.TransferData(currentItemData, tempSlotItemData);
        tempSlotItemData.currentStackSize = currentQuantity;

        // Set the new icon as the currentlySelectedItem/ItemData and also set invSlotMovingFrom
        invUI.currentlySelectedItem = tempSlotItemData.item;
        invUI.currentlySelectedItemData = tempSlotItemData;
        invUI.invSlotMovingFrom = invUI.tempSlot;

        // Subtract amount taken from parentSlot
        currentItemData.currentStackSize -= currentQuantity;
        if (currentItemData.currentStackSize > 1)
            currentParentSlot.stackSizeText.text = currentItemData.currentStackSize.ToString();
        else
            currentParentSlot.stackSizeText.text = "";

        CloseQuantityMenu();
    }

    public void CloseQuantityMenu()
    {
        gameObject.SetActive(false);
        currentItemData = null;
        currentParentSlot = null;
    }
}
