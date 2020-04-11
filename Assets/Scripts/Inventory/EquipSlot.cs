using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    [Header("Slot")]
    public WeaponSlot thisWeaponSlot;
    public EquipmentSlot thisEquipmentSlot;
    public bool isEmpty = true;

    [Header("Icon")]
    public GameObject iconPrefab;
    public Image iconImage;

    [Header("Slot Background")]
    public Image slotBackgroundImage;
    public Sprite emptySlotSprite;
    public Sprite fullSlotSprite;
    public Text slotText;
    public Text quiverText;

    [Header("Item Data")]
    public Equipment equipment;
    public ItemData itemData;

    InventoryUI invUI;
    Inventory inv;
    AudioManager audioManager;
    EquipmentManager equipmentManager;
    HoverHighlight hoverHighlightScript;

    Vector3 mousePos;
    float xPosOffset = 0;
    float yPosOffset = 0;

    void Awake()
    {
        hoverHighlightScript = GetComponent<HoverHighlight>();
        slotText = transform.Find("Text").GetComponent<Text>();

        if (thisEquipmentSlot == EquipmentSlot.Quiver)
            quiverText = transform.Find("Stack Size Text").GetComponentInChildren<Text>();

        if (isEmpty == false)
            slotBackgroundImage.sprite = fullSlotSprite;
    }

    void Start()
    {
        invUI = InventoryUI.instance;
        inv = Inventory.instance;
        audioManager = AudioManager.instance;
        equipmentManager = PlayerMovement.instance.GetComponent<EquipmentManager>();
    }

    void Update()
    {
        FollowMouse();
    }

    public void MoveEquipment()
    {
        if (isEmpty == false && invUI.currentlySelectedItemData == null) // If we don't have an item selected and this slot has an item in it (select item so we can move it)
        {
            iconImage.transform.SetParent(invUI.menusParent);

            // Set our currently selected item info
            invUI.currentlySelectedItem = equipment;
            invUI.currentlySelectedItemData = itemData;
            invUI.equipSlotMovingFrom = this;

            audioManager.PlayPickUpItemSound(invUI.currentlySelectedItem);

            // Set this slot to appear empty
            isEmpty = true;
            slotBackgroundImage.sprite = emptySlotSprite;
            if (thisEquipmentSlot == EquipmentSlot.Quiver)
                quiverText.text = "";

            equipmentManager.Unequip(equipment, itemData, thisWeaponSlot, thisEquipmentSlot, false); // Unequip the item

            hoverHighlightScript.HighlightEquipSlots();
        }
        else if (isEmpty && this == invUI.equipSlotMovingFrom) // If we're trying to place an item back into the same slot it came from (place selected item back in the slot)
        {
            invUI.StopDraggingInvItem();
            iconImage.transform.SetParent(transform.GetChild(0).transform);
            iconImage.transform.localPosition = Vector3.zero;
            isEmpty = false;
            slotBackgroundImage.sprite = fullSlotSprite;
            slotBackgroundImage.color = Color.white;
            SetQuiverStackSizeText(); // If this is the quiver slot, set the stack size text

            equipmentManager.EquipItem(equipment, itemData, thisWeaponSlot, thisEquipmentSlot); // Equip the item
        }
        else if (isEmpty && invUI.currentlySelectedItemData != null) // If we have an item selected already and this slot is empty (equip item)
        {
            if (invUI.currentlySelectedItemData.equipment != null) // If this item is of the equipment type (armor, weapons, etc.)
            {
                // If this item belongs in this slot
                if ((invUI.currentlySelectedItemData.equipment.equipmentSlot != EquipmentSlot.None && invUI.currentlySelectedItemData.equipment.equipmentSlot == thisEquipmentSlot)
                    || (invUI.currentlySelectedItemData.equipment.weaponSlot != WeaponSlot.None && invUI.currentlySelectedItemData.equipment.weaponSlot == thisWeaponSlot)
                    || (thisWeaponSlot != WeaponSlot.None && thisWeaponSlot != WeaponSlot.Ranged
                        && (invUI.currentlySelectedItemData.equipment.weaponSlot == WeaponSlot.WeaponLeft || invUI.currentlySelectedItemData.equipment.weaponSlot == WeaponSlot.WeaponRight)))
                {
                    AddItem(invUI.currentlySelectedItemData.equipment, invUI.currentlySelectedItemData); // Add the item and data to this slot
                    SetQuiverStackSizeText(); // If this is the quiver slot, set the stack size text

                    if (invUI.invSlotMovingFrom != null) // Clear out the moving from slot if this item came from the inventory
                    {
                        inv.ClearParentAndChildSlots(invUI.invSlotMovingFrom);
                        invUI.invSlotMovingFrom.ClearSlot();
                    }
                    else if (invUI.equipSlotMovingFrom != null)
                        ClearSlot(invUI.equipSlotMovingFrom);

                    equipmentManager.EquipItem(equipment, itemData, thisWeaponSlot, thisEquipmentSlot); // Equip the item
                    
                    invUI.StopDraggingInvItem(); // Set the appropriate variables to null
                }
                else // If we this item doesn't go here
                {
                    Debug.Log(invUI.currentlySelectedItem.name + " doesn't go in this slot.");
                }
            }
            else // If this item is not of the equipment type
            {
                Debug.Log("Cannot equip this item.");
            }
        }
        else if (isEmpty == false && invUI.invSlotMovingFrom == invUI.tempSlot && invUI.currentlySelectedItem.itemType != ItemType.Ammunition) // If we're placing an item in the same slot it came from, but the slot already has an item in it (replace item)
        {
            equipmentManager.Unequip(equipment, itemData, thisWeaponSlot, thisEquipmentSlot, false); // Unequip the item before we do anything else

            itemData.SwapData(itemData, invUI.tempSlot.itemData); // Swap this slots info with the temp slots info

            // Assign the appropriate items to both slots
            equipment = itemData.equipment;
            invUI.tempSlot.item = invUI.tempSlot.itemData.equipment;

            iconImage.sprite = itemData.inventoryIcon;

            slotBackgroundImage.color = Color.white;
            iconImage.name = itemData.name;
            invUI.tempSlot.iconImage.sprite = invUI.tempSlot.itemData.inventoryIcon;

            SetQuiverStackSizeText(); // If this is the quiver slot, set the stack size text

            equipmentManager.EquipItem(equipment, itemData, thisWeaponSlot, thisEquipmentSlot); // Equip the item

            // Set our currently selected variables
            invUI.currentlySelectedItem = invUI.tempSlot.item;
            invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
        }
        // If this slot has an item in it and we have an item from the inventory or another equip slot selected already (replace item)
        else if (isEmpty == false && (invUI.invSlotMovingFrom != null || invUI.equipSlotMovingFrom != null))
        {
            if (invUI.currentlySelectedItemData.equipment != null)
            {
                // If this item belongs in this slot
                if (invUI.currentlySelectedItemData.equipment.equipmentSlot != EquipmentSlot.None && invUI.currentlySelectedItemData.equipment.equipmentSlot == thisEquipmentSlot
                    || invUI.currentlySelectedItemData.equipment.weaponSlot != WeaponSlot.None && invUI.currentlySelectedItemData.equipment.weaponSlot == thisWeaponSlot
                    || (thisWeaponSlot != WeaponSlot.None && thisWeaponSlot != WeaponSlot.Ranged
                        && (invUI.currentlySelectedItemData.equipment.weaponSlot == WeaponSlot.WeaponLeft || invUI.currentlySelectedItemData.equipment.weaponSlot == WeaponSlot.WeaponRight)))
                {
                    equipmentManager.Unequip(equipment, itemData, thisWeaponSlot, thisEquipmentSlot, false); // Unequip the item before we do anything else

                    // Add the currently selected item and its data to a temp slot
                    invUI.tempSlot.AddItem(equipment, itemData);
                    itemData.TransferData(itemData, invUI.tempSlot.itemData);

                    // Clear this slot out and add the currently selected item
                    ClearSlot(this);
                    AddItem(invUI.currentlySelectedItemData.equipment, invUI.currentlySelectedItemData);
                    SetQuiverStackSizeText(); // If this is the quiver slot, set the stack size text

                    // Clear out the moving from slot if the item came from the inventory
                    if (invUI.invSlotMovingFrom != null)
                    {
                        inv.ClearParentAndChildSlots(invUI.invSlotMovingFrom);
                        invUI.invSlotMovingFrom.ClearSlot();
                    }
                    else if (invUI.equipSlotMovingFrom != null && invUI.equipSlotMovingFrom != this)
                        ClearSlot(invUI.equipSlotMovingFrom);

                    // Set the appropriate currently selected item and moving from slot variables
                    invUI.currentlySelectedItem = invUI.tempSlot.item;
                    invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
                    invUI.equipSlotMovingFrom = null;
                    invUI.invSlotMovingFrom = invUI.tempSlot;

                    equipmentManager.EquipItem(equipment, itemData, thisWeaponSlot, thisEquipmentSlot); // Equip the item
                }
                // If we're trying to add ammo to the quiver
                else if (isEmpty == false && invUI.invSlotMovingFrom != null && invUI.currentlySelectedItemData != null 
                    && thisEquipmentSlot == EquipmentSlot.Quiver && invUI.currentlySelectedItemData.equipment.itemType == ItemType.Ammunition)
                {
                    int currentlySelectedItemStackSize = invUI.currentlySelectedItemData.currentStackSize;
                    for (int i = 0; i < currentlySelectedItemStackSize; i++)
                    {
                        if (itemData.currentAmmoCount < equipment.maxAmmo)
                        {
                            itemData.currentAmmoCount++;
                            invUI.currentlySelectedItemData.currentStackSize--;
                            itemData.ammoTypePrefab = invUI.currentlySelectedItem.prefab;
                        }

                        if (invUI.currentlySelectedItemData.currentStackSize <= 0)
                        {
                            invUI.invSlotMovingFrom.ClearSlot();
                            invUI.StopDraggingInvItem();
                        }
                    }

                    SetQuiverAmmoSprites();
                    SetQuiverStackSizeText();
                }
                else // If we this item doesn't go here
                {
                    Debug.Log(invUI.currentlySelectedItem.name + " doesn't go in this slot.");
                }
            }
            else // If this item is not of the equipment type
            {
                Debug.Log("Cannot equip this item.");
            }
        }
    }

    public void ClearSlot(EquipSlot slotToClear)
    {
        if (slotToClear.iconImage != null)
        {
            slotToClear.isEmpty = true;
            slotToClear.slotBackgroundImage.sprite = emptySlotSprite;
            slotToClear.itemData = null;
            slotToClear.equipment = null;
            slotToClear.slotText.enabled = true;
            Destroy(slotToClear.iconImage.gameObject);
        }
    }

    /// <summary>Set the slot to appear full and isEmpty to false (does not transfer any data).</summary>
    public void SoftFillSlot(EquipSlot slotToFill)
    {
        slotToFill.isEmpty = false;
        slotToFill.slotBackgroundImage.color = Color.white;
        slotToFill.slotBackgroundImage.sprite = fullSlotSprite;

        slotToFill.iconImage.sprite = slotToFill.itemData.inventoryIcon;
        slotToFill.slotText.enabled = false;
    }

    public void AddItem(Equipment newItem, ItemData itemDataToTransferDataFrom)
    {
        GameObject newIcon = Instantiate(iconPrefab, transform.GetChild(0).transform, true);
        iconImage = newIcon.GetComponent<Image>();
        itemData = iconImage.GetComponent<ItemData>();
        newIcon.transform.position = transform.position;

        RectTransform newIconRectTransform = newIcon.GetComponent<RectTransform>();
        newIconRectTransform.sizeDelta = new Vector2(newItem.iconWidth, newItem.iconHeight);
        newIconRectTransform.localScale = new Vector3(67.5f, 67.5f, 67.5f);

        isEmpty = false;
        equipment = newItem;
        
        iconImage.preserveAspect = true;
        iconImage.sprite = itemData.inventoryIcon;
        slotBackgroundImage.sprite = fullSlotSprite;

        itemData.TransferData(itemDataToTransferDataFrom, itemData); // Transfer the item's data to this slot
        SoftFillSlot(this); // Give the slot the appropriate background/icon and set isEmpty to false
        
        iconImage.name = itemData.itemName;
    }

    public void SetQuiverStackSizeText()
    {
        if (thisEquipmentSlot == EquipmentSlot.Quiver)
        {
            if (itemData.currentAmmoCount > 0)
                quiverText.text = itemData.currentAmmoCount.ToString();
            else
                quiverText.text = "";
        }
    }

    public void SetQuiverAmmoSprites()
    {
        if (itemData.equipment != null && thisEquipmentSlot == EquipmentSlot.Quiver)
        {
            switch (itemData.currentAmmoCount)
            {
                case 0:
                    itemData.gameSprite = itemData.item.possibleSprites[0]; // 0 arrows
                    itemData.inventoryIcon = itemData.item.inventoryIcons[0];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
                case 1:
                    itemData.gameSprite = itemData.item.possibleSprites[1]; // 1 arrow
                    itemData.inventoryIcon = itemData.item.inventoryIcons[1];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
                case 2:
                    itemData.gameSprite = itemData.item.possibleSprites[2]; // 2 arrows
                    itemData.inventoryIcon = itemData.item.inventoryIcons[2];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
                case 3:
                    itemData.gameSprite = itemData.item.possibleSprites[3]; // 3 arrows
                    itemData.inventoryIcon = itemData.item.inventoryIcons[3];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
                case 4:
                    itemData.gameSprite = itemData.item.possibleSprites[4]; // 4 arrows
                    itemData.inventoryIcon = itemData.item.inventoryIcons[4];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
                default:
                    itemData.gameSprite = itemData.item.possibleSprites[5]; // 5 arrows
                    itemData.inventoryIcon = itemData.item.inventoryIcons[5];
                    iconImage.sprite = itemData.inventoryIcon;
                    break;
            }
        }
    }

    void FollowMouse()
    {
        if (invUI.currentlySelectedItemData != null && invUI.equipSlotMovingFrom == this)
        {
            if (invUI.currentlySelectedItem.iconWidth == 1)
                xPosOffset = 0;
            else if (invUI.currentlySelectedItem.iconWidth == 2)
                xPosOffset = 0.5f;
            else
                xPosOffset = 1;

            if (invUI.currentlySelectedItem.iconHeight == 1)
                yPosOffset = 0;
            else if (invUI.currentlySelectedItem.iconHeight == 2)
                yPosOffset = 0.5f;
            else
                yPosOffset = 1;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            iconImage.transform.position = new Vector3(mousePos.x + xPosOffset, mousePos.y - yPosOffset, 0);
            Cursor.visible = false;
        }
    }
}
