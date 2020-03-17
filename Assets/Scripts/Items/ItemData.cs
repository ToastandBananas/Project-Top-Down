using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

/// <summary> This class will read data from the Item scriptable object and potentially randomize some of the stats and then store the new data here. </summary>
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
    public float durability;

    [Header("Weapon Data")]
    public int damage;

    [Header("Armor Data")]
    public int defense;

    // Used in calculating the item's value
    float percentPointValue;

    void Awake()
    {
        if (item == null)
        {
            if (equipment != null)
                item = equipment;
            //if (consumable != null)
                //item = consumable;
        }
        
        if (item != null)
            RandomizeData();
    }

    public void RandomizeData()
    {
        // Item class data
        itemName = item.name;
        
        // Equipment class data
        if (equipment != null)
        {
            maxDurability = Mathf.RoundToInt(Random.Range(equipment.minBaseDurability, equipment.maxBaseDurability));
            durability = maxDurability;

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield)
                damage =  Mathf.RoundToInt(Random.Range(equipment.minBaseDamage, equipment.maxBaseDamage));

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                defense = Mathf.RoundToInt(Random.Range(equipment.minBaseDefense, equipment.maxBaseDefense));
        }

        // Consumable class data
        /*if (consumable != null)
        {

        }*/

        value = CalculateItemValue();

        hasBeenRandomized = true;
    }

    public void TransferData(ItemData dataGiver, ItemData dataReceiver)
    {
        // Item Class
        dataReceiver.item = dataGiver.item;
        dataReceiver.equipment = dataGiver.equipment;
        //dataReceiver.consumable = dataGiver.consumable;

        // General Data
        dataReceiver.itemName = dataGiver.itemName;
        dataReceiver.currentStackSize = dataGiver.currentStackSize;
        dataReceiver.maxDurability = dataGiver.maxDurability;
        dataReceiver.durability = dataGiver.durability;

        // Consumable Data


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

    int CalculateItemValue()
    {
        int itemValue = 0;

        if (equipment != null)//|| consumable != null)
            itemValue = Mathf.RoundToInt((item.maxBaseValue = item.minBaseValue) * GetPercentPointValue());
        else
            itemValue = item.staticValue;

        return itemValue;
    }

    float GetTotalPointValue()
    {
        // Add up all the possible points that can be added to our stats when randomized (damage, defense, etc)
        float totalPointValue = 0;

        if (equipment != null)
        {
            totalPointValue += (equipment.maxBaseDurability - equipment.minBaseDurability);

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield)
                totalPointValue += (equipment.maxBaseDamage - equipment.minBaseDamage);

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                totalPointValue += (equipment.maxBaseDefense - equipment.minBaseDefense);
        }
        /*else if (consumable != null)
        {

        }*/

        return totalPointValue;
    }

    float GetPercentPointValue()
    {
        float pointIncrease = 0; // Amount the stats have been increased by in relation to its base stat values, in total
        float percent = 0; // Percent of possible stat increase this item has
        percentPointValue = 0;

        if (equipment != null)
        {
            pointIncrease += (durability - equipment.minBaseDurability);

            if (equipment.itemType == ItemType.Weapon || equipment.itemType == ItemType.Shield)
                pointIncrease += (damage - equipment.minBaseDamage);

            if (equipment.itemType == ItemType.Armor || equipment.itemType == ItemType.Shield)
                pointIncrease += (defense - equipment.minBaseDefense);
        }

        percent = pointIncrease / GetTotalPointValue();
        Debug.Log(percent);

        return percent;
    }
}
