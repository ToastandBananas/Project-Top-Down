using UnityEngine;

public enum GENERAL_CLASSIFICATION { WEAPON_1H, WEAPON_2H, SHIELD }
public enum WEAPON_TYPE { SHIELD, SWORD, MACE, AXE, SPEAR, STAFF, BOW, CROSSBOW }

public class WeaponStats : MonoBehaviour
{
    public Vector2 positionOffset;

    [Header("Enums")]
    public GENERAL_CLASSIFICATION generalClassification;
    public WEAPON_TYPE weaponType;

    [Header("Stats")]
    public string weaponName;
    public string description;
    public float damage;

    void Start()
    {
        transform.position += new Vector3(positionOffset.x, positionOffset.y, 0);
    }
    
    void Update()
    {
        
    }
}
