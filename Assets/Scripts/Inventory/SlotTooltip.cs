using UnityEngine.EventSystems;
using UnityEngine;

public class SlotTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventorySlot thisInvSlot;
    InventorySlot thisInvParentSlot;
    EquipSlot thisEquipSlot;

    InventoryUI invUI;

    int tooltipCount;

    void Start()
    {
        thisInvSlot = GetComponent<InventorySlot>();
        thisEquipSlot = GetComponent<EquipSlot>();
        invUI = InventoryUI.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisInvSlot != null && thisInvSlot.isEmpty == false && invUI.currentlySelectedItem == null) // If this is an invSlot
        {
            // Get the tooltip for the slot we're hovering over, regardless of its ItemType
            GetInvSlotTooltip();

            // Show tooltip(s) for weapons/equipment of the same type that are already equipped (for comparison)
            if (thisInvParentSlot.item.itemType == ItemType.Weapon)
                GetEquippedWeaponTooltips();
            else if (thisInvParentSlot.item.itemType == ItemType.Shield)
                GetEquippedShieldTooltips();
            else if (thisInvParentSlot.item.itemType == ItemType.Armor)
                GetEquippedArmorTooltips();
        }
        else if (thisEquipSlot != null && thisEquipSlot.isEmpty == false && invUI.currentlySelectedItem == null) // If this is an equipSlot
        {
            // Get the tooltip for the equipped item we're hovering over
            GetEquipSlotTooltips();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Clear and disable all tooltips
        invUI.ClearAllTooltips();
    }

    void GetInvSlotTooltip()
    {
        thisInvParentSlot = thisInvSlot.GetParentSlot(thisInvSlot); // Get the parent slot of thisInvSlot

        // Display the tooltip for the parent slot's item
        invUI.invTooltip.gameObject.SetActive(true);
        invUI.invTooltip.ShowItemTooltip(thisInvParentSlot, null);
    }
    
    void GetEquippedWeaponTooltips()
    {
        if (thisInvParentSlot.item.itemType != ItemType.Shield
            && (thisInvParentSlot.itemData.equipment.weaponSlot == WeaponSlot.WeaponLeft || thisInvParentSlot.itemData.equipment.weaponSlot == WeaponSlot.WeaponRight))
        {
            if (invUI.leftWeaponSlot.isEmpty == false)
            {
                // Display the left equipped weapon's info
                invUI.equipTooltip1.gameObject.SetActive(true);
                invUI.equipTooltip1.ShowItemTooltip(null, invUI.leftWeaponSlot);
            }
            if (invUI.rightWeaponSlot.isEmpty == false)
            {
                // Display the right equipped weapon's info
                invUI.equipTooltip2.gameObject.SetActive(true);
                invUI.equipTooltip2.ShowItemTooltip(null, invUI.rightWeaponSlot);
            }
        }
        else // If this is a ranged weapon
        {
            if (invUI.rangedWeaponSlot.isEmpty == false)
            {
                // Display the ranged weapon's info
                invUI.equipTooltip1.gameObject.SetActive(true);
                invUI.equipTooltip1.ShowItemTooltip(null, invUI.rangedWeaponSlot);
            }
        }
    }

    void GetEquippedShieldTooltips()
    {
        if (invUI.leftWeaponSlot.isEmpty == false && invUI.leftWeaponSlot.equipment.itemType == ItemType.Shield)
        {
            // Display the shield equipped on the left arm's info
            invUI.equipTooltip1.gameObject.SetActive(true);
            invUI.equipTooltip1.ShowItemTooltip(null, invUI.leftWeaponSlot);
        }

        if (invUI.rightWeaponSlot.isEmpty == false && invUI.rightWeaponSlot.equipment.itemType == ItemType.Shield)
        {
            // Display the shield equipped on the right arm's info
            invUI.equipTooltip2.gameObject.SetActive(true);
            invUI.equipTooltip2.ShowItemTooltip(null, invUI.rightWeaponSlot);
        }
    }

    void GetEquippedArmorTooltips()
    {
        Equipment invSlotEquipment = thisInvSlot.GetParentSlot(thisInvSlot).item as Equipment; // Cast the item in the inv slot as Equipment so we can access its data
        tooltipCount = 0; // Reset the tooltipCount

        foreach (EquipSlot equipSlot in invUI.equipSlots)
        {
            if (tooltipCount == 2) // No need to go through the loop anymore
                break;

            if (equipSlot.isEmpty == false)
            { 
                if((equipSlot.thisEquipmentSlot == EquipmentSlot.LeftRing || equipSlot.thisEquipmentSlot == EquipmentSlot.RightRing) // If the equipSlot is for a ring
                        && invSlotEquipment.armorType == ArmorType.Ring)// And if this inv item is a ring
                {
                    if (tooltipCount == 0) // If we haven't found an equipSlot tooltip yet
                    {
                        invUI.equipTooltip1.gameObject.SetActive(true);
                        invUI.equipTooltip1.ShowItemTooltip(null, equipSlot);
                    }
                    else // if (tooltipCount == 1) // Not needed since it will break out of the loop if (tooltipCount == 2)
                    {
                        invUI.equipTooltip2.gameObject.SetActive(true);
                        invUI.equipTooltip2.ShowItemTooltip(null, equipSlot);
                    }
                    tooltipCount++; // Add to our count so we can break out of the loop when we have both the equipSlot tooltips we need
                }
                else if (equipSlot.equipment.equipmentSlot == invSlotEquipment.equipmentSlot) // If this is a normal piece of equipment (or amulet)
                {
                    invUI.equipTooltip1.gameObject.SetActive(true);
                    invUI.equipTooltip1.ShowItemTooltip(null, equipSlot);
                    break; // We only need to find the appropriate equipSlot, display the tooltip and then we can break out of the loop
                }
            }
        }
    }

    void GetEquipSlotTooltips()
    {
        // Display the tooltip for the equipment we're hovering over
        invUI.equipTooltip1.gameObject.SetActive(true);
        invUI.equipTooltip1.ShowItemTooltip(null, thisEquipSlot);

        // Then, if it's a weapon or ring, display the other weapon or ring slot's tooltip
        if (thisEquipSlot.equipment.armorType == ArmorType.Ring) // If this is a ring slot
        {
            tooltipCount = 0; // Reset our tooltipCount

            foreach (EquipSlot equipSlot in invUI.equipSlots) // Loop through each equipSlot and look for the other ring slot
            {
                if (tooltipCount == 1) // We just need to find the other ring's slot, display it's tooltip and then break out of the loop
                    break;

                if (equipSlot.thisEquipmentSlot != thisEquipSlot.thisEquipmentSlot) // If this isn't the slot we're already hovering over
                {
                    if (tooltipCount == 0)
                    {
                        if (equipSlot.thisEquipmentSlot == EquipmentSlot.LeftRing && equipSlot.isEmpty == false) // If this is the left ring slot
                        {
                            // Display the equipped left ring's data
                            invUI.equipTooltip2.gameObject.SetActive(true);
                            invUI.equipTooltip2.ShowItemTooltip(null, equipSlot);
                            tooltipCount++;
                        }
                        else if (equipSlot.thisEquipmentSlot == EquipmentSlot.RightRing && equipSlot.isEmpty == false) // If this is the right ring slot
                        {
                            // Display the equipped right ring's data
                            invUI.equipTooltip2.gameObject.SetActive(true);
                            invUI.equipTooltip2.ShowItemTooltip(null, equipSlot);
                            tooltipCount++;
                        }
                    }
                }
            }
        }
        else if (thisEquipSlot.thisWeaponSlot == WeaponSlot.WeaponLeft || thisEquipSlot.thisWeaponSlot == WeaponSlot.WeaponRight) // If this is a left/right weapon slot
        {
            tooltipCount = 0; // Reset our tooltipCount

            foreach (EquipSlot equipSlot in invUI.weaponSlots) // Loop through each equipSlot and look for the other weapon slot
            {
                if (tooltipCount == 1) // We just need to find the other weapon's slot, display it's tooltip and then break out of the loop
                    break;

                if (equipSlot.thisWeaponSlot != thisEquipSlot.thisWeaponSlot) // If this isn't the slot we're already hovering over
                {
                    if (tooltipCount == 0)
                    {
                        if (equipSlot.thisWeaponSlot == WeaponSlot.WeaponLeft && equipSlot.isEmpty == false) // If this is the left ring slot
                        {
                            // Display the equipped left ring's data
                            invUI.equipTooltip2.gameObject.SetActive(true);
                            invUI.equipTooltip2.ShowItemTooltip(null, equipSlot);
                            tooltipCount++;
                        }
                        else if (equipSlot.thisWeaponSlot == WeaponSlot.WeaponRight && equipSlot.isEmpty == false) // If this is the right ring slot
                        {
                            // Display the equipped right ring's data
                            invUI.equipTooltip2.gameObject.SetActive(true);
                            invUI.equipTooltip2.ShowItemTooltip(null, equipSlot);
                            tooltipCount++;
                        }
                    }
                }
            }
        }
    }
}
