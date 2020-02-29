using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

/// <summary> This class will read data from the Item scriptable object and potentially randomize some of the stats and then store the new data here. </summary>
public class ItemData : MonoBehaviour
{
    [Header("Item Class")]
    public Item item;
    public Equipment equipment;

    [Header("General Data")]
    public string itemName;
    public Rarity rarity;
    public int value;
    public int currentStackSize = 1;
    public float maxDurability;
    public float durability;

    [Header("Weapon Data")]
    public float damage;

    [Header("Armor Data")]
    public float defense;

    public void TransferData(ItemData dataGiver, ItemData dataReceiver)
    {
        //Debug.Log(dataGiver.item);
        //Debug.Log(dataGiver.equipment);

        // Item Class
        if (dataGiver.item != null)
            dataReceiver.item = dataGiver.item;
        else
            dataReceiver.equipment = dataGiver.equipment;

        // General Data
        dataReceiver.itemName = dataGiver.itemName;
        dataReceiver.rarity = dataGiver.rarity;
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
}
