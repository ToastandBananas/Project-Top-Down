using UnityEngine;

public enum Rarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, UNIQUE }

/// <summary> This class will read data from the Item scriptable object and potentially randomize some of the stats and then store the new data here. </summary>
public class ItemData : MonoBehaviour
{
    [Header("General Data")]
    public Item item;
    public Rarity rarity;
    public int currentStackSize = 1;
    
    void Start()
    {
        
    }
}
