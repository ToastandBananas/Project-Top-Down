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
    public float damage = 1f;
    public float knockbackPower = 1f;

    bool canDoDamage = true;

    BloodParticleSystemHandler bloodSystem;
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    PlayerSpecialAttack playerSpecialAttack;
    Transform thisWeapon;
    Transform weaponOwner;
    Transform weaponOwnwerHeadReset;

    LayerMask obstacleMask;

    void Start()
    {
        weaponName = name;
        bloodSystem = BloodParticleSystemHandler.Instance;
        playerAttack = PlayerAttack.instance;
        playerMovement = playerAttack.GetComponent<PlayerMovement>();
        playerSpecialAttack = FindObjectOfType<PlayerSpecialAttack>();
        thisWeapon = transform.parent.parent;
        weaponOwner = transform.parent.parent.parent.parent.parent.parent;
        weaponOwnwerHeadReset = weaponOwner.Find("Head Reset");
        obstacleMask = LayerMask.GetMask("Walls", "Doors");

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

    void Knockback(Transform whoToKnockback, float knockbackDistance)
    {
        Vector3 dir = (weaponOwnwerHeadReset.position - weaponOwner.position).normalized;
        float raycastDistance = Vector3.Distance(whoToKnockback.position, whoToKnockback.position + dir * knockbackDistance);
        RaycastHit2D hit = Physics2D.Raycast(whoToKnockback.position, dir, raycastDistance, obstacleMask);

        if (hit == false)
        {
            if (whoToKnockback.tag == "NPC")
                StartCoroutine(whoToKnockback.GetComponent<NPCMovement>().SmoothMovement(whoToKnockback.position + dir * knockbackDistance));
            else if (whoToKnockback.tag == "Player")
                StartCoroutine(playerMovement.SmoothMovement(whoToKnockback.position + dir * knockbackDistance));
        }
    }

    void SpawnBlood(Transform victim)
    {
        Vector3 dir = (victim.position - weaponOwner.position).normalized;
        float raycastDistance = Vector3.Distance(victim.position, victim.position + dir * 3f);
        RaycastHit2D hit = Physics2D.Raycast(victim.position, dir, raycastDistance, obstacleMask);

        if (hit == false)
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, false);
        else
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, true);
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
                collision.GetComponent<BasicStats>().TakeDamage(damage);
                Debug.Log(collision.GetComponent<BasicStats>().health);
                canDoDamage = false;
                SpawnBlood(collision.transform);

                if (playerAttack.leftArmAttacking)
                {
                    Knockback(collision.transform, knockbackPower);
                    StartCoroutine(DamageCooldown(playerAttack.leftChargeAttackTime));
                }
                else if (playerAttack.leftQuickAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.leftQuickAttackTime));
            }
        }
        else if (thisWeapon.name == "Right Weapon" && (playerAttack.rightArmAttacking || playerAttack.rightQuickAttacking))
        {
            if (canDoDamage && collision.tag == "NPC")
            {
                collision.GetComponent<BasicStats>().TakeDamage(damage);
                Debug.Log(collision.GetComponent<BasicStats>().health);
                canDoDamage = false;
                SpawnBlood(collision.transform);

                if (playerAttack.rightArmAttacking)
                {
                    Knockback(collision.transform, knockbackPower);
                    StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                }
                else if (playerAttack.rightQuickAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
            }
        }
    }
}
