using System.Collections;
using UnityEngine;

public enum GENERAL_CLASSIFICATION { WEAPON_1H, WEAPON_2H, SHIELD }
public enum WEAPON_TYPE { SHIELD, SWORD, MACE, AXE, SPEAR, STAFF, BOW, CROSSBOW }

public class Weapon : MonoBehaviour
{
    public Vector2 positionOffset;

    [Header("Enums")]
    public GENERAL_CLASSIFICATION generalClassification;
    public WEAPON_TYPE weaponType;
    public SPECIAL_ATTACK[] thisWeaponsSpecialAttacks;

    [Header("Stats")]
    public string weaponName;
    public string description;
    public float damage;

    bool canDoDamage = true;

    PlayerAttack playerAttack;
    PlayerSpecialAttack playerSpecialAttack;
    Transform thisWeapon;

    void Start()
    {
        playerAttack = PlayerAttack.instance;
        playerSpecialAttack = FindObjectOfType<PlayerSpecialAttack>();
        thisWeapon = transform.parent.parent;

        transform.position += new Vector3(positionOffset.x, positionOffset.y, 0);

        if (thisWeapon.name == "Left Weapon")
            GetComponent<SpriteRenderer>().flipX = true;

        SetSpecialAttackSlots();
    }

    void SetSpecialAttackSlots()
    {
        if (thisWeapon.name == "Left Weapon" && thisWeaponsSpecialAttacks.Length > 0)
            playerSpecialAttack.leftSpecialAttackSlot = thisWeaponsSpecialAttacks[0];
        else if (thisWeapon.name == "Right Weapon" && thisWeaponsSpecialAttacks.Length > 0)
            playerSpecialAttack.rightSpecialAttackSlot = thisWeaponsSpecialAttacks[0];
    }

    IEnumerator DamageCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canDoDamage = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisWeapon.name == "Left Weapon" && (playerAttack.leftArmAttacking || playerAttack.leftQuickAttacking))
        {
            if (canDoDamage && collision.tag == "NPC")
            {
                collision.GetComponent<BasicStats>().TakeDamage(10f);
                Debug.Log(collision.GetComponent<BasicStats>().health);
                canDoDamage = false;

                if (playerAttack.leftArmAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.leftChargeAttackTime));
                else if (playerAttack.leftQuickAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.leftQuickAttackTime));
            }
        }
        else if (thisWeapon.name == "Right Weapon" && (playerAttack.rightArmAttacking || playerAttack.rightQuickAttacking))
        {
            if (canDoDamage && collision.tag == "NPC")
            {
                collision.GetComponent<BasicStats>().TakeDamage(10f);
                Debug.Log(collision.GetComponent<BasicStats>().health);
                canDoDamage = false;

                if (playerAttack.rightArmAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                else if (playerAttack.rightQuickAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
            }
        }
    }
}
