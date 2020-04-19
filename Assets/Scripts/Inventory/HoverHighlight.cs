using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventorySlot thisInvSlot;
    EquipSlot thisEquipSlot;
    InventorySlot[] slotsToFill;

    InventoryUI invUI;
    Inventory inv;
    List<InventorySlot> invSlots = new List<InventorySlot>();
    
    int itemsOverlappingCount;
    int totalSlotsToCheck;

    void Start()
    {
        thisInvSlot = GetComponent<InventorySlot>();
        thisEquipSlot = GetComponent<EquipSlot>();

        invUI = InventoryUI.instance;
        inv = Inventory.instance;

        if (thisInvSlot != null && thisInvSlot.slotParent != null)
        {
            if (thisInvSlot.slotParent.name == "Pockets")
                invSlots = invUI.pocketsSlots;
            else if (thisInvSlot.slotParent.name == "Bag")
                invSlots = invUI.bagSlots;
            else if (thisInvSlot.slotParent.name == "Horse Bags")
                invSlots = invUI.horseBagSlots;
            else if (thisInvSlot.slotParent.name == "Container")
                invSlots = invUI.containerSlots;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (invUI.currentlySelectedItem != null && GameManager.instance.isUsingController == false)
            Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveHighlight(slotsToFill);
    }

    public void Highlight()
    {
        if (thisInvSlot != null)
            HighlightInvSlots();
        else if (thisEquipSlot != null)
            HighlightEquipSlots();
    }

    public void RemoveHighlight(InventorySlot[] inventorySlots)
    {
        if (thisInvSlot != null && inventorySlots != null && inventorySlots.Length > 0)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot != null)
                    TurnSlotWhite(slot, null);
            }
        }
        else if (thisEquipSlot != null)
            TurnSlotWhite(null, thisEquipSlot);
    }

    public void HighlightInvSlots()
    {
        totalSlotsToCheck = invUI.currentlySelectedItem.iconWidth * invUI.currentlySelectedItem.iconHeight;
        slotsToFill = new InventorySlot[totalSlotsToCheck];

        itemsOverlappingCount = inv.GetOverlappingItemCount(invUI.currentlySelectedItem, thisInvSlot, slotsToFill, invSlots);
        
        if (itemsOverlappingCount < 2)
        {
            foreach (InventorySlot slot in slotsToFill)
            {
                if (slot != null)
                    TurnSlotGreen(slot, null);
            }
        }
        else // If we can't place an item here
        {
            foreach (InventorySlot slot in slotsToFill)
            {
                if (slot != null)
                    TurnSlotRed(slot, null);
            }
        }
    }

    public void HighlightEquipSlots()
    {
        // If this item goes in this slot
        if ((invUI.currentlySelectedItemData.equipment != null && invUI.currentlySelectedItemData.equipment.equipmentSlot == thisEquipSlot.thisEquipmentSlot
            && invUI.currentlySelectedItemData.equipment.weaponSlot == thisEquipSlot.thisWeaponSlot) 
            || (invUI.currentlySelectedItem.itemType == ItemType.Ammunition && thisEquipSlot.thisEquipmentSlot == EquipmentSlot.Quiver && thisEquipSlot.isEmpty == false))
        {
            TurnSlotGreen(null, thisEquipSlot);
        }
        // If this item is a weapon or shield and this slot is a weapon/shield slot
        else if ((thisEquipSlot.thisWeaponSlot == WeaponSlot.WeaponLeft || thisEquipSlot.thisWeaponSlot == WeaponSlot.WeaponRight)
            && (invUI.currentlySelectedItem.itemType == ItemType.Weapon || invUI.currentlySelectedItem.itemType == ItemType.Shield))
        {
            TurnSlotGreen(null, thisEquipSlot);
        }
        // If this item is a ring and this slot is a ring slot
        else if ((thisEquipSlot.thisEquipmentSlot == EquipmentSlot.LeftRing || thisEquipSlot.thisEquipmentSlot == EquipmentSlot.RightRing)
            && invUI.currentlySelectedItemData.equipment.armorType == ArmorType.Ring)
        {
            TurnSlotGreen(null, thisEquipSlot);
        }
        else // If this item doesn't go in this equip slot
        {
            TurnSlotRed(null, thisEquipSlot);
        }
    }

    void TurnSlotRed(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null)
        {
            invSlot.slotBackgroundImage.sprite = thisInvSlot.emptySlotSprite;
            invSlot.slotBackgroundImage.color = Color.red;
        }
        else if (equipSlot != null)
        {
            equipSlot.slotBackgroundImage.sprite = thisEquipSlot.emptySlotSprite;
            equipSlot.slotBackgroundImage.color = Color.red;
        }
    }

    void TurnSlotGreen(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null)
        {
            invSlot.slotBackgroundImage.sprite = thisInvSlot.emptySlotSprite;
            invSlot.slotBackgroundImage.color = Color.green;
        }
        else if (equipSlot != null)
        {
            equipSlot.slotBackgroundImage.sprite = thisEquipSlot.emptySlotSprite;
            equipSlot.slotBackgroundImage.color = Color.green;
        }
    }

    void TurnSlotWhite(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null)
        {
            if (invSlot.isEmpty == false)
                invSlot.slotBackgroundImage.sprite = thisInvSlot.fullSlotSprite;

            invSlot.slotBackgroundImage.color = Color.white;
        }
        else if (equipSlot != null)
        {
            if (equipSlot.isEmpty == false)
                equipSlot.slotBackgroundImage.sprite = thisEquipSlot.fullSlotSprite;

            equipSlot.slotBackgroundImage.color = Color.white;
        }
    }
}
