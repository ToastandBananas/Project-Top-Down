using UnityEngine;

public enum WeaponSlot { None, WeaponLeft, WeaponRight, Ranged }
public enum EquipmentSlot { None, Head, Shirt, Cuirass, Gauntlets, Pants, Greaves, Belt, Boots, LeftRing, RightRing, Amulet, Quiver, Ammunition }

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
    public Sprite leftCuirassArmSprite;
    public Sprite rightCuirassArmSpite;

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

    public override void Use(ItemData itemData, EquipmentManager equipmentManager, InventorySlot invSlot)
    {
        base.Use(itemData);

        // Equip the item
        equipmentManager.AutoEquip(this, itemData, invSlot);

        // Remove from the inventory
        RemoveFromInventory(itemData);
    }
}
