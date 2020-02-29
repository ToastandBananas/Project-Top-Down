using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    Text tooltipText;
    InventorySlot tooltipSlot;
    RectTransform rectTransform;
    HorizontalLayoutGroup horizontalLayoutGroup;

    Vector3 offset;

    // Start is called before the first frame update
    void Awake()
    {
        tooltipText = GetComponentInChildren<Text>();
        rectTransform = GetComponentInParent<RectTransform>();
        horizontalLayoutGroup = GetComponentInParent<HorizontalLayoutGroup>();
    }

    public void ShowItemTooltip(InventorySlot invSlot, EquipSlot equipSlot)
    {
        if (invSlot != null && invSlot.isEmpty == false)
        {
            tooltipSlot = invSlot.GetParentSlot(invSlot); // Get the slot we'll be gathering data from and displaying
            
            tooltipText.text += "<b><size=24>" + tooltipSlot.itemData.itemName + "</size></b>\n";

            if (tooltipSlot.item.itemType == ItemType.Ammunition)
                    tooltipText.text += tooltipSlot.itemData.currentStackSize.ToString() + " ";
            tooltipText.text += tooltipSlot.item.description + "\n";

            if (tooltipSlot.item.itemType == ItemType.Weapon)
            {
                tooltipText.text += "Damage: " + tooltipSlot.itemData.damage.ToString() + "\n";
            }
            else if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Shield)
            {
                tooltipText.text += "Defense: " + tooltipSlot.itemData.defense.ToString() + "\n";
            }

            if (tooltipSlot.item.itemType == ItemType.Armor || tooltipSlot.item.itemType == ItemType.Weapon || tooltipSlot.item.itemType == ItemType.Shield)
            {
                tooltipText.text += "Durability: " + tooltipSlot.itemData.durability.ToString() + "/" + tooltipSlot.itemData.maxDurability.ToString() + "\n";
            }

            tooltipText.text += "Value: " + tooltipSlot.itemData.value.ToString();

            CalculateOffset(tooltipSlot.item);
            transform.position = tooltipSlot.transform.position + offset;
        }
        else if (equipSlot != null && equipSlot.isEmpty == false)
        {
            tooltipText.text += "<b><size=24>" + equipSlot.itemData.itemName + "</size></b>\n";
            tooltipText.text += equipSlot.equipment.description + "\n";

            if (equipSlot.equipment.itemType == ItemType.Weapon)
            {
                tooltipText.text += "Damage: " + equipSlot.itemData.damage.ToString() + "\n";
            }
            else if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Shield)
            {
                tooltipText.text += "Defense: " + equipSlot.itemData.defense.ToString() + "\n";
            }

            if (equipSlot.equipment.itemType == ItemType.Armor || equipSlot.equipment.itemType == ItemType.Weapon || equipSlot.equipment.itemType == ItemType.Shield)
            {
                tooltipText.text += "Durability: " + equipSlot.itemData.durability.ToString() + "/" + equipSlot.itemData.maxDurability.ToString() + "\n";
            }

            tooltipText.text += "Value: " + equipSlot.itemData.value.ToString();

            CalculateOffset(equipSlot.equipment);
            transform.position = equipSlot.transform.position + offset;
        }

        RecalculateTooltipSize();
    }

    public void ClearTooltip()
    {
        tooltipText.text = "";
        gameObject.SetActive(false);
    }

    void CalculateOffset(Item item)
    {
        if (item.iconWidth == 1)
            offset = (Vector3.up * 0.55f) + (Vector3.right * 0.55f);
        else if (item.iconWidth == 2)
            offset = (Vector3.up * 0.55f) + (Vector3.right * 1.65f);
        else if (item.iconWidth == 3)
            offset = (Vector3.up * 0.55f) + (Vector3.right * 2.75f);
    }

    void RecalculateTooltipSize()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
