using System.Collections;
using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

/// <summary> This class will read data from its Item scriptable object, potentially randomize some of the stats and then store the new data here. </summary>
public class ItemData : MonoBehaviour
{
    public bool hasBeenRandomized = false;

    [Header("Item Class")]
    public Item item;
    public Equipment equipment;
    public Consumable consumable;

    [Header("General Data")]
    public Sprite inventoryIcon;
    public Sprite gameSprite;
    public string itemName;
    public int value;
    public int currentStackSize = 1;
    public int maxDurability;
    public float durability = 0;

    [Header("Weapon Data")]
    public int damage = 0;

    [Header("Armor Data")]
    public int defense = 0;

    [Header("Consumable Data")]
    public int freshness = 100;

    [Header("Quiver Data")]
    public GameObject ammoTypePrefab;
    public int currentAmmoCount = 0;

    void Awake()
    {
        if (item == null)
        {
            if (equipment != null)
                item = equipment;
            else if (consumable != null)
                item = consumable;
        }
    }

    void Start()
    {
        if (hasBeenRandomized)
            value = CalculateItemValue();
        else if (item != null)
            RandomizeData();
    }

    public void TransferData(ItemData dataGiver, ItemData dataReceiver)
    {
        // Randomization Data
        dataReceiver.hasBeenRandomized = dataGiver.hasBeenRandomized;

        // Item Class
        dataReceiver.item = dataGiver.item;
        dataReceiver.equipment = dataGiver.equipment;
        dataReceiver.consumable = dataGiver.consumable;

        // General Data
        dataReceiver.inventoryIcon = dataGiver.inventoryIcon;
        dataReceiver.gameSprite = dataGiver.gameSprite;
        dataReceiver.itemName = dataGiver.itemName;
        dataReceiver.value = dataGiver.value;
        dataReceiver.currentStackSize = dataGiver.currentStackSize;
        dataReceiver.maxDurability = dataGiver.maxDurability;
        dataReceiver.durability = dataGiver.durability;

        // Consumable Data
        dataReceiver.freshness = dataGiver.freshness;

        // Weapon Data
        dataReceiver.damage = dataGiver.damage;

        // Armor Data
        dataReceiver.defense = dataGiver.defense;

        // Ammo Data
        dataReceiver.currentAmmoCount = dataGiver.currentAmmoCount;
        dataReceiver.ammoTypePrefab = dataGiver.ammoTypePrefab;
    }

    public IEnumerator TransferDataWithDelay(ItemData dataGiver, ItemData dataReceiver)
    {
        yield return new WaitForSeconds(0.1f);
        TransferData(dataGiver, dataReceiver);
    }

    public void SwapData(ItemData dataSet1, ItemData dataSet2)
    {
        ItemData tempData1 = new ItemData(); // Our temporary ItemData classes
        ItemData tempData2 = new ItemData();

        TransferData(dataSet1, tempData1); // Transfer data to temp1 from dataSet1
        TransferData(dataSet2, tempData2); // Transfer data to temp2 from dataSet2

        TransferData(tempData1, dataSet2); // Transfer data to dataSet2 from tempData1
        TransferData(tempData2, dataSet1); // Transfer data to dataSet1 from tempData2
    }

    public void RandomizeData()
    {
        // Item class data
        itemName = item.name;

        // Equipment class data
        if (equipment != null)
        {
            if (equipment.maxBaseDurability > 0)
                maxDurability = Random.Range(equipment.minBaseDurability, equipment.maxBaseDurability + 1);

            durability = maxDurability;

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield)
                damage = Random.Range(equipment.minBaseDamage, equipment.maxBaseDamage + 1);

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                defense = Random.Range(equipment.minBaseDefense, equipment.maxBaseDefense + 1);

            if (equipment.isStackable)
                currentStackSize = Random.Range(1, equipment.maxStackSize + 1);
        }
        // Consumable class data
        else if (consumable != null)
        {
            freshness = Random.Range(consumable.minBaseFreshness, consumable.maxBaseFreshness + 1);
        }

        // Multiple sprite possiblities item data
        if (item.inventoryIcons.Length > 1 && item.itemType != ItemType.Ammunition)
            inventoryIcon = item.inventoryIcons[Random.Range(0, item.inventoryIcons.Length - 1)];
        else if (item.itemType == ItemType.Ammunition)
            SetAmmoSprites();
        else
            inventoryIcon = item.inventoryIcons[0];

        for (int i = 0; i < item.inventoryIcons.Length; i++)
        {
            if (inventoryIcon == item.inventoryIcons[i])
                gameSprite = item.possibleSprites[i];
        }

        value = CalculateItemValue();

        hasBeenRandomized = true;
    }

    public IEnumerator RandomizeDataWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        RandomizeData();
    }

    public void SetAmmoSprites()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        switch (currentStackSize)
        {
            case 1:
                gameSprite = item.possibleSprites[0]; // 1 arrow
                inventoryIcon = item.inventoryIcons[0];
                spriteRenderer.sprite = gameSprite;
                break;
            case 2:
                gameSprite = item.possibleSprites[1]; // 2 arrows
                inventoryIcon = item.inventoryIcons[1];
                spriteRenderer.sprite = gameSprite;
                break;
            case 3:
                gameSprite = item.possibleSprites[2]; // 3 arrows
                inventoryIcon = item.inventoryIcons[2];
                spriteRenderer.sprite = gameSprite;
                break;
            case 4:
                gameSprite = item.possibleSprites[3]; // 4 arrows
                inventoryIcon = item.inventoryIcons[3];
                spriteRenderer.sprite = gameSprite;
                break;
            default:
                gameSprite = item.possibleSprites[4]; // 5 arrows
                inventoryIcon = item.inventoryIcons[4];
                spriteRenderer.sprite = gameSprite;
                break;
        }
    }

    public void ClearData()
    {
        hasBeenRandomized = false;
        
        item = null;
        equipment = null;
        consumable = null;

        inventoryIcon = null;
        gameSprite = null;
        itemName = "";
        value = 0;
        currentStackSize = 1;
        maxDurability = 0;
        durability = 0;
        
        damage = 0;
        defense = 0;
        freshness = 0;

        currentAmmoCount = 0;
        ammoTypePrefab = null;
}

    int CalculateItemValue()
    {
        int itemValue = 0;

        if ((equipment != null || consumable != null) && item.minBaseValue != 0 && item.maxBaseValue != 0)
            itemValue = Mathf.RoundToInt(item.minBaseValue + ((item.maxBaseValue - item.minBaseValue) * CalculatePercentPointValue()));
        else
            itemValue = item.staticValue;
        
        return itemValue;
    }

    float GetTotalPointValue()
    {
        // Add up all the possible points that can be added to our stats when randomized (damage, defense, etc)
        float totalPointsPossible = 0;

        if (equipment != null)
        {
            if (equipment.maxBaseDurability > 0)
                totalPointsPossible += (equipment.maxBaseDurability - equipment.minBaseDurability);

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield)
                totalPointsPossible += (equipment.maxBaseDamage - equipment.minBaseDamage) * 2;

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                totalPointsPossible += (equipment.maxBaseDefense - equipment.minBaseDefense) * 2;
        }
        else if (consumable != null)
        {
            if (consumable.consumableType == ConsumableType.Food)
                totalPointsPossible += (consumable.maxBaseFreshness - consumable.minBaseFreshness);
        }
        
        return totalPointsPossible;
    }

    float CalculatePercentPointValue()
    {
        // Calculate the percentage of points that were added to the item's stats when randomized (compared to the total possible points)
        float pointIncrease = 0; // Amount the stats have been increased by in relation to its base stat values, in total
        float percent = 0; // Percent of possible stat increase this item has

        ClampMaxValues();

        if (equipment != null)
        {
            if (equipment.maxBaseDurability > 0)
                pointIncrease += (maxDurability - equipment.minBaseDurability);

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield || equipment.itemType == ItemType.Ammunition)
                pointIncrease += (damage - equipment.minBaseDamage) * 2; // Damage contributes to value twice as much

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                pointIncrease += (defense - equipment.minBaseDefense) * 2; // Armor contributes to value twice as much
        }
        else if (consumable != null)
        {
            if (consumable.consumableType == ConsumableType.Food)
                pointIncrease += (freshness - consumable.minBaseFreshness);
        }

        percent = pointIncrease / GetTotalPointValue();

        return percent;
    }

    void ClampMaxValues()
    {
        // This function just makes sure that our stats aren't too high or too low
        if (equipment != null)
        {
            if (maxDurability > equipment.maxBaseDurability)
            {
                maxDurability = equipment.maxBaseDurability;
                durability = maxDurability;
            }
            else if (maxDurability < equipment.minBaseDurability)
            {
                maxDurability = equipment.minBaseDurability;
                durability = maxDurability;
            }

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield || equipment.itemType == ItemType.Ammunition)
            {
                if (damage > equipment.maxBaseDamage)
                    damage = equipment.maxBaseDamage;
                else if (damage < equipment.minBaseDamage)
                    damage = equipment.minBaseDamage;
            }

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
            {
                if (defense > equipment.maxBaseDefense)
                    defense = equipment.maxBaseDefense;
                else if (defense < equipment.minBaseDefense)
                    defense = equipment.minBaseDefense;
            }
        }
        else if (consumable != null)
        {
            if (consumable.consumableType == ConsumableType.Food)
            {
                if (freshness > consumable.maxBaseFreshness)
                    freshness = consumable.maxBaseFreshness;
                else if (freshness < consumable.minBaseFreshness)
                    freshness = consumable.minBaseFreshness;
            }
        }
    }
}
