using UnityEngine;

public class WeaponData : ItemData
{
    [Header("Weapon Data")]
    public Equipment equipmentItem;
    public float damage = 1f;
    public float durability = 100f;

    void Awake()
    {
        if (item == null)
            item = GetComponent<WeaponDamage>().equipment;
    }
}
