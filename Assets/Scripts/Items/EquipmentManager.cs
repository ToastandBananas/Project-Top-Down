using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class EquipmentManager : MonoBehaviour
{
    [Header("Weapon Base GO Prefabs")]
    public GameObject prefabWeaponBase1H_L;
    public GameObject prefabWeaponBase1H_R;
    public GameObject prefabShieldBase_L;
    public GameObject prefabShieldBase_R;
    public GameObject prefabRangedWeaponBase;

    [Header("Currently Equipped Lists")]
    public Equipment[] currentWeapons;
    public Equipment[] currentEquipment;

    [Header("Weapon Sprite Renderers")]
    public Transform leftWeaponParent;
    public Transform rightWeaponParent;
    public SpriteRenderer leftWeapon;
    public SpriteRenderer rightWeapon;
    public SpriteRenderer rangedWeapon;

    [Header("Equipment Sprite Renderers")]
    public SpriteRenderer helmet;
    public SpriteRenderer cuirass;
    public SpriteRenderer shirt;
    public SpriteRenderer leftCuirassArm;
    public SpriteRenderer rightCuirassArm;
    public SpriteRenderer leftGauntlet;
    public SpriteRenderer rightGauntlet;
    public SpriteRenderer leftGreaves;
    public SpriteRenderer rightGreaves;
    public SpriteRenderer leftPants;
    public SpriteRenderer rightPants;
    public SpriteRenderer leftBoot;
    public SpriteRenderer rightBoot;

    [Header("Quiver")]
    public EquipSlot quiverSlot;

    Inventory inv;
    Arms arms;
    BasicStats basicStats;

    int slotIndex = 0;
    Equipment oldItem = null;

    void Start()
    {
        inv = Inventory.instance;
        arms = transform.Find("Arms").GetComponent<Arms>();
        basicStats = GetComponent<BasicStats>();

        int numWeaponSlots = System.Enum.GetNames(typeof(WeaponSlot)).Length;
        currentWeapons = new Equipment[numWeaponSlots];

        int numEquipmentSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numEquipmentSlots];
    }

    public void EquipItem(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        oldItem = null; // Reset the oldItem

        if (weaponSlot != WeaponSlot.None) // If the item is a weapon
        {
            slotIndex = (int)weaponSlot; // Grab the index number of this weaponSlot

            if (currentWeapons[slotIndex] != null) // Make a reference to our old weapon if we already had something equipped in this slot
                oldItem = currentWeapons[slotIndex];

            currentWeapons[slotIndex] = newItem; // Assign the weapon to our currentWeapons array
        }
        else if (equipmentSlot != EquipmentSlot.None) // If the item is a piece of equipment
        {
            slotIndex = (int)equipmentSlot; // Grab the index number of this equipmentSlot

            if (currentEquipment[slotIndex] != null) // Make a reference to our old armor if we already had something equipped in this slot
                oldItem = currentEquipment[slotIndex];

            currentEquipment[slotIndex] = newItem; // Assign the equipment to our currentEquipment array
        }

        EquipToCharacter(newItem, itemData, weaponSlot, equipmentSlot); // Physically equip the weapon/equipment on the character
    }

    public void AutoEquip(Equipment newItem, ItemData itemData)
    {
        Equipment oldItem = null;

        int slotIndex = 0;
        if (newItem.weaponType != WeaponType.NotAWeapon)
        {
            if (newItem.weaponSlot != WeaponSlot.None)
            {
                if (newItem.weaponType == WeaponType.Bow || newItem.weaponType == WeaponType.Crossbow)
                    newItem.weaponSlot = WeaponSlot.Ranged;
                else
                {
                    newItem.weaponSlot = WeaponSlot.WeaponRight;
                    if (currentWeapons[(int)WeaponSlot.WeaponRight] != null)
                        newItem.weaponSlot = WeaponSlot.WeaponLeft;
                }
            }

            slotIndex = (int)newItem.weaponSlot;

            if (currentWeapons[slotIndex] != null)
                oldItem = currentWeapons[slotIndex];

            currentWeapons[slotIndex] = newItem;
        }
        else if (newItem.armorType == ArmorType.Ring)
        {
            newItem.equipmentSlot = EquipmentSlot.LeftRing;
            if (currentEquipment[(int)EquipmentSlot.LeftRing] != null)
                newItem.equipmentSlot = EquipmentSlot.RightRing;
            if (newItem.equipmentSlot == EquipmentSlot.RightRing && currentEquipment[(int)EquipmentSlot.RightRing] != null)
                newItem.equipmentSlot = EquipmentSlot.LeftRing;

            slotIndex = (int)newItem.equipmentSlot;

            if (currentEquipment[slotIndex] != null)
                oldItem = currentEquipment[slotIndex];

            currentEquipment[slotIndex] = newItem;
        }
        else
        {
            slotIndex = (int)newItem.equipmentSlot;

            if (currentEquipment[slotIndex] != null)
                oldItem = currentEquipment[slotIndex];

            currentEquipment[slotIndex] = newItem;
        }
    }

    void EquipToCharacter(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        if (weaponSlot != WeaponSlot.None) // If this is a weapon
        {
            switch (weaponSlot) // Determine which slot to change
            {
                case WeaponSlot.WeaponLeft:
                    EquipWeapon(newItem, itemData, prefabWeaponBase1H_L, leftWeaponParent);
                    break;
                case WeaponSlot.WeaponRight:
                    EquipWeapon(newItem, itemData, prefabWeaponBase1H_R, rightWeaponParent);
                    break;
                case WeaponSlot.Ranged:
                    EquipWeapon(newItem, itemData, prefabRangedWeaponBase, leftWeaponParent);
                    break;
            }
        }
        else if (equipmentSlot != EquipmentSlot.None) // If this is a piece of armor
        {
            switch (equipmentSlot) // Determine which slot to change
            {
                case EquipmentSlot.Head:
                    EquipArmor(newItem, itemData, helmet);
                    break;
                case EquipmentSlot.Shirt:
                    EquipArmor(newItem, itemData, shirt);
                    break;
                case EquipmentSlot.Cuirass:
                    EquipArmor(newItem, itemData, cuirass);
                    break;
                case EquipmentSlot.Gauntlets:
                    EquipArmor(newItem, itemData, leftGauntlet);
                    EquipArmor(newItem, itemData, rightGauntlet);
                    break;
                case EquipmentSlot.Pants:
                    EquipArmor(newItem, itemData, leftPants);
                    EquipArmor(newItem, itemData, rightPants);
                    break;
                case EquipmentSlot.Greaves:
                    EquipArmor(newItem, itemData, leftGreaves);
                    EquipArmor(newItem, itemData, rightGreaves);
                    break;
                case EquipmentSlot.Boots:
                    EquipArmor(newItem, itemData, leftBoot);
                    EquipArmor(newItem, itemData, rightBoot);
                    break;
                case EquipmentSlot.Quiver:
                    quiverSlot.SetQuiverStackSizeText();
                    // TODO: Make visible quiver on character? // EquipArmor(newItem, itemData, quiver, true);
                    break;
                case EquipmentSlot.Belt:
                    break;
                case EquipmentSlot.RightRing:
                    break;
                case EquipmentSlot.LeftRing:
                    break;
                case EquipmentSlot.Amulet:
                    break;
            }

            AdjustStats(itemData, basicStats);
        }
        else
            Debug.LogError("Either a valid weaponSlot or equipmentSlot needs passed into this function to work.");
    }

    void EquipWeapon(Equipment newItem, ItemData itemData, GameObject weaponBasePrefab, Transform weaponParent)
    {
        GameObject weaponBase = Instantiate(weaponBasePrefab, weaponParent);
        GameObject weapon = Instantiate(newItem.prefab, weaponBase.transform);
        weapon.name = itemData.itemName;
        weapon.GetComponent<SpriteRenderer>().sprite = newItem.sprite;
        weapon.GetComponent<BoxCollider2D>().enabled = true;
        weapon.GetComponent<WeaponDamage>().enabled = true;
        
        if (weaponParent == leftWeaponParent && newItem.generalClassification == GeneralClassification.Weapon1H)
            arms.leftWeaponEquipped = true;
        else if (weaponParent == leftWeaponParent && newItem.generalClassification == GeneralClassification.Shield)
            arms.leftShieldEquipped = true;
        else if (weaponParent == rightWeaponParent && newItem.generalClassification == GeneralClassification.Weapon1H)
            arms.rightWeaponEquipped = true;
        else if (weaponParent == rightWeaponParent && newItem.generalClassification == GeneralClassification.Shield)
            arms.rightShieldEquipped = true;
        else if (newItem.generalClassification == GeneralClassification.Weapon2H)
            arms.twoHanderEquipped = true;
        else if (newItem.generalClassification == GeneralClassification.RangedWeapon)
            arms.rangedWeaponEquipped = true;

        StartCoroutine(SetArmAnims());
    }

    void EquipArmor(Equipment newItem, ItemData itemData, SpriteRenderer armorSpriteRenderer)
    {
        armorSpriteRenderer.sprite = newItem.sprite;

        if (newItem.armorType == ArmorType.Cuirass)
        {
            if (newItem.leftCuirassArmSprite != null && newItem.rightCuirassArmSpite != null)
            {
                leftCuirassArm.sprite = newItem.leftCuirassArmSprite;
                rightCuirassArm.sprite = newItem.rightCuirassArmSpite;
            }
            else
                Debug.LogWarning("Left and/or right cuirass arm sprite not set for this cuirass. Does it need to be?");
        }
    }

    public void Unequip(Equipment item, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        if (weaponSlot != WeaponSlot.None) // If the item is a weapon
        {
            slotIndex = (int)weaponSlot; // Grab the index number of this weaponSlot

            currentWeapons[slotIndex] = null; // Assign the weapon to our currentWeapons array

            switch (weaponSlot) // Determine which slot to change
            {
                case WeaponSlot.WeaponLeft:
                    if (leftWeaponParent.childCount > 0)
                        Destroy(leftWeaponParent.GetChild(0).gameObject);
                    break;
                case WeaponSlot.WeaponRight:
                    if (rightWeaponParent.childCount > 0)
                        Destroy(rightWeaponParent.GetChild(0).gameObject);
                    break;
                case WeaponSlot.Ranged:
                        if (leftWeaponParent.childCount > 0)
                    Destroy(leftWeaponParent.GetChild(0).gameObject);
                    break;
            }

            RemoveStats(itemData, basicStats);

            if (weaponSlot == WeaponSlot.WeaponLeft && item.generalClassification == GeneralClassification.Weapon1H)
                arms.leftWeaponEquipped = false;
            else if (weaponSlot == WeaponSlot.WeaponLeft && item.generalClassification == GeneralClassification.Shield)
                arms.leftShieldEquipped = false;
            else if (weaponSlot == WeaponSlot.WeaponRight && item.generalClassification == GeneralClassification.Weapon1H)
                arms.rightWeaponEquipped = false;
            else if (weaponSlot == WeaponSlot.WeaponRight && item.generalClassification == GeneralClassification.Shield)
                arms.rightShieldEquipped = false;
            else if (item.generalClassification == GeneralClassification.Weapon2H)
                arms.twoHanderEquipped = false;
            else if (item.generalClassification == GeneralClassification.RangedWeapon)
                arms.rangedWeaponEquipped = false;

            StartCoroutine(SetArmAnims());
        }
        else if (equipmentSlot != EquipmentSlot.None) // If the item is a piece of equipment
        {
            slotIndex = (int)equipmentSlot; // Grab the index number of this equipmentSlot

            currentEquipment[slotIndex] = null; // Assign the equipment to our currentEquipment array

            switch (equipmentSlot) // Determine which slot to change
            {
                case EquipmentSlot.Head:
                    helmet.sprite = null;
                    break;
                case EquipmentSlot.Shirt:
                    shirt.sprite = null;
                    break;
                case EquipmentSlot.Cuirass:
                    cuirass.sprite = null;
                    break;
                case EquipmentSlot.Gauntlets:
                    leftGauntlet.sprite = null;
                    rightGauntlet.sprite = null;
                    break;
                case EquipmentSlot.Pants:
                    leftPants.sprite = null;
                    rightPants.sprite = null;
                    break;
                case EquipmentSlot.Greaves:
                    leftGreaves.sprite = null;
                    rightGreaves.sprite = null;
                    break;
                case EquipmentSlot.Boots:
                    leftBoot.sprite = null;
                    rightBoot.sprite = null;
                    break;
                case EquipmentSlot.Quiver:
                    // TODO: Make visible quiver on character? // quiver.sprite = null;
                    quiverSlot.GetComponentInChildren<Text>().text = "";
                    break;
                case EquipmentSlot.Belt:
                    break;
                case EquipmentSlot.RightRing:
                    break;
                case EquipmentSlot.LeftRing:
                    break;
                case EquipmentSlot.Amulet:
                    break;
            }

            RemoveStats(itemData, basicStats);
        }
    }

    void AdjustStats(ItemData itemData, BasicStats stats)
    {
        stats.defense += itemData.defense;
        stats.encumbrance += itemData.equipment.weight;
    }

    void RemoveStats(ItemData itemData, BasicStats stats)
    {
        stats.defense -= itemData.defense;
        stats.encumbrance -= itemData.equipment.weight;
    }

    IEnumerator SetArmAnims()
    {
        yield return new WaitForSeconds(0.1f);
        arms.SetLeftAnims();
        arms.SetRightAnims();
    }
}
