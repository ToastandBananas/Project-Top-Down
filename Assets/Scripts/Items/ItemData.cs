using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

/// <summary> This class will read data from its Item scriptable object, potentially randomize some of the stats and then store the new data here. </summary>
public class ItemData : MonoBehaviour
{
    public bool hasBeenRandomized = false;

    [Header("Item Class")]
    public Item item;
    public Equipment equipment;

    [Header("General Data")]
    public string itemName;
    public int value;
    public int currentStackSize = 1;
    public int maxDurability;
    public float durability = 0;

    [Header("Weapon Data")]
    public int damage;

    [Header("Armor Data")]
    public int defense;

    void Awake()
    {
        if (item == null)
        {
            if (equipment != null)
                item = equipment;
            //if (consumable != null)
                //item = consumable;
        }
    }

    void Start()
    {
        if (hasBeenRandomized)
            value = CalculateItemValue();
    }

    public void TransferData(ItemData dataGiver, ItemData dataReceiver)
    {
        // Item Class
        dataReceiver.item = dataGiver.item;
        dataReceiver.equipment = dataGiver.equipment;
        //dataReceiver.consumable = dataGiver.consumable;

        // General Data
        dataReceiver.itemName = dataGiver.itemName;
        dataReceiver.value = dataGiver.value;
        dataReceiver.currentStackSize = dataGiver.currentStackSize;
        dataReceiver.maxDurability = dataGiver.maxDurability;
        dataReceiver.durability = dataGiver.durability;

        // Consumable Data
        // dataReceiver.freshness = dataGiver.freshness;

        // Weapon Data
        dataReceiver.damage = dataGiver.damage;

        // Armor Data
        dataReceiver.defense = dataGiver.defense;
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
                maxDurability = Random.Range(equipment.minBaseDurability, equipment.maxBaseDurability);

            durability = maxDurability;

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield || equipment.itemType == ItemType.Ammunition)
                damage = Random.Range(equipment.minBaseDamage, equipment.maxBaseDamage);

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                defense = Random.Range(equipment.minBaseDefense, equipment.maxBaseDefense);
        }

        // Consumable class data
        /*else if (consumable != null)
        {

        }*/

        value = CalculateItemValue();

        hasBeenRandomized = true;
    }

    int CalculateItemValue()
    {
        int itemValue = 0;

        if (equipment != null)//|| consumable != null)
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

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield || equipment.itemType == ItemType.Ammunition)
                totalPointsPossible += (equipment.maxBaseDamage - equipment.minBaseDamage) * 2;

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                totalPointsPossible += (equipment.maxBaseDefense - equipment.minBaseDefense) * 2;
        }
        /*else if (consumable != null)
        {

        }*/
        
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
                pointIncrease += (damage - equipment.minBaseDamage) * 2;

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                pointIncrease += (defense - equipment.minBaseDefense) * 2;
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
        /*else if (consumable != null)
        {

        }*/
    }
}
