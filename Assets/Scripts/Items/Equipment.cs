using UnityEngine;

public enum WeaponSlot { None, WeaponLeft, WeaponRight, Ranged }
public enum EquipmentSlot { None, Head, Shirt, Cuirass, Gauntlets, Pants, Greaves, Belt, Boots, LeftRing, RightRing, Amulet, Quiver }

public enum GeneralClassification { Weapon1H, Weapon2H, RangedWeapon, Shield, Armor, Quiver, Ammunition }
public enum WeaponType { NotAWeapon, Shield, Sword, Mace, Axe, Spear, Staff, Bow, Crossbow }
public enum ArmorType { NotArmor, Helmet, Shirt, Cuirass, Gauntlets, Pants, Greaves, Belt, Boots, Ring, Amulet }

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

    [Header("Sprite")]
    public Sprite deathSprite;
    public Sprite leftCuirassArmSprite; // For cuirass only
    public Sprite rightCuirassArmSpite;

    [Header("Weapon Stats")]
    public int minBaseDamage = 1;
    public int maxBaseDamage = 1;
    public float damageModifier = 1f;
    public float knockbackPower = 1f;
    public int baseStaminaUse = 5;

    [Header("Armor Stats")]
    public int minBaseDefense = 1;
    public int maxBaseDefense = 1;
    public float armorModifier = 1f;

    [Header("Other Stats")]
    public int minBaseDurability = 10;
    public int maxBaseDurability = 20;

    public override void Use(ItemData itemData, EquipmentManager equipmentManager, InventorySlot invSlot)
    {
        base.Use(itemData);

        // Equip the item
        equipmentManager.AutoEquip(this, itemData, invSlot);

        // Remove from the inventory
        RemoveFromInventory(itemData);
    }
}
