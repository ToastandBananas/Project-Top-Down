using UnityEngine;

public enum ConsumableType { Food, Potion }

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    [HideInInspector] public int minBaseFreshness = 25;
    [HideInInspector] public int maxBaseFreshness = 100;

    [Header("Consumable Stats")]
    public ConsumableType consumableType;
    public int nourishment;
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
        if (nourishment > 0)
            Debug.Log("Mmm tasty...TODO: Implement hunger");
        if (healAmount > 0)
            userStats.Heal(healAmount);
        if (staminaRecoveryAmount > 0)
            userStats.RestoreStamina(staminaRecoveryAmount);
        if (manaRecoveryAmount > 0)
            userStats.RestoreMana(manaRecoveryAmount);

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
}
