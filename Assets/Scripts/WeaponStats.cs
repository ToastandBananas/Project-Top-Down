using UnityEngine;

public enum GENERAL_CLASSIFICATION { WEAPON_1H, WEAPON_2H, SHIELD }
public enum WEAPON_TYPE
{
    SHIELD, ONE_HANDED_SWORD, TWO_HANDED_SWORD, ONE_HANDED_MACE, TWO_HANDED_MACE, ONE_HANDED_AXE, TWO_HANDED_AXE,
    ONE_HANDED_SPEAR, TWO_HANDED_SPEAR, BOW, CROSSBOW
}

public class WeaponStats : MonoBehaviour
{
    public Vector2 positionOffset;

    [Header("Enums")]
    public GENERAL_CLASSIFICATION generalClassification;
    public WEAPON_TYPE weaponType;

    [Header("Stats")]
    public string name;
    public string description;
    public float damage;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
