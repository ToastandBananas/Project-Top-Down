using UnityEngine.UI;
using UnityEngine;
using System.Text;

public class Tooltip : MonoBehaviour
{
    StringBuilder stringBuilder = new StringBuilder(50);
    Text tooltipText;
    InventorySlot tooltipSlot;
    RectTransform rectTransform;
    Canvas canvas;
    InventoryUI invUI;

    Vector3 offset;

    // Start is called before the first frame update
    void Awake()
    {
        tooltipText = GetComponentInChildren<Text>();
        rectTransform = GetComponentInParent<RectTransform>();
        canvas = GameObject.Find("Tooltip Canvas").GetComponent<Canvas>();
        invUI = InventoryUI.instance;
    }

    public void ShowItemTooltip(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null && invSlot.isEmpty == false) // For inventorySlots
        {
            // Get the slot we'll be gathering data from and displaying
            tooltipSlot = invSlot.GetParentSlot(invSlot);

            // Item name
            stringBuilder.Append("<b><size=24>" + tooltipSlot.itemData.itemName + "</size></b>\n");

            // Stack size add on for ammunition description
            if (tooltipSlot.item.itemType == ItemType.Ammunition)
                stringBuilder.Append(tooltipSlot.itemData.currentStackSize + " ");

            // Description
            stringBuilder.Append(tooltipSlot.item.description + "\n");

            // Weapon specific info
            if (tooltipSlot.item.itemType == ItemType.Weapon)
            {
                stringBuilder.Append("Damage: " + tooltipSlot.itemData.damage + "\n");
            }
            // Armor specific info
            else if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Shield)
            {
                stringBuilder.Append("Defense: " + tooltipSlot.itemData.defense + "\n");
            }

            // Weapon/Equipment specific info
            if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Weapon || tooltipSlot.item.itemType == ItemType.Shield)
            {
                stringBuilder.Append("Durability: " + tooltipSlot.itemData.durability.ToString() + "/" + tooltipSlot.itemData.maxDurability + "\n");
            }

            // Consumable info
            if (tooltipSlot.item.itemType == ItemType.Consumable)
            {
                stringBuilder.Append("\n");

                if (tooltipSlot.itemData.consumable.consumableType == ConsumableType.Food)
                    stringBuilder.Append("Freshness: " + tooltipSlot.itemData.freshness + "%\n");

                if (tooltipSlot.itemData.consumable.consumableType == ConsumableType.Drink)
                    stringBuilder.Append("Uses: " + tooltipSlot.itemData.uses + "\n");

                stringBuilder.Append("\n<b>Effects:</b>\n");

                if (tooltipSlot.itemData.consumable.nourishment > 0)
                    stringBuilder.Append("    + " + tooltipSlot.itemData.consumable.nourishment + " nourishment\n");
                if (tooltipSlot.itemData.consumable.healAmount > 0)
                    stringBuilder.Append("    + " + tooltipSlot.itemData.consumable.healAmount + " health\n");
                if (tooltipSlot.itemData.consumable.staminaRecoveryAmount > 0)
                    stringBuilder.Append("    + " + tooltipSlot.itemData.consumable.staminaRecoveryAmount + " stamina\n");
                if (tooltipSlot.itemData.consumable.manaRecoveryAmount > 0)
                    stringBuilder.Append("    + " + tooltipSlot.itemData.consumable.manaRecoveryAmount + " mana\n");

                stringBuilder.Append("\n");
            }

            // Weight
            stringBuilder.Append("Weight: " + tooltipSlot.item.weight + "\n");

            // Value
            stringBuilder.Append("Value: " + tooltipSlot.itemData.value);

            tooltipText.text = stringBuilder.ToString();

            CalculateOffset(tooltipSlot.item, equipSlot); // Get our tooltip's position offset
            transform.position = tooltipSlot.transform.position + offset; // Reposition the tooltip to the item's slot + the offset
        }
        else if (equipSlot != null && equipSlot.isEmpty == false) // For equipSlots
        {
            // Item name
            stringBuilder.Append("<b><size=24>" + equipSlot.itemData.itemName + "</size></b>\n");

            // Description
            stringBuilder.Append(equipSlot.equipment.description + "\n");

            // Weapon specific info
            if (equipSlot.equipment.itemType == ItemType.Weapon)
            {
                stringBuilder.Append("Damage: " + equipSlot.itemData.damage + "\n");
            }
            // Armor specific info
            else if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Shield)
            {
                stringBuilder.Append("Defense: " + equipSlot.itemData.defense + "\n");
            }

            // Weapon/Equipment specific info
            if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Weapon || equipSlot.equipment.itemType == ItemType.Shield)
            {
                stringBuilder.Append("Durability: " + equipSlot.itemData.durability + "/" + equipSlot.itemData.maxDurability + "\n");
            }

            // Weight
            stringBuilder.Append("Weight: " + equipSlot.equipment.weight + "\n");

            // Value
            stringBuilder.Append("Value: " + equipSlot.itemData.value);

            tooltipText.text = stringBuilder.ToString();

            CalculateOffset(equipSlot.equipment, equipSlot); // Get our tooltip's position offset
            transform.position = equipSlot.transform.position + offset; // Reposition the tooltip to the item's slot + the offset
        }
        
        RecalculateTooltipSize();
    }

    public void ClearTooltip()
    {
        stringBuilder.Clear();
        //tooltipText.text = "";
        gameObject.SetActive(false);
    }

    void CalculateOffset(Item item, EquipSlot equipSlot)
    {
        if (equipSlot == null)
        {
            // For invSlots
            if (item.iconWidth == 1)
                offset = new Vector3(0.55f, 0.55f);
            else if (item.iconWidth == 2)
                offset = new Vector3(1.65f, 0.55f);
            else if (item.iconWidth == 3)
                offset = new Vector3(2.75f, 0.55f);

            // If the tooltip is going to be too far to the right
            if (tooltipSlot.slotParent != invUI.containerParent && tooltipSlot.slotCoordinate.x > 3)
                offset += new Vector3(-5.15f, 0);

            // Get our mouse position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos);

            // If the tooltip is going to be too close to the bottom
            if (pos.y < -260.0f)
                offset += new Vector3(0, 2f);
        }
        else
        {
            // For equipSlots
            offset = new Vector3(0.75f, 0.75f);

            if (equipSlot.thisEquipmentSlot == EquipmentSlot.Boots || equipSlot.thisEquipmentSlot == EquipmentSlot.Quiver)
                offset += new Vector3(0, 2f);
        }
    }

    void RecalculateTooltipSize()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
