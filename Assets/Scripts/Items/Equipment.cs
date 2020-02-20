using UnityEngine;

public enum WeaponSlot { None, PrimaryWeaponLeft, PrimaryWeaponRight, Ranged }
public enum EquipmentSlot { None, Head, Shirt, Cuirass, Gauntlets, Pants, Greaves, Socks, Boots, RingLeft, RingRight, Amulet, Quiver, Ammunition }

public enum GeneralClassification { Weapon1H, Weapon2H, RangedWeapon, Shield, Armor, Quiver, Ammunition }
public enum WeaponType { NotAWeapon, Shield, Sword, Mace, Axe, Spear, Staff, Bow, Crossbow }
public enum ArmorType { NotArmor, Helmet, Shirt, Cuirass, Gauntlets, Pants, Greaves, Socks, Boots, Ring, Amulet }

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

    public override void Use(ItemData itemData)
    {
        base.Use(itemData);

        // Equip the item
        EquipmentManager.instance.Equip(this, null);

        // Remove from the inventory
        RemoveFromInventory(itemData);
    }
}
