using UnityEngine;

public enum WeaponSlot { PRIMARY_WEAPON_LEFT, PRIMARY_WEAPON_RIGHT, SECONDARY_WEAPON_LEFT, SECONDARY_WEAPON_RIGHT }
public enum EquipmentSlot { HEAD, SHIRT, CUIRASS, GAUNTLETS, PANTS, GREAVES, SOCKS, BOOTS, RING1, RING2, AMULET }

public enum GeneralClassification { WEAPON_1H, WEAPON_2H, SHIELD, ARMOR }
public enum WeaponType { NOT_A_WEAPON, SHIELD, SWORD, MACE, AXE, SPEAR, STAFF, BOW, CROSSBOW }
public enum ArmorType { NOT_ARMOR, HELMET, SHIRT, CUIRASS, GAUNTLETS, PANTS, GREAVES, SOCKS, BOOTS, RING, AMULET }

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    [Header("Enums")]
    public GeneralClassification generalClassification;
    public WeaponType weaponType;
    public ArmorType armorType;
    public SpecialAttack[] thisWeaponsSpecialAttacks;

    [Header("Equip Slots")]
    public WeaponSlot weaponSlot;
    public EquipmentSlot equipmentSlot;

    [Header("Weapon Stats")]
    public float minBaseDamage = 1f;
    public float maxBaseDamage = 1f;
    public float damageModifier = 1f;
    public float knockbackPower = 1f;

    [Header("Armor Stats")]
    public float minBaseArmor = 1f;
    public float maxBaseArmor = 1f;
    public float armorModifier = 1f;

    [Header("Other Stats")]
    public float minBaseDurability = 10f;
    public float maxBaseDurability = 20f;

    public override void Use()
    {
        base.Use();

        // Equip the item
        EquipmentManager.instance.Equip(this, null);

        // Remove from the inventory
        RemoveFromInventory();
    }
}
