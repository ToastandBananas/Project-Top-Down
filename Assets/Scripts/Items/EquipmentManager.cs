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

    Equipment[] currentWeapons;
    Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    public delegate void OnWeaponChanged(Equipment newItem, Equipment oldItem);
    public OnWeaponChanged onWeaponChanged;

    Inventory inventory;
    PlayerMovement player;

    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerMovement.instance;

        int numWeaponSlots = System.Enum.GetNames(typeof(WeaponSlot)).Length;
        currentWeapons = new Equipment[numWeaponSlots];

        int numEquipmentSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numEquipmentSlots];
    }

    public void Equip(Equipment newItem, ItemData itemData)
    {
        Equipment oldItem = null;
        int slotIndex = 0;
        if (newItem.weaponType != WeaponType.NOT_A_WEAPON)
        {
            newItem.weaponSlot = WeaponSlot.PRIMARY_WEAPON_RIGHT;
            if (currentWeapons[(int)WeaponSlot.PRIMARY_WEAPON_RIGHT] != null)
                newItem.weaponSlot = WeaponSlot.PRIMARY_WEAPON_LEFT;
            if (newItem.weaponSlot == WeaponSlot.PRIMARY_WEAPON_LEFT && currentWeapons[(int)WeaponSlot.PRIMARY_WEAPON_LEFT] != null)
                newItem.weaponSlot = WeaponSlot.SECONDARY_WEAPON_RIGHT;
            if (newItem.weaponSlot == WeaponSlot.SECONDARY_WEAPON_RIGHT && currentWeapons[(int)WeaponSlot.SECONDARY_WEAPON_RIGHT] != null)
                newItem.weaponSlot = WeaponSlot.SECONDARY_WEAPON_LEFT;
            if (newItem.weaponSlot == WeaponSlot.SECONDARY_WEAPON_LEFT && currentWeapons[(int)WeaponSlot.SECONDARY_WEAPON_LEFT] != null)
                newItem.weaponSlot = WeaponSlot.PRIMARY_WEAPON_RIGHT;

            slotIndex = (int)newItem.weaponSlot;

            if (currentWeapons[slotIndex] != null)
            {
                oldItem = currentWeapons[slotIndex];
                StartCoroutine(SwapEquipment(oldItem, itemData));
            }

            if (onWeaponChanged != null)
                onWeaponChanged.Invoke(newItem, oldItem);

            currentWeapons[slotIndex] = newItem;
        }
        else if (newItem.armorType == ArmorType.RING)
        {
            newItem.equipmentSlot = EquipmentSlot.RING1;
            if (currentEquipment[(int)EquipmentSlot.RING1] != null)
                newItem.equipmentSlot = EquipmentSlot.RING2;
            if (newItem.equipmentSlot == EquipmentSlot.RING2 && currentEquipment[(int)EquipmentSlot.RING2] != null)
                newItem.equipmentSlot = EquipmentSlot.RING1;

            slotIndex = (int)newItem.equipmentSlot;

            if (currentEquipment[slotIndex] != null)
            {
                oldItem = currentEquipment[slotIndex];
                StartCoroutine(SwapEquipment(oldItem, itemData));
            }

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(newItem, oldItem);

            currentEquipment[slotIndex] = newItem;
        }
        else
        {
            slotIndex = (int)newItem.equipmentSlot;

            if (currentEquipment[slotIndex] != null)
            {
                oldItem = currentEquipment[slotIndex];
                StartCoroutine(SwapEquipment(oldItem, itemData));
            }

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(newItem, oldItem);

            currentEquipment[slotIndex] = newItem;
        }
    }

    public void Unequip(int slotIndex, bool isWeapon, ItemData itemData)
    {
        if (isWeapon == false && currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            if (AddToInventory(oldItem, itemData))
            {
                currentEquipment[slotIndex] = null;

                if (onEquipmentChanged != null)
                    onEquipmentChanged.Invoke(null, oldItem);
            }
        }
        else if (isWeapon && currentWeapons[slotIndex] != null)
        {
            Equipment oldItem = currentWeapons[slotIndex];
            if (AddToInventory(oldItem, itemData))
            {
                currentWeapons[slotIndex] = null;

                if (onWeaponChanged != null)
                    onWeaponChanged.Invoke(null, oldItem);
            }
        }
    }

    /*public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i, false);
        }

        for (int i = 0; i < currentWeapons.Length; i++)
        {
            Unequip(i, true);
        }
    }*/

    bool AddToInventory(Item itemToAdd, ItemData itemData)
    {
        if (inventory.AddToPockets(itemToAdd, itemData))
            return true;
        else if (inventory.AddToBag(itemToAdd, itemData))
            return true;
        else if (player.isMounted && inventory.AddToHorseBag(itemToAdd, itemData))
            return true;

        return false;
    }

    IEnumerator SwapEquipment(Item itemToAdd, ItemData itemData)
    {
        yield return new WaitForSeconds(0.1f);
        AddToInventory(itemToAdd, itemData);
    }
}
