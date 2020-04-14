using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerNavigation : MonoBehaviour
{
    GameManager gm;
    InventoryUI invUI;
    Inventory inv;

    public GameObject currentlySelectedObject;
    public InventorySlot currentlySelectedInventorySlot;
    public EquipSlot currentlySelectedEquipSlot;
    public Button currentlySelectedButton;

    public int currentOverallYCoord = 1;

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
    }
    
    void Update()
    {
        if (gm.isUsingController && gm.menuOpen)
        {
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
            else if (invUI.contextMenu.gameObject.activeSelf)
            {
                if (GameControls.gamePlayActions.menuDown.WasPressed)
                {
                    // Navigate through children of the context menu
                    // Will have to GetComponent<Button>() because menu items are Instantiated
                }
                else if (GameControls.gamePlayActions.menuUp.WasPressed)
                {

                }
            }
            else if (invUI.inventoryMenu.activeSelf || invUI.playerEquipmentMenu.activeSelf || invUI.containerMenu.activeSelf)
            {
                if (GameControls.gamePlayActions.menuLeft.WasPressed)
                {
                    if (currentlySelectedInventorySlot != null)
                    {
                        if (currentlySelectedInventorySlot.slotCoordinate.x == 1) // If a left most inv or container slot is selected
                        {
                            // If equipment menu is active and a container slot is selected, select right weapon slot
                            if (invUI.playerEquipmentMenu.activeSelf && currentlySelectedInventorySlot.slotParent == invUI.containerParent)
                                FocusOnEquipSlot(inv.GetEquipSlot(WeaponSlot.WeaponRight, EquipmentSlot.None));
                            else // If left most inventory slot is selected
                            {
                                if (invUI.containerMenu.activeSelf) // If container is active, select right most slot in container
                                {
                                    FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(invUI.maxContainerWidth, 1), invUI.containerSlots), 0);
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
                }
                else if (GameControls.gamePlayActions.menuRight.WasPressed)
                {
                    if (currentlySelectedInventorySlot != null)
                    {
                        // If right most container slot is selected
                        if (currentlySelectedInventorySlot.slotParent == invUI.containerParent && currentlySelectedInventorySlot.slotCoordinate.x == invUI.maxContainerWidth)
                        {
                            FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.pocketsSlots), 0);
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
                                FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.containerSlots), 0);
                                currentOverallYCoord = 1;
                                invUI.containerItemsParent.localPosition = new Vector3(0, 240, 0);
                            }
                            else if (invUI.inventoryMenu.activeSelf)
                            {
                                FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.pocketsSlots), 0);
                                currentOverallYCoord = 1;
                                invUI.invItemsParent.localPosition = new Vector3(0, 510, 0);
                            }
                        }
                        else
                            FocusOnEquipSlot(currentlySelectedEquipSlot.rightSlot);
                    }
                }
                else if (GameControls.gamePlayActions.menuUp.WasPressed)
                {
                    if (currentlySelectedInventorySlot != null)
                    {
                        if (currentlySelectedInventorySlot.slotCoordinate.y == 1) // If top most inv slot is selected
                        {
                            // If bag equipped, focus on bag slots
                            if (currentlySelectedInventorySlot.slotParent == invUI.bagParent && invUI.pocketsParent.gameObject.activeSelf)
                                FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(1, invUI.pocketsHeight), invUI.pocketsSlots), -1);
                            else if (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent) // If horse bag active
                            {
                                if (invUI.bagParent.gameObject.activeSelf) // If bag equipped, focus on bag slots
                                    FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(1, invUI.bagHeight), invUI.bagSlots), -1);
                                else if (invUI.pocketsParent.gameObject.activeSelf) // If pocket slots active, focus on pocket slots
                                    FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(1, invUI.pocketsHeight), invUI.pocketsSlots), -1);
                            }
                        }
                        else // Otherwise just navigate the inventory
                            NavigateInventory(0, -1);
                    }
                    else if (currentlySelectedEquipSlot != null)
                        FocusOnEquipSlot(currentlySelectedEquipSlot.upSlot);
                }
                else if (GameControls.gamePlayActions.menuDown.WasPressed)
                {
                    if (currentlySelectedInventorySlot != null)
                    {
                        // If top most inv slot is selected
                        if ((currentlySelectedInventorySlot.slotParent == invUI.pocketsParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.pocketsHeight)
                            || (currentlySelectedInventorySlot.slotParent == invUI.bagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.bagHeight)
                            || (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent && currentlySelectedInventorySlot.slotCoordinate.y == invUI.horseBagHeight))
                        {
                            // If horse bag slots are active, focus on bag slots
                            if (currentlySelectedInventorySlot.slotParent == invUI.bagParent && invUI.horseBagParent.gameObject.activeSelf)
                                FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.horseBagSlots), 1);
                            else if (currentlySelectedInventorySlot.slotParent == invUI.pocketsParent) // If pocket slots active
                            {
                                if (invUI.bagParent.gameObject.activeSelf) // If bag equipped, focus on bag slots
                                    FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.bagSlots), 1);
                                else if (invUI.horseBagParent.gameObject.activeSelf) // If horse bag slots active, focus on horse bag slots
                                    FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.horseBagSlots), 1);
                            }
                        }
                        else // Otherwise just navigate the inventory
                            NavigateInventory(0, 1);
                    }
                    else if (currentlySelectedEquipSlot != null)
                        FocusOnEquipSlot(currentlySelectedEquipSlot.downSlot);
                }
            }
        }
    }

    void NavigateInventory(int addX, int addY)
    {
        if (currentlySelectedInventorySlot.slotParent == invUI.containerParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.containerSlots), addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.pocketsParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.pocketsSlots), addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.bagParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.bagSlots), addY);
        else if (currentlySelectedInventorySlot.slotParent == invUI.horseBagParent)
            FocusOnInvSlot(inv.GetSlotByCoordinates(new Vector2(currentlySelectedInventorySlot.slotCoordinate.x + addX, currentlySelectedInventorySlot.slotCoordinate.y + addY), invUI.horseBagSlots), addY);

        if (currentlySelectedInventorySlot.slotParent == invUI.containerParent)
        {
            if (addY == 1 && currentOverallYCoord > 6)
                invUI.containerItemsParent.localPosition += new Vector3(0, 75, 0);
            else if (addY == -1 && currentOverallYCoord >= 6)
                invUI.containerItemsParent.localPosition += new Vector3(0, -75, 0);
        }
        else
        {
            if (addY == 1 && currentOverallYCoord > invUI.maxOverallInventoryHeight)
                invUI.invItemsParent.localPosition += new Vector3(0, 75, 0);
            else if (addY == -1 && currentOverallYCoord >= invUI.maxOverallInventoryHeight)
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

            currentlySelectedEquipSlot = slotToFocusOn;
            currentlySelectedObject = slotToFocusOn.gameObject;
            currentlySelectedInventorySlot = null;

            HighlightSlot(null, slotToFocusOn);
        }
    }

    public void FocusOnInvSlot(InventorySlot slotToFocusOn, int addToYCoord)
    {
        if (slotToFocusOn != null)
        {
            if (addToYCoord != 0)
                currentOverallYCoord += addToYCoord;

            if (currentlySelectedEquipSlot != null)
                RemoveHighlight(null, currentlySelectedEquipSlot);

            if (currentlySelectedInventorySlot != null)
                RemoveHighlight(currentlySelectedInventorySlot, null);

            currentlySelectedEquipSlot = null;
            currentlySelectedObject = slotToFocusOn.gameObject;
            currentlySelectedInventorySlot = slotToFocusOn;

            HighlightSlot(slotToFocusOn, null);
        }
    }

    public void ClearCurrentlySelected()
    {
        if (currentlySelectedEquipSlot != null)
            RemoveHighlight(null, currentlySelectedEquipSlot);

        if (currentlySelectedInventorySlot != null)
            RemoveHighlight(currentlySelectedInventorySlot, null);

        currentOverallYCoord = 1;
        currentlySelectedEquipSlot = null;
        currentlySelectedObject = null;
        currentlySelectedInventorySlot = null;
    }

    void HighlightSlot(InventorySlot invSlot, EquipSlot equipSlot)
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

    public IEnumerator SelectButton(Button button)
    {
        currentlySelectedObject = button.gameObject;
        currentlySelectedButton = button;
        yield return null;
        button.OnSelect(null);
        button.Select();
    }
}
