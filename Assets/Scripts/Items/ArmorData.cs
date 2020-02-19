using UnityEngine;

public class ArmorData : ItemData
{
    [Header("Armor Data")]
    public Equipment equipmentItem;
    public float defense = 1f;
    public float durability = 100f;

    void Awake()
    {
        //item = GetComponent<Armor>().equipment;
    }
}
