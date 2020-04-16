using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuantityMenu : MonoBehaviour
{
    public InventorySlot currentParentSlot;
    public ItemData currentItemData;
    public int currentQuantity = 1;

    public Button submitButton;
    public Button cancelButton;

    Text quantityText;
    InventoryUI invUI;
    UIControllerNavigation UIControllerNav;

    void Awake()
    {
        quantityText = transform.Find("Quantity Text").GetComponent<Text>();
        invUI = InventoryUI.instance;
        UIControllerNav = UIControllerNavigation.instance;
    }

    void OnEnable()
    {
        ResetCurrentQuantity();
    }

    void Update()
    {
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
        invUI.tempSlot.AddItem(currentItemData.item, currentItemData);
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

        currentParentSlot.SetAmmoSprites();
        invUI.tempSlot.SetAmmoSprites();

        UIControllerNav.SetIconPosition();
        UIControllerNav.HighlightItem();

        CloseQuantityMenu();
    }

    public void CloseQuantityMenu()
    {
        gameObject.SetActive(false);
        currentItemData = null;
        currentParentSlot = null;

        UIControllerNav.ClearSelectedButton();
    }
}
