using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIControllerNavigation : MonoBehaviour
{
    GameManager gm;
    InventoryUI invUI;
    Inventory inv;
    RectTransform canvasRectTransform;

    Vector2 uiOffset;
    Vector2 viewportPos;
    Vector2 proportionalPos;

    InventorySlot[] selectedInvSlots;
    int totalSlotsToCheck;

    public GameObject currentlySelectedObject;
    public InventorySlot currentlySelectedInventorySlot;
    public EquipSlot currentlySelectedEquipSlot;
    public Button currentlySelectedButton;

    public int currentXCoord = 1;
    public int currentOverallYCoord = 1;

    int xOffset = 0;
    bool canNavigate = true;
    float navigationWaitTime = 0.15f;

    #region Singleton
    public static UIControllerNavigation instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of UIControllerNavigation found!");
            Destroy(this);
        }
    }
    #endregion

    void Start()
    {
        gm = GameManager.instance;
        invUI = InventoryUI.instance;
        inv = Inventory.instance;
        canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        uiOffset = new Vector2(canvasRectTransform.sizeDelta.x / 2f, canvasRectTransform.sizeDelta.y / 2f);
    }
    
    void Update()
    {
        if (gm.isUsingController && gm.menuOpen)
        {
            // Pause Menu
            if (gm.pauseMenu.activeSelf)
            {
                if (GameControls.gamePlayActions.menuDown.WasPressed)
                {
                    if (currentlySelectedButton == gm.resumeButton)
                        StartCoroutine(SelectButton(gm.loadButton));
                    else if (currentlySelectedButton == gm.loadButton)
                        StartCoroutine(SelectButton(gm.saveButton));
                    else if (currentlySelectedButton == gm.saveButton)
                        StartCoroutine(SelectButton(gm.saveAndQuitButton));
                }
                else if (GameControls.gamePlayActions.menuUp.WasPressed)
                {
                    if (currentlySelectedButton == gm.saveAndQuitButton)
                        StartCoroutine(SelectButton(gm.saveButton));
                    else if (currentlySelectedButton == gm.saveButton)
                        StartCoroutine(SelectButton(gm.loadButton));
                    else if (currentlySelectedButton == gm.loadButton)
                        StartCoroutine(SelectButton(gm.resumeButton));
                    else if (currentlySelectedButton == gm.resumeButton)
                        StartCoroutine(SelectButton(gm.resumeButton));
                }

                if (GameControls.gamePlayActions.menuSelect.WasPressed)
                    currentlySelectedButton.onClick.Invoke();
            }
            // Context Menu
            else if (invUI.contextMenu.transform.childCount > 0)
            {
                if (GameControls.gamePlayActions.menuDown.WasPressed)
                {
                    // Navigate through children of the context menu
                    for (int i = 0; i < invUI.contextMenu.childCount; i++)
                    {
                        if (invUI.contextMenu.GetChild(i).gameObject == currentlySelectedObject)
                        {
                            if (invUI.contextMenu.childCount > i + 1)
                                StartCoroutine(SelectButton(invUI.contextMenu.GetChild(i + 1).GetComponent<Button>()));
                            break;
                        }
                    }
                }
                else if (GameControls.gamePlayActions.menuUp.WasPressed)
                {
                    // Navigate through children of the context menu
                    for (int i = 0; i < invUI.contextMenu.childCount; i++)
                    {
                        if (invUI.contextMenu.GetChild(i).gameObject == currentlySelectedObject)
                        {
                            if (i != 0)
                                StartCoroutine(SelectButton(invUI.contextMenu.GetChild(i - 1).GetComponent<Button>()));
                            break;
                        }
                    }
                }

                if (GameControls.gamePlayActions.menuSelect.WasPressed)
                    currentlySelectedButton.onClick.Invoke();
                else if (GameControls.gamePlayActions.menuContext.WasPressed || GameControls.gamePlayActions.menuLeft.WasPressed || GameControls.gamePlayActions.menuRight.WasPressed)
                {
                    if (invUI.contextMenu.childCount > 0)
                        DisableContextMenu();
                }
            }
            // Quantity Menu
            else if (invUI.quantityMenu.gameObject.activeSelf)
            {
                if (GameControls.gamePlayActions.menuLeft.WasPressed)
                    invUI.quantityMenu.DecreaseQuantity();
                else if (GameControls.gamePlayActions.menuRight.WasPressed)
                    invUI.quantityMenu.IncreaseQuantity();
                else if (GameControls.gamePlayActions.menuContext.WasPressed)
                    invUI.quantityMenu.CloseQuantityMenu();
                else if (GameControls.gamePlayActions.menuSelect.WasPressed)
                    invUI.quantityMenu.Submit();
            }
            // Inventory Menu
            else if (invUI.inventoryMenu.activeSelf || invUI.playerEquipmentMenu.activeSelf || invUI.containerMenu.activeSelf)
            {
                if (GameControls.gamePlayActions.menuContext.WasPressed)
                {
                    if (currentlySelectedInventorySlot != null && currentlySelectedInventorySlot.isEmpty == false)
                        currentlySelectedInventorySlot.contextMenu.BuildContextMenu(false);
                    else if (currentlySelectedEquipSlot != null && currentlySelectedEquipSlot.isEmpty == false)
                        currentlySelectedEquipSlot.contextMenu.BuildContextMenu(false);
                }

                if (GameControls.gamePlayActions.menuSelect.WasPressed)
                {
                    // Select or place the item if there's one in the currently selected slot
                    if (currentlySelectedInventorySlot != null)
                        currentlySelectedInventorySlot.itemButton.onClick.Invoke();
                    else if (currentlySelectedEquipSlot != null)
                        currentlySelectedEquipSlot.itemButton.onClick.Invoke();

                    // If an item was selected
                    if (invUI.currentlySelectedItem != null)
                    {
                        SetIconPosition();
                        HighlightItem();
                    }
                    else
                    {
                        if (currentlySelectedInventorySlot != null)
                            HighlightSlot(currentlySelectedInventorySlot, null);
                        else if (currentlySelectedEquipSlot != null)
                            HighlightSlot(null, currentlySelectedEquipSlot);
                    }
                }

                if (GameControls.gamePlayActions.menuDropItem)
                {
                    if (currentlySelectedInventorySlot != null && currentlySelectedInventorySlot.isEmpty == false)
                    {
                        InventorySlot parentSlot = currentlySelectedInventorySlot.GetParentSlot(currentlySelectedInventorySlot);
                        parentSlot.DropItem();
                    }
                    else if (currentlySelectedEquipSlot != null && currentlySelectedEquipSlot.isEmpty == false)
                        currentlySelectedEquipSlot.DropItem();
                }

                if (currentlySelectedInventorySlot != null && currentlySelectedInventorySlot.slotParent == invUI.containerParent)
                {
                    if (GameControls.gamePlayActions.menuContainerTakeAll.WasPressed)
                        inv.TakeAll();
                    else if (GameControls.gamePlayActions.menuContainerTakeGold.WasPressed)
                        inv.TakeGold();
                }

                if (canNavigate)
                {
                    if (GameControls.gamePlayActions.menuLeft.IsPressed)
                        NavigateLeft();
                    else if (GameControls.gamePlayActions.menuRight.IsPressed)
                        NavigateRight();
                    else if (GameControls.gamePlayActions.menuDown.IsPressed)
                        NavigateDown();
                    else if (GameControls.gamePlayActions.menuUp.IsPressed)
                        NavigateUp();
                }
            }
        }
    }

    public void SetIconPosition()
    {
        if (currentlySelectedInventorySlot != null)
            viewportPos = Camera.main.WorldToViewportPoint(currentlySelectedInventorySlot.transform.position);
        else if (currentlySelectedEquipSlot != null)
            viewportPos = Camera.main.WorldToViewportPoint(currentlySelectedEquipSlot.transform.position);

        proportionalPos = new Vector2(viewportPos.x * canvasRectTransform.sizeDelta.x, viewportPos.y * canvasRectTransform.sizeDelta.y);

        if (invUI.invSlotMovingFrom != null)
        {
            invUI.invSlotMovingFrom.iconImage.rectTransform.localPosition = proportionalPos - uiOffset;
            if (currentlySelectedInventorySlot != null)
                invUI.invSlotMovingFrom.iconImage.rectTransform.localPosition -= inv.GetItemInvPositionOffset(invUI.currentlySelectedItem);
        }
        else if (invUI.equipSlotMovingFrom != null)
        {
            invUI.equipSlotMovingFrom.iconImage.rectTransform.localPosition = proportionalPos - uiOffset;
            if (currentlySelectedInventorySlot != null)
                invUI.equipSlotMovingFrom.iconImage.rectTransform.localPosition -= inv.GetItemInvPositionOffset(invUI.currentlySelectedItem);
        }
    }

    IEnumerator NavigateCooldown()
    {
        canNavigate = false;
        yield return new WaitForSeconds(navigationWaitTime);
        canNavigate = true;
    }

    void NavigateLeft()
    {
        if (currentlySelectedInventorySlot != null)
        {
            RemoveHighlightFromItem();

            if (currentlySelectedInventorySlot.slotCoordinate.x == 1) // If a left most inv or container slot is selected
            {
                // If equipment menu is active and a container slot is selected, select right weapon slot
                if (invUI.playerEquipmentMenu.activeSelf && currentlySelectedInventorySlot.slotParent == invUI.containerParent)
                    FocusOnEquipSlot(inv.GetEquipSlot(WeaponSlot.WeaponRight, EquipmentSlot.None));
                else // If left most inventory slot is selected
                {
                    if (invUI.containerMenu.activeSelf) // If container is active, select right most slot in container
                    {
                        FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(invUI.maxContainerWidth, 1), invUI.containerSlots), 0, 0);
                        currentXCoord = invUI.maxContainerWidth;
                        currentOverallYCoord = 1;
                        invUI.containerItemsParent.localPosition = new Vector3(0, 240, 0);
                    }
                    else if (invUI.playerEquipmentMenu.activeSelf) // If equipment menu is active, select right weapon slot
                        FocusOnEquipSlot(inv.GetEquipSlot(WeaponSlot.WeaponRight, EquipmentSlot.None));
                }
            }
            else
                NavigateInventory(-1, 0);
        }
        else if (currentlySelectedEquipSlot != null)
            FocusOnEquipSlot(currentlySelectedEquipSlot.leftSlot);

        StartCoroutine(NavigateCooldown());
        
        // If an item is selected
        if (invUI.currentlySelectedItem != null)
        {
            SetIconPosition();
            HighlightItem();
        }
    }

    void NavigateRight()
    {
        RemoveHighlightFromItem();

        if (currentlySelectedInventorySlot != null)
        {
            // If right most container slot is selected
            if (currentlySelectedInventorySlot.slotParent == invUI.containerParent && currentlySelectedInventorySlot.slotCoordinate.x == invUI.maxContainerWidth)
            {
                FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.pocketsSlots), 1, 0);
                currentXCoord = 1;
                currentOverallYCoord = 1;
                invUI.invItemsParent.localPosition = new Vector3(0, 510, 0);
            }
            else
                NavigateInventory(1, 0);
        }
        else if (currentlySelectedEquipSlot != null)
        {
            // If a right most equip slot is selected
            if (currentlySelectedEquipSlot == inv.GetEquipSlot(WeaponSlot.WeaponRight, EquipmentSlot.None)
                || currentlySelectedEquipSlot == inv.GetEquipSlot(WeaponSlot.Ranged, EquipmentSlot.None)
                || currentlySelectedEquipSlot == inv.GetEquipSlot(WeaponSlot.None, EquipmentSlot.Quiver))
            {
                if (invUI.containerMenu.activeSelf)
                {
                    FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.containerSlots), 0, 0);
                    currentXCoord = 1;
                    currentOverallYCoord = 1;
                    invUI.containerItemsParent.localPosition = new Vector3(0, 240, 0);
                }
                else if (invUI.inventoryMenu.activeSelf)
                {
                    FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.pocketsSlots), 0, 0);
                    currentXCoord = 1;
                    currentOverallYCoord = 1;
                    invUI.invItemsParent.localPosition = new Vector3(0, 510, 0);
                }
            }
            else
                FocusOnEquipSlot(currentlySelectedEquipSlot.rightSlot);
        }

        StartCoroutine(NavigateCooldown());

        // If an item is selected
        if (invUI.currentlySelectedItem != null)
        {
            SetIconPosition();
            HighlightItem();
        }
    }

    void NavigateUp()
    {
        RemoveHighlightFromItem();

        if (currentlySelectedInventorySlot != null)
        {
            if (currentlySelectedInventorySlot.slotCoordinate.y == 1) // If top most inv slot is selected
            {
                // If bag equipped, focus on bag slots
                if (currentlySelectedInventorySlot.slotParent == invUI.bagParent && invUI.pocketsParent.gameObject.activeSelf)
                    NavigateToNextBag(0, -1, invUI.pocketsHeight, invUI.pocketsSlots);
                else if (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent) // If horse bag active
                {
                    if (invUI.bagParent.gameObject.activeSelf) // If bag equipped, focus on bag slots
                        NavigateToNextBag(0, -1, invUI.bagHeight, invUI.bagSlots);
                    else if (invUI.pocketsParent.gameObject.activeSelf) // If pocket slots active, focus on pocket slots
                        NavigateToNextBag(0, -1, invUI.pocketsHeight, invUI.pocketsSlots);
                }
            }
            else // Otherwise just navigate the inventory
                NavigateInventory(0, -1);
        }
        else if (currentlySelectedEquipSlot != null)
            FocusOnEquipSlot(currentlySelectedEquipSlot.upSlot);

        StartCoroutine(NavigateCooldown());

        // If an item is selected
        if (invUI.currentlySelectedItem != null)
        {
            SetIconPosition();
            HighlightItem();
        }
    }

    void NavigateDown()
    {
        RemoveHighlightFromItem();

        if (currentlySelectedInventorySlot != null)
        {
            if ((currentlySelectedInventorySlot.slotParent == invUI.pocketsParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.pocketsHeight - 1 && inv.GetSlotByCoordinates(new Vector2(currentXCoord, currentlySelectedInventorySlot.slotCoordinate.y + 1), invUI.pocketsSlots) == null)
                || (currentlySelectedInventorySlot.slotParent == invUI.bagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.bagHeight - 1 && inv.GetSlotByCoordinates(new Vector2(currentXCoord, currentlySelectedInventorySlot.slotCoordinate.y + 1), invUI.bagSlots) == null)
                || (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.horseBagHeight - 1 && inv.GetSlotByCoordinates(new Vector2(currentXCoord, currentlySelectedInventorySlot.slotCoordinate.y + 1), invUI.horseBagSlots) == null))
            {
                if (currentlySelectedInventorySlot.slotParent == invUI.pocketsParent)
                    NavigateToRowBelow(invUI.pocketsHeight, invUI.pocketsSlots);
                else if (currentlySelectedInventorySlot.slotParent == invUI.bagParent)
                    NavigateToRowBelow(invUI.bagHeight, invUI.bagSlots);
                else if (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent)
                    NavigateToRowBelow(invUI.horseBagHeight, invUI.horseBagSlots);
            }
            // If top most inv slot is selected
            else if ((currentlySelectedInventorySlot.slotParent == invUI.pocketsParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.pocketsHeight)
                || (currentlySelectedInventorySlot.slotParent == invUI.bagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.bagHeight)
                || (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.horseBagHeight))
            {
                // If horse bag slots are active, focus on bag slots
                if (currentlySelectedInventorySlot.slotParent == invUI.bagParent && invUI.horseBagParent.gameObject.activeSelf)
                    NavigateToNextBag(0, 1, invUI.horseBagHeight, invUI.horseBagSlots);
                else if (currentlySelectedInventorySlot.slotParent == invUI.pocketsParent) // If pocket slots active
                {
                    if (invUI.bagParent.gameObject.activeSelf) // If bag equipped, focus on bag slots
                        NavigateToNextBag(0, 1, invUI.bagHeight, invUI.bagSlots);
                    else if (invUI.horseBagParent.gameObject.activeSelf) // If horse bag slots active, focus on horse bag slots
                        NavigateToNextBag(0, 1, invUI.horseBagHeight, invUI.horseBagSlots);
                }
            }
            else // Otherwise just navigate the inventory
                NavigateInventory(0, 1);
        }
        else if (currentlySelectedEquipSlot != null)
            FocusOnEquipSlot(currentlySelectedEquipSlot.downSlot);

        StartCoroutine(NavigateCooldown());

        // If an item is selected
        if (invUI.currentlySelectedItem != null)
        {
            SetIconPosition();
            HighlightItem();
        }
    }

    void NavigateToRowBelow(int bagHeight, List<InventorySlot> slotsList)
    {
        xOffset = 0;
        for (int i = currentXCoord; i > 0; i--)
        {
            if (inv.GetSlotByCoordinates(new Vector2(currentXCoord + xOffset, bagHeight), slotsList) != null)
            {
                FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentXCoord + xOffset, bagHeight), slotsList), 0, 1);

                currentXCoord += xOffset;
                break;
            }
            else
                xOffset--;
        }
    }

    void NavigateToNextBag(int xAddOn, int yAddOn, int bagHeight, List<InventorySlot> slotsList)
    {
        xOffset = 0;
        for (int i = currentXCoord; i > 0; i--)
        {
            if (inv.GetSlotByCoordinates(new Vector2(currentXCoord + xOffset, bagHeight), slotsList) != null)
            {
                if (yAddOn == 1)
                    FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentXCoord + xOffset, 1), slotsList), xAddOn, yAddOn);
                else
                    FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentXCoord + xOffset, bagHeight), slotsList), xAddOn, yAddOn);

                currentXCoord += xOffset;
                break;
            }
            else
                xOffset--;
        }
    }

    void NavigateInventory(int addX, int addY)
    {
        if (currentlySelectedInventorySlot.slotParent == invUI.containerParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.containerSlots), addX, addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.pocketsParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.pocketsSlots), addX, addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.bagParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.bagSlots), addX, addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.horseBagSlots), addX, addY);

        if (currentlySelectedInventorySlot.slotParent == invUI.containerParent)
        {
            Debug.Log(currentOverallYCoord);
            if (addY == 1 && currentOverallYCoord > 3)
                invUI.containerItemsParent.localPosition += new Vector3(0, 75, 0);
            else if (addY == -1 && currentOverallYCoord >= 3 && currentOverallYCoord < invUI.containerHeight)
                invUI.containerItemsParent.localPosition += new Vector3(0, -75, 0);
        }
        else
        {
            Debug.Log(currentOverallYCoord);
            if (addY == 1 && currentOverallYCoord > invUI.maxInventoryViewHeight / 2)
                invUI.invItemsParent.localPosition += new Vector3(0, 75, 0);
            else if (addY == -1 && currentOverallYCoord >= invUI.maxInventoryViewHeight / 2 && currentOverallYCoord < invUI.overallInventoryHeight)
                invUI.invItemsParent.localPosition += new Vector3(0, -75, 0);
        }
    }

    public void FocusOnEquipSlot(EquipSlot slotToFocusOn)
    {
        if (slotToFocusOn != null)
        {
            currentOverallYCoord = 1; // Reset this

            if (currentlySelectedEquipSlot != null)
                RemoveHighlight(null, currentlySelectedEquipSlot);

            if (currentlySelectedInventorySlot != null)
                RemoveHighlight(currentlySelectedInventorySlot, null);

            invUI.ClearAllTooltips();
            EventSystem.current.SetSelectedGameObject(null);
            currentlySelectedEquipSlot = slotToFocusOn;
            currentlySelectedObject = slotToFocusOn.gameObject;
            currentlySelectedInventorySlot = null;
            
            currentlySelectedEquipSlot.slotTooltip.GetTooltips();

            HighlightSlot(null, slotToFocusOn);
        }
    }

    public void FocusOnInvSlot(InventorySlot slotToFocusOn, int addToXCoord, int addToYCoord)
    {
        if (slotToFocusOn != null)
        {
            if (addToXCoord != 0)
                currentXCoord += addToXCoord;

            if (addToYCoord != 0)
                currentOverallYCoord += addToYCoord;

            if (currentlySelectedEquipSlot != null)
                RemoveHighlight(null, currentlySelectedEquipSlot);

            if (currentlySelectedInventorySlot != null)
                RemoveHighlight(currentlySelectedInventorySlot, null);

            invUI.ClearAllTooltips();
            EventSystem.current.SetSelectedGameObject(null);
            currentlySelectedEquipSlot = null;
            currentlySelectedObject = slotToFocusOn.gameObject;
            currentlySelectedInventorySlot = slotToFocusOn;

            currentlySelectedInventorySlot.slotTooltip.GetTooltips();

            HighlightSlot(slotToFocusOn, null);
        }
    }

    public void ClearCurrentlySelected()
    {
        if (currentlySelectedEquipSlot != null)
            RemoveHighlight(null, currentlySelectedEquipSlot);

        if (currentlySelectedInventorySlot != null)
            RemoveHighlight(currentlySelectedInventorySlot, null);

        currentXCoord = 1;
        currentOverallYCoord = 1;
        currentlySelectedEquipSlot = null;
        currentlySelectedObject = null;
        currentlySelectedInventorySlot = null;

        invUI.containerItemsParent.localPosition = new Vector3(0, 240, 0);
        invUI.invItemsParent.localPosition = new Vector3(0, 510, 0);
    }

    public void ClearSelectedButton()
    {
        currentlySelectedButton = null;
        if (currentlySelectedInventorySlot != null)
            currentlySelectedObject = currentlySelectedInventorySlot.gameObject;
        else if (currentlySelectedEquipSlot != null)
            currentlySelectedObject = currentlySelectedEquipSlot.gameObject;
        else
            ClearCurrentlySelected();
    }

    public void HighlightItem()
    {
        if (currentlySelectedInventorySlot != null)
            currentlySelectedInventorySlot.hoverHighlightScript.Highlight();
        else if (currentlySelectedEquipSlot != null)
            currentlySelectedEquipSlot.hoverHighlightScript.Highlight();
    }

    public void RemoveHighlightFromItem()
    {
        if (invUI.currentlySelectedItem != null)
        {
            if (currentlySelectedInventorySlot != null)
            {
                totalSlotsToCheck = invUI.currentlySelectedItem.iconWidth * invUI.currentlySelectedItem.iconHeight;
                selectedInvSlots = new InventorySlot[totalSlotsToCheck];
                inv.GetOverlappingItemCount(invUI.currentlySelectedItem, currentlySelectedInventorySlot, selectedInvSlots, currentlySelectedInventorySlot.invSlots);

                currentlySelectedInventorySlot.hoverHighlightScript.RemoveHighlight(selectedInvSlots);
            }
            else if (currentlySelectedEquipSlot != null)
                currentlySelectedEquipSlot.hoverHighlightScript.RemoveHighlight(null);
        }
    }

    public void HighlightSlot(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null)
        {
            invSlot.slotBackgroundImage.color = Color.green;
            if (invSlot.isEmpty == false)
                invSlot.slotBackgroundImage.sprite = invSlot.emptySlotSprite;
        }
        else if (equipSlot != null)
        {
            equipSlot.slotBackgroundImage.color = Color.green;
            if (equipSlot.isEmpty == false)
                equipSlot.slotBackgroundImage.sprite = equipSlot.emptySlotSprite;
        }
    }

    void RemoveHighlight(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null)
        {
            invSlot.slotBackgroundImage.color = Color.white;
            if (invSlot.isEmpty == false)
                invSlot.slotBackgroundImage.sprite = invSlot.fullSlotSprite;
        }
        else if (equipSlot != null)
        {
            equipSlot.slotBackgroundImage.color = Color.white;
            if (equipSlot.isEmpty == false)
                equipSlot.slotBackgroundImage.sprite = equipSlot.fullSlotSprite;
        }
    }

    public void DisableContextMenu()
    {
        for (int i = 0; i < invUI.contextMenu.transform.childCount; i++)
        {
            Destroy(invUI.contextMenu.transform.GetChild(i).gameObject);
        }

        ClearSelectedButton();
    }

    public IEnumerator SelectButton(Button button)
    {
        currentlySelectedObject = button.gameObject;
        currentlySelectedButton = button;
        yield return null;
        if (button != null)
        {
            button.OnSelect(null);
            button.Select();
        }
    }
}
