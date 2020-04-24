using UnityEngine;

public enum ConsumableType { Food, Drink }

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    [HideInInspector] public int minBaseFreshness = 25;
    [HideInInspector] public int maxBaseFreshness = 100;

    [Header("Consumable Stats")]
    public ConsumableType consumableType;
    public int maxUses = 1;
    public int nourishment;
    public int thirstQuench;
    public int energyRestoration;
    public int healAmount;
    public int staminaRecoveryAmount;
    public int manaRecoveryAmount;

    BasicStats playerBasicStats;

    public override void Use(ItemData itemData, EquipmentManager equipmentManager, InventorySlot invSlot)
    {
        base.Use(itemData);

        if (playerBasicStats == null)
            playerBasicStats = PlayerMovement.instance.GetComponent<BasicStats>();

        Consume(playerBasicStats, invSlot);
    }

    public void Consume(BasicStats userStats, InventorySlot invSlot)
    {
        if (invSlot.itemData.uses > 0)
        {
            if (consumableType == ConsumableType.Food)
                AudioManager.instance.PlayRandomSound(AudioManager.instance.eatFoodSounds, userStats.transform.position);
            else if (consumableType == ConsumableType.Drink)
                AudioManager.instance.PlayRandomSound(AudioManager.instance.drinkSounds, userStats.transform.position);

            if (nourishment > 0)
                Needs.instance.ReplenishNourishment(nourishment);
            if (thirstQuench > 0)
                Needs.instance.ReplenishHydration(thirstQuench);
            if (energyRestoration > 0)
                Needs.instance.ReplenishEnergy(energyRestoration);
            if (healAmount > 0)
                userStats.Heal(healAmount);
            if (staminaRecoveryAmount > 0)
                userStats.RestoreStamina(staminaRecoveryAmount);
            if (manaRecoveryAmount > 0)
                userStats.RestoreMana(manaRecoveryAmount);

            InventorySlot parentSlot = invSlot.GetParentSlot(invSlot); // Parent slot that the context menu was brought up on
            parentSlot.itemData.uses--;

            if (parentSlot.itemData.uses == 0 && parentSlot.itemData.consumable.consumableType == ConsumableType.Food)
            {
                // Clear out the slot's data
                invSlot.ClearSlot();
                for (int i = 0; i < invSlot.childrenSlots.Length; i++)
                {
                    if (invSlot.childrenSlots[i] != null)
                    {
                        invSlot.childrenSlots[i].ClearSlot();
                        invSlot.childrenSlots[i].parentSlot = null;
                        invSlot.childrenSlots[i] = null;
                    }
                }
                invSlot.parentSlot = null;
            }
            else
            {
                // Change the item's sprite
                InventorySlot slot = invSlot.GetParentSlot(invSlot); // Parent slot that the context menu was brought up on
                slot.iconImage.sprite = slot.item.inventoryIcons[slot.itemData.uses];
            }
        }
    }
}
