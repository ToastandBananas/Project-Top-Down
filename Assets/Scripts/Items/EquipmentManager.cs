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

    [Header("Currently Equipped Lists")]
    public ItemData[] currentWeapons;
    public ItemData[] currentEquipment;

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

    [Header("Other Equipment")]
    public GameObject amulet;
    public GameObject leftRing, rightRing;
    public GameObject belt;
    public GameObject quiver;

    [Header("Quiver Slot")]
    public EquipSlot quiverSlot;

    AudioManager audioManager;
    GameManager GM;
    Inventory inv;
    InventoryUI invUI;
    Arms arms;
    BasicStats basicStats;
    GameObject weaponsParent;
    GameObject EquipmentParent;

    int slotIndex = 0;

    void Start()
    {
        audioManager = AudioManager.instance;
        GM = GameManager.instance;
        inv = Inventory.instance;
        invUI = InventoryUI.instance;
        arms = transform.Find("Arms").GetComponent<Arms>();
        basicStats = GetComponent<BasicStats>();
        weaponsParent = invUI.playerEquipmentMenu.transform.GetChild(0).Find("Weapons").gameObject;
        EquipmentParent = invUI.playerEquipmentMenu.transform.GetChild(0).Find("Equipment").gameObject;

        int numWeaponSlots = System.Enum.GetNames(typeof(WeaponSlot)).Length;
        currentWeapons = new ItemData[numWeaponSlots];

        int numEquipmentSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new ItemData[numEquipmentSlots];

        StartCoroutine(SetCurrentlyEquipped());
    }

    void Update()
    {
        if (basicStats != null && basicStats.isPlayer)
        {
            if (GameControls.gamePlayActions.playerSwapWeapon.WasPressed)
            {
                // Swap weapons if the player has both melee and ranged weapons equipped
                if (currentWeapons[(int)WeaponSlot.Ranged] != null && (currentWeapons[(int)WeaponSlot.WeaponLeft] != null || currentWeapons[(int)WeaponSlot.WeaponRight] != null))
                {
                    if (arms.leftWeapon != null && arms.leftWeapon.generalClassification == GeneralClassification.RangedWeapon)
                    {
                        Unequip(currentWeapons[(int)WeaponSlot.Ranged].equipment, currentWeapons[(int)WeaponSlot.Ranged], WeaponSlot.Ranged, EquipmentSlot.None, false);

                        if (currentWeapons[(int)WeaponSlot.WeaponLeft] != null)
                            EquipToCharacter(currentWeapons[(int)WeaponSlot.WeaponLeft].equipment, currentWeapons[(int)WeaponSlot.WeaponLeft], WeaponSlot.WeaponLeft, EquipmentSlot.None);

                        if (currentWeapons[(int)WeaponSlot.WeaponRight] != null)
                            EquipToCharacter(currentWeapons[(int)WeaponSlot.WeaponRight].equipment, currentWeapons[(int)WeaponSlot.WeaponRight], WeaponSlot.WeaponRight, EquipmentSlot.None);
                    }
                    else
                    {
                        if (currentWeapons[(int)WeaponSlot.WeaponLeft] != null)
                            Unequip(currentWeapons[(int)WeaponSlot.WeaponLeft].equipment, currentWeapons[(int)WeaponSlot.WeaponLeft], WeaponSlot.WeaponLeft, EquipmentSlot.None, false);

                        if (currentWeapons[(int)WeaponSlot.WeaponRight] != null)
                            Unequip(currentWeapons[(int)WeaponSlot.WeaponRight].equipment, currentWeapons[(int)WeaponSlot.WeaponRight], WeaponSlot.WeaponRight, EquipmentSlot.None, false);

                        EquipToCharacter(currentWeapons[(int)WeaponSlot.Ranged].equipment, currentWeapons[(int)WeaponSlot.Ranged], WeaponSlot.Ranged, EquipmentSlot.None);
                    }
                }
            }
        }
    }

    public void EquipItem(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        audioManager.PlayRandomSound(audioManager.inventorySounds);

        if (weaponSlot != WeaponSlot.None) // If the item is a weapon
        {
            slotIndex = (int)weaponSlot; // Grab the index number of this weaponSlot

            currentWeapons[slotIndex] = itemData; // Assign the weapon to our currentWeapons array
        }
        else if (equipmentSlot != EquipmentSlot.None) // If the item is a piece of equipment
        {
            slotIndex = (int)equipmentSlot; // Grab the index number of this equipmentSlot

            currentEquipment[slotIndex] = itemData; // Assign the equipment to our currentEquipment array
        }

        if ((weaponSlot == WeaponSlot.Ranged && !arms.leftShieldEquipped && !arms.rightShieldEquipped && !arms.leftWeaponEquipped && !arms.rightWeaponEquipped && !arms.twoHanderEquipped)
            || (weaponSlot != WeaponSlot.None && weaponSlot != WeaponSlot.Ranged && !arms.rangedWeaponEquipped))
        {
            EquipToCharacter(newItem, itemData, weaponSlot, equipmentSlot); // Physically equip the weapon/equipment on the character
        }
    }

    public void AutoAddAmmoToQuiver(Equipment newItem, ItemData itemData, InventorySlot invSlotTakingFrom)
    {
        if (currentEquipment[(int)EquipmentSlot.Quiver] != null)
        {
            int ammoCount = itemData.currentStackSize;
            for (int i = 0; i < ammoCount; i++)
            {
                if (currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount < currentEquipment[(int)EquipmentSlot.Quiver].equipment.maxAmmo)
                {
                    itemData.currentStackSize--;
                    currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount++;
                    quiverSlot.quiverText.text = currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount.ToString();

                    if (itemData.currentStackSize > 1)
                        invSlotTakingFrom.stackSizeText.text = itemData.currentStackSize.ToString();
                    else
                        invSlotTakingFrom.stackSizeText.text = "";
                }
                else
                    break;
            }

            audioManager.PlayRandomSound(audioManager.inventorySounds);
            quiverSlot.SetQuiverAmmoSprites();

            currentEquipment[(int)EquipmentSlot.Quiver].ammoTypePrefab = itemData.equipment.prefab;

            if (itemData.currentStackSize <= 0)
            {
                newItem.RemoveFromInventory(itemData);
                invSlotTakingFrom.ClearSlot();
            }
        }
    }

    public void AutoEquip(Equipment newItem, ItemData itemData, InventorySlot invSlotTakingFrom)
    {

        int slotIndex = 0;
        if (newItem.weaponType != WeaponType.NotAWeapon) // If this is a weapon
        {
            if (newItem.weaponSlot != WeaponSlot.None)
            {
                // If this is a ranged weapon
                if (newItem.weaponType == WeaponType.Bow || newItem.weaponType == WeaponType.Crossbow)
                    newItem.weaponSlot = WeaponSlot.Ranged;
                else // If this is a melee weapon or shield
                {
                    // See if one of the weapon slots is empty and assign it, or if they're both full, equip to the right arm
                    newItem.weaponSlot = WeaponSlot.WeaponLeft;
                    if (currentWeapons[(int)WeaponSlot.WeaponLeft] != null)
                        newItem.weaponSlot = WeaponSlot.WeaponRight;
                }
            }

            slotIndex = (int)newItem.weaponSlot;
        }
        else if (newItem.armorType == ArmorType.Ring)
        {
            newItem.equipmentSlot = EquipmentSlot.LeftRing;
            if (currentEquipment[(int)EquipmentSlot.LeftRing] != null)
                newItem.equipmentSlot = EquipmentSlot.RightRing;
            if (newItem.equipmentSlot == EquipmentSlot.RightRing && currentEquipment[(int)EquipmentSlot.RightRing] != null)
                newItem.equipmentSlot = EquipmentSlot.LeftRing;

            slotIndex = (int)newItem.equipmentSlot;
        }
        else
            slotIndex = (int)newItem.equipmentSlot;

        EquipSlot equipSlotAddingTo = null;
        foreach (EquipSlot equipSlot in FindObjectsOfType<EquipSlot>())
        {
            if (equipSlot.thisWeaponSlot == newItem.weaponSlot && equipSlot.thisEquipmentSlot == newItem.equipmentSlot)
            {
                equipSlotAddingTo = equipSlot;

                if (invSlotTakingFrom.slotParent == invUI.containerParent)
                {
                    for (int i = 0; i < invUI.currentlyActiveContainer.containerObjects.Count; i++)
                    {
                        if (invUI.currentlyActiveContainer.containerObjects[i].GetComponent<ItemData>() == invSlotTakingFrom.itemData)
                            Destroy(invUI.currentlyActiveContainer.containerObjects[i]);
                    }
                }

                invSlotTakingFrom.ClearSlot(); // Clear out the new item from the inventory
                
                if (equipSlot.isEmpty == false)
                {
                    // If there's no room in our inventory, send it to the temp slot so we can manually place or drop the item
                    if (inv.AddToInventory(equipSlot.equipment, equipSlot.itemData) == false)
                    {
                        invUI.tempSlot.AddItem(equipSlot.equipment, equipSlot.itemData);
                        itemData.TransferData(equipSlot.itemData, invUI.tempSlot.itemData);

                        invUI.currentlySelectedItem = invUI.tempSlot.item;
                        invUI.currentlySelectedItemData = invUI.tempSlot.itemData;
                        invUI.invSlotMovingFrom = invUI.tempSlot;
                    }

                    // Remove old item
                    Unequip(equipSlot.equipment, equipSlot.itemData, equipSlot.thisWeaponSlot, equipSlot.thisEquipmentSlot, false);
                    equipSlot.ClearSlot(equipSlot);
                }

                // Add new item
                EquipItem(newItem, itemData, newItem.weaponSlot, newItem.equipmentSlot);
                equipSlot.AddItem(newItem, itemData);
                equipSlot.SetQuiverStackSizeText();

                // Display floating text
                GM.floatingTexts[GM.floatingTextIndex].DisplayUseItemFloatingText(equipSlot.equipment, PlayerMovement.instance.transform, true);
                break;
            }
        }

        if (newItem.weaponType != WeaponType.NotAWeapon) // If this is a weapon
            currentWeapons[slotIndex] = equipSlotAddingTo.itemData;
        else // If this is equipment
            currentEquipment[slotIndex] = equipSlotAddingTo.itemData;
    }

    public void EquipToCharacter(Equipment newItem, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        if (weaponSlot != WeaponSlot.None) // If this is a weapon
        {
            switch (weaponSlot) // Determine which slot to change
            {
                case WeaponSlot.WeaponLeft:
                    if (newItem.itemType == ItemType.Shield)
                        EquipWeapon(newItem, itemData, prefabShieldBase_L, leftWeaponParent);
                    else
                        EquipWeapon(newItem, itemData, prefabWeaponBase1H_L, leftWeaponParent);
                    break;
                case WeaponSlot.WeaponRight:
                    if (newItem.itemType == ItemType.Shield)
                        EquipWeapon(newItem, itemData, prefabShieldBase_R, rightWeaponParent);
                    else
                        EquipWeapon(newItem, itemData, prefabWeaponBase1H_R, rightWeaponParent);
                    break;
                case WeaponSlot.Ranged:
                    EquipWeapon(newItem, itemData, prefabWeaponBase1H_L, leftWeaponParent);
                    break;
            }

            AdjustStats(itemData, basicStats);
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
                    if (itemData.currentAmmoCount > 0)
                        quiverSlot.quiverText.text = itemData.currentAmmoCount.ToString();
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
            
            GM.floatingTexts[GM.floatingTextIndex].DisplayUseItemFloatingText(newItem, PlayerMovement.instance.transform, true);
        }
        else
            Debug.LogError("Either a valid weaponSlot or equipmentSlot needs passed into this function to work.");
    }

    void EquipWeapon(Equipment newItem, ItemData itemData, GameObject weaponBasePrefab, Transform weaponParent)
    {
        GameObject weaponBase = Instantiate(weaponBasePrefab, weaponParent);
        GameObject weapon = Instantiate(newItem.prefab, weaponBase.transform);

        itemData.TransferData(itemData, weapon.GetComponent<ItemData>());

        weapon.name = itemData.itemName;
        weapon.GetComponent<SpriteRenderer>().sprite = itemData.gameSprite;
        if (weapon.TryGetComponent(out BoxCollider2D boxCollider))
            boxCollider.enabled = true;
        if (weapon.TryGetComponent(out WeaponDamage weaponDamage))
        weaponDamage.enabled = true;
        
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

        StartCoroutine(arms.SetArmAnims());
    }

    void EquipArmor(Equipment newItem, ItemData itemData, SpriteRenderer armorSpriteRenderer)
    {
        itemData.TransferData(itemData, armorSpriteRenderer.GetComponent<ItemData>());

        armorSpriteRenderer.sprite = itemData.gameSprite;

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

    public bool Unequip(Equipment item, ItemData itemData, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot, bool addToInventory)
    {
        bool roomInInventory = true;
        if (addToInventory)
            roomInInventory = inv.AddToInventory(item, itemData);

        if (roomInInventory)
        {
            if (weaponSlot != WeaponSlot.None) // If the item is a weapon
            {
                slotIndex = (int)weaponSlot; // Grab the index number of this weaponSlot
                
                if (addToInventory)
                    currentWeapons[slotIndex] = null; // Unassign the weapon from our currentWeapons array

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

                StartCoroutine(arms.SetArmAnims());
            }
            else if (equipmentSlot != EquipmentSlot.None) // If the item is a piece of equipment
            {
                slotIndex = (int)equipmentSlot; // Grab the index number of this equipmentSlot
                
                currentEquipment[slotIndex] = null; // Assign the equipment to our currentEquipment array

                switch (equipmentSlot) // Determine which slot to change
                {
                    case EquipmentSlot.Head:
                        ClearSpriteAndData(helmet, helmet.GetComponent<ItemData>());
                        break;
                    case EquipmentSlot.Shirt:
                        ClearSpriteAndData(shirt, shirt.GetComponent<ItemData>());
                        break;
                    case EquipmentSlot.Cuirass:
                        ClearSpriteAndData(cuirass, cuirass.GetComponent<ItemData>());
                        ClearSpriteAndData(leftCuirassArm, null);
                        ClearSpriteAndData(rightCuirassArm, null);
                        break;
                    case EquipmentSlot.Gauntlets:
                        ClearSpriteAndData(leftGauntlet, leftGauntlet.GetComponent<ItemData>());
                        ClearSpriteAndData(rightGauntlet, null);
                        break;
                    case EquipmentSlot.Pants:
                        ClearSpriteAndData(leftPants, leftPants.GetComponent<ItemData>());
                        ClearSpriteAndData(rightPants, null);
                        break;
                    case EquipmentSlot.Greaves:
                        ClearSpriteAndData(leftGreaves, leftGreaves.GetComponent<ItemData>());
                        ClearSpriteAndData(rightGreaves, null);
                        break;
                    case EquipmentSlot.Boots:
                        ClearSpriteAndData(leftBoot, leftBoot.GetComponent<ItemData>());
                        ClearSpriteAndData(rightBoot, null);
                        break;
                    case EquipmentSlot.Quiver:
                        // TODO: Make visible quiver on character?
                        ClearSpriteAndData(null, quiver.GetComponent<ItemData>());
                        quiverSlot.GetComponentInChildren<Text>().text = "";
                        break;
                    case EquipmentSlot.Belt:
                        ClearSpriteAndData(null, belt.GetComponent<ItemData>());
                        break;
                    case EquipmentSlot.LeftRing:
                        ClearSpriteAndData(null, leftRing.GetComponent<ItemData>());
                        break;
                    case EquipmentSlot.RightRing:
                        ClearSpriteAndData(null, rightRing.GetComponent<ItemData>());
                        break;
                    case EquipmentSlot.Amulet:
                        ClearSpriteAndData(null, amulet.GetComponent<ItemData>());
                        break;
                }
            }
            else
                Debug.LogWarning("No equipSlot or invSlot set...Fix this!");

            RemoveStats(itemData, basicStats);
            return true;
        }
        else
        {
            Debug.Log("No room in inventory");
            return false;
        }
    }

    void AdjustStats(ItemData itemData, BasicStats stats)
    {
        if (itemData.equipment.itemType != ItemType.Weapon)
            stats.defense += itemData.defense;
        stats.encumbrance += itemData.equipment.weight;
    }

    void RemoveStats(ItemData itemData, BasicStats stats)
    {
        if (itemData.equipment.itemType != ItemType.Weapon)
            stats.defense -= itemData.defense;
        stats.encumbrance -= itemData.equipment.weight;
    }

    void ClearSpriteAndData(SpriteRenderer spriteRenderer, ItemData itemData)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = null;
        if (itemData != null)
            itemData.ClearData();
    }

    // This function runs only if we manually equip item's on the character in the hierarchy
    IEnumerator SetCurrentlyEquipped()
    {
        yield return new WaitForSeconds(0.1f);

        // Get weapon sprites if any are in the characters arms
        if (leftWeaponParent != null && leftWeaponParent.childCount > 0)
        {
            if (leftWeaponParent.GetChild(0).GetComponentInChildren<ItemData>().equipment.generalClassification == GeneralClassification.RangedWeapon)
                rangedWeapon = leftWeaponParent.GetChild(0).GetComponentInChildren<SpriteRenderer>();
            else
                leftWeapon = leftWeaponParent.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        }

        if (rightWeaponParent != null && rightWeaponParent.childCount > 0)
            rightWeapon = rightWeaponParent.GetChild(0).GetComponentInChildren<SpriteRenderer>();

        // Add each weapon's ItemData to our currentWeapons and add the weapons to the appropriate slots in the Equipment Menu
        SetEquippedItem(leftWeapon, null, WeaponSlot.WeaponLeft, EquipmentSlot.None);
        SetEquippedItem(rightWeapon, null, WeaponSlot.WeaponRight, EquipmentSlot.None);
        SetEquippedItem(rangedWeapon, null, WeaponSlot.Ranged, EquipmentSlot.None);

        // Add each equipment's ItemData to our currentEquipment and add the equipment to the appropriate slots in the Equipment Menu
        SetEquippedItem(helmet, null, WeaponSlot.None, EquipmentSlot.Head);
        SetEquippedItem(shirt, null, WeaponSlot.None, EquipmentSlot.Shirt);
        SetEquippedItem(cuirass, null, WeaponSlot.None, EquipmentSlot.Cuirass);
        SetEquippedItem(leftGauntlet, null, WeaponSlot.None, EquipmentSlot.Gauntlets);
        SetEquippedItem(leftPants, null, WeaponSlot.None, EquipmentSlot.Pants);
        SetEquippedItem(leftGreaves, null, WeaponSlot.None, EquipmentSlot.Greaves);
        SetEquippedItem(leftBoot, null, WeaponSlot.None, EquipmentSlot.Boots);
        SetEquippedItem(null, amulet, WeaponSlot.None, EquipmentSlot.Amulet);
        SetEquippedItem(null, leftRing, WeaponSlot.None, EquipmentSlot.LeftRing);
        SetEquippedItem(null, rightRing, WeaponSlot.None, EquipmentSlot.RightRing);
        SetEquippedItem(null, belt, WeaponSlot.None, EquipmentSlot.Belt);
        SetEquippedItem(null, quiver, WeaponSlot.None, EquipmentSlot.Quiver);
    }
    
    void SetEquippedItem(SpriteRenderer spriteRenderer, GameObject itemGameObject, WeaponSlot weaponSlot, EquipmentSlot equipmentSlot)
    {
        if ((spriteRenderer != null && spriteRenderer.sprite != null) || (itemGameObject != null && itemGameObject.GetComponent<ItemData>().equipment != null))
        {
            ItemData itemData;
            if (spriteRenderer != null)
                itemData = spriteRenderer.GetComponent<ItemData>();
            else
                itemData = itemGameObject.GetComponent<ItemData>();

            if (basicStats != null && basicStats.isPlayer)
            {
                if (weaponSlot != WeaponSlot.None)
                {
                    foreach (EquipSlot equipSlot in invUI.weaponSlots)
                    {
                        if (equipSlot.thisWeaponSlot == weaponSlot)
                        {
                            equipSlot.AddItem(itemData.equipment, itemData);
                            StartCoroutine(TransferDataWithDelay(itemData, equipSlot));
                            currentWeapons[(int)weaponSlot] = equipSlot.itemData;
                            break;
                        }
                    }
                }
                else if (equipmentSlot != EquipmentSlot.None)
                {
                    foreach (EquipSlot equipSlot in invUI.equipSlots)
                    {
                        if (equipSlot.thisEquipmentSlot == equipmentSlot)
                        {
                            equipSlot.AddItem(itemData.equipment, itemData);
                            StartCoroutine(TransferDataWithDelay(itemData, equipSlot));
                            currentEquipment[(int)equipmentSlot] = equipSlot.itemData;
                            break;
                        }
                    }
                }
            }

            if (basicStats != null)
                AdjustStats(itemData, basicStats);
        }
    }

    public IEnumerator TransferEquippedItemsToBody(EquipmentManager deadBodyEquipmentManager)
    {
        yield return new WaitForSeconds(0.05f);

        TransferItemToBody(helmet, deadBodyEquipmentManager.helmet, null, null, deadBodyEquipmentManager);
        TransferItemToBody(shirt, deadBodyEquipmentManager.shirt, null, null, deadBodyEquipmentManager);
        TransferItemToBody(cuirass, deadBodyEquipmentManager.cuirass, null, null, deadBodyEquipmentManager);
        TransferItemToBody(leftCuirassArm, deadBodyEquipmentManager.leftCuirassArm, null, null, deadBodyEquipmentManager);
        TransferItemToBody(rightCuirassArm, deadBodyEquipmentManager.rightCuirassArm, null, null, deadBodyEquipmentManager);
        TransferItemToBody(leftGauntlet, deadBodyEquipmentManager.leftGauntlet, null, null, deadBodyEquipmentManager);
        TransferItemToBody(rightGauntlet, deadBodyEquipmentManager.rightGauntlet, null, null, deadBodyEquipmentManager);
        TransferItemToBody(leftPants, deadBodyEquipmentManager.leftPants, null, null, deadBodyEquipmentManager);
        TransferItemToBody(rightPants, deadBodyEquipmentManager.rightPants, null, null, deadBodyEquipmentManager);
        TransferItemToBody(leftGreaves, deadBodyEquipmentManager.leftGreaves, null, null, deadBodyEquipmentManager);
        TransferItemToBody(rightGreaves, deadBodyEquipmentManager.rightGreaves, null, null, deadBodyEquipmentManager);
        TransferItemToBody(leftBoot, deadBodyEquipmentManager.leftBoot, null, null, deadBodyEquipmentManager);
        TransferItemToBody(rightBoot, deadBodyEquipmentManager.rightBoot, null, null, deadBodyEquipmentManager);
        TransferItemToBody(null, null, amulet, deadBodyEquipmentManager.amulet, deadBodyEquipmentManager);
        TransferItemToBody(null, null, leftRing, deadBodyEquipmentManager.leftRing, deadBodyEquipmentManager);
        TransferItemToBody(null, null, rightRing, deadBodyEquipmentManager.rightRing, deadBodyEquipmentManager);
        TransferItemToBody(null, null, belt, deadBodyEquipmentManager.belt, deadBodyEquipmentManager);
        TransferItemToBody(null, null, quiver, deadBodyEquipmentManager.quiver, deadBodyEquipmentManager);

        Container deadBodyContainer = deadBodyEquipmentManager.GetComponent<Container>();
        foreach (ItemData equippedObjItemData in deadBodyEquipmentManager.currentEquipment)
        {
            if (equippedObjItemData != null)
                deadBodyContainer.containerObjects.Add(equippedObjItemData.gameObject);
        }
    }

    void TransferItemToBody(SpriteRenderer giver, SpriteRenderer receiver, GameObject giverGameObject, GameObject receiverGameObject, EquipmentManager deadBodyEquipmentManager)
    {
        if (giver != null && giver.sprite != null)
        {
            ItemData giverItemData = giver.GetComponent<ItemData>();
            if (giverItemData != null)
            {
                ItemData receiverItemData = receiver.GetComponent<ItemData>();
                receiver.sprite = giverItemData.equipment.deathSprite;
                giverItemData.TransferData(giverItemData, receiverItemData);

                deadBodyEquipmentManager.currentEquipment[(int)giverItemData.equipment.equipmentSlot] = receiverItemData;
            }
        }
        else if (giverGameObject != null)
        {
            ItemData giverItemData = giverGameObject.GetComponent<ItemData>();
            if (giverItemData != null && giverItemData.equipment != null)
            {
                ItemData receiverItemData = receiverGameObject.GetComponent<ItemData>();
                giverItemData.TransferData(giverItemData, receiverItemData);

                deadBodyEquipmentManager.currentEquipment[(int)giverItemData.equipment.equipmentSlot] = receiverItemData;
            }
        }
    }

    IEnumerator TransferDataWithDelay(ItemData itemData, EquipSlot equipSlot)
    {
        yield return new WaitForSeconds(0.1f);
        itemData.TransferData(itemData, equipSlot.itemData);
    }
}
