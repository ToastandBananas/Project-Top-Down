using System.Collections;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of EquipmentManager found!");
            Destroy(this);
        }
    }
    #endregion
    
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
    PlayerMovement player;
    Arms playerArms;
    BasicStats playerBasicStats;

    int slotIndex = 0;
    Equipment oldItem = null;

    void Start()
    {
        inv = Inventory.instance;
        player = PlayerMovement.instance;
        playerArms = player.transform.Find("Arms").GetComponent<Arms>();
        playerBasicStats = player.GetComponent<BasicStats>();

        int numWeaponSlots = System.Enum.GetNames(typeof(WeaponSlot)).Length;
        currentWeapons = new Equipment[numWeaponSlots];

        int numEquipmentSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numEquipmentSlots];
    }

    public void EquipToSlot(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
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

        EquipToPlayer(newItem, itemData, weaponSlot, equipmentSlot); // Physically equip the weapon/equipment on the player
    }

    public void Equip(Equipment newItem, ItemData itemData)
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

    public void Unequip(int slotIndex, bool isWeapon, ItemData itemData)
    {
        if (isWeapon == false && currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            if (inv.AddToInventory(oldItem, itemData))
            {
                currentEquipment[slotIndex] = null;
            }
        }
        else if (isWeapon && currentWeapons[slotIndex] != null)
        {
            Equipment oldItem = currentWeapons[slotIndex];
            if (inv.AddToInventory(oldItem, itemData))
            {
                currentWeapons[slotIndex] = null;
            }
        }
    }

    void EquipToPlayer(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        if (weaponSlot != WeaponSlot.None) // If this is a weapon
        {
            switch (weaponSlot)
            {
                case WeaponSlot.WeaponLeft:
                    EquipWeapon(newItem, itemData, prefabWeaponBase1H_L, leftWeaponParent, playerArms, true);
                    break;
                case WeaponSlot.WeaponRight:
                    EquipWeapon(newItem, itemData, prefabWeaponBase1H_R, rightWeaponParent, playerArms, true);
                    break;
                case WeaponSlot.Ranged:
                    EquipWeapon(newItem, itemData, prefabRangedWeaponBase, leftWeaponParent, playerArms, true);
                    break;
            }
        }
        else if (equipmentSlot != EquipmentSlot.None) // If this is a piece of armor
        {
            switch (equipmentSlot)
            {
                case EquipmentSlot.Head:
                    EquipArmor(newItem, itemData, helmet, true);
                    break;
                case EquipmentSlot.Shirt:
                    EquipArmor(newItem, itemData, shirt, true);
                    break;
                case EquipmentSlot.Cuirass:
                    EquipArmor(newItem, itemData, cuirass, true);
                    break;
                case EquipmentSlot.Gauntlets:
                    EquipArmor(newItem, itemData, leftGauntlet, true);
                    EquipArmor(newItem, itemData, rightGauntlet, true);
                    break;
                case EquipmentSlot.Pants:
                    EquipArmor(newItem, itemData, leftPants, true);
                    EquipArmor(newItem, itemData, rightPants, true);
                    break;
                case EquipmentSlot.Greaves:
                    EquipArmor(newItem, itemData, leftGreaves, true);
                    EquipArmor(newItem, itemData, rightGreaves, true);
                    break;
                case EquipmentSlot.Boots:
                    EquipArmor(newItem, itemData, leftBoot, true);
                    EquipArmor(newItem, itemData, rightBoot, true);
                    break;
                case EquipmentSlot.Quiver:
                    quiverSlot.SetQuiverStackSizeText();
                    // TODO: Make visible quiver on character? // EquipArmor(newItem, itemData, quiver, true);
                    break;
                case EquipmentSlot.Belt:
                    AdjustStats(itemData, playerBasicStats);
                    break;
                case EquipmentSlot.RightRing:
                    AdjustStats(itemData, playerBasicStats);
                    break;
                case EquipmentSlot.LeftRing:
                    AdjustStats(itemData, playerBasicStats);
                    break;
                case EquipmentSlot.Amulet:
                    AdjustStats(itemData, playerBasicStats);
                    break;
            }
        }
        else
            Debug.LogError("Either a valid weaponSlot or equipmentSlot needs passed into this function to work.");
    }

    void EquipWeapon(Equipment newItem, ItemData itemData, GameObject weaponBasePrefab, Transform weaponParent, Arms arms, bool equippingToPlayer)
    {
        GameObject weaponBase = Instantiate(weaponBasePrefab, weaponParent);
        GameObject weapon = Instantiate(newItem.prefab, weaponBase.transform);
        weapon.name = itemData.itemName;
        weapon.GetComponent<SpriteRenderer>().sprite = newItem.sprite;
        
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

        arms.SetLeftAnims();
        arms.SetRightAnims();

        if (equippingToPlayer)
            AdjustStats(itemData, playerBasicStats);
    }

    void EquipArmor(Equipment newItem, ItemData itemData, SpriteRenderer armorSpriteRenderer, bool equippingToPlayer)
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

        if (equippingToPlayer)
            AdjustStats(itemData, playerBasicStats);
    }

    void AdjustStats(ItemData itemData, BasicStats stats)
    {
        stats.defense += itemData.defense;
    }
}
