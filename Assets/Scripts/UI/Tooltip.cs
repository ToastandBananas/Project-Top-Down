using UnityEngine.UI;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
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
            tooltipText.text += "<b><size=24>" + tooltipSlot.itemData.itemName + "</size></b>\n";

            // Stack size add on for ammunition description
            if (tooltipSlot.item.itemType == ItemType.Ammunition)
                    tooltipText.text += tooltipSlot.itemData.currentStackSize.ToString() + " ";

            // Description
            tooltipText.text += tooltipSlot.item.description + "\n";

            // Weapon specific info
            if (tooltipSlot.item.itemType == ItemType.Weapon)
            {
                tooltipText.text += "Damage: " + tooltipSlot.itemData.damage.ToString() + "\n";
            }
            // Armor specific info
            else if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Shield)
            {
                tooltipText.text += "Defense: " + tooltipSlot.itemData.defense.ToString() + "\n";
            }

            // Weapon/Equipment specific info
            if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Weapon || tooltipSlot.item.itemType == ItemType.Shield)
            {
                tooltipText.text += "Durability: " + tooltipSlot.itemData.durability.ToString() + "/" + tooltipSlot.itemData.maxDurability.ToString() + "\n";
            }

            // Value
            tooltipText.text += "Value: " + tooltipSlot.itemData.value.ToString();

            CalculateOffset(tooltipSlot.item, equipSlot); // Get our tooltip's position offset
            transform.position = tooltipSlot.transform.position + offset; // Reposition the tooltip to the item's slot + the offset
        }
        else if (equipSlot != null && equipSlot.isEmpty == false) // For equipSlots
        {
            // Item name
            tooltipText.text += "<b><size=24>" + equipSlot.itemData.itemName + "</size></b>\n";

            // Description
            tooltipText.text += equipSlot.equipment.description + "\n";

            // Weapon specific info
            if (equipSlot.equipment.itemType == ItemType.Weapon)
            {
                tooltipText.text += "Damage: " + equipSlot.itemData.damage.ToString() + "\n";
            }
            // Armor specific info
            else if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Shield)
            {
                tooltipText.text += "Defense: " + equipSlot.itemData.defense.ToString() + "\n";
            }

            // Weapon/Equipment specific info
            if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Weapon || equipSlot.equipment.itemType == ItemType.Shield)
            {
                tooltipText.text += "Durability: " + equipSlot.itemData.durability.ToString() + "/" + equipSlot.itemData.maxDurability.ToString() + "\n";
            }

            // Value
            tooltipText.text += "Value: " + equipSlot.itemData.value.ToString();

            CalculateOffset(equipSlot.equipment, equipSlot); // Get our tooltip's position offset
            transform.position = equipSlot.transform.position + offset; // Reposition the tooltip to the item's slot + the offset
        }
        
        RecalculateTooltipSize();
    }

    public void ClearTooltip()
    {
        tooltipText.text = "";
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
