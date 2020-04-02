using System.Collections;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public Equipment equipment;
    [HideInInspector] public ItemData itemData;

    public Vector2 positionOffset;

    bool canDoDamage = true;

    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    PlayerSpecialAttack playerSpecialAttack;
    NPCAttacks npcAttacks;
    Transform thisWeapon;
    Transform weaponOwner;
    Transform weaponOwnerHeadReset;

    LayerMask obstacleMask;

    void Start()
    {
        thisWeapon = transform.parent.parent;
        weaponOwner = transform.parent.parent.parent.parent.parent.parent;
        weaponOwnerHeadReset = weaponOwner.Find("Head Reset");
        playerAttack = weaponOwner.GetComponent<PlayerAttack>();
        playerSpecialAttack = weaponOwner.GetComponent<PlayerSpecialAttack>();
        playerMovement = PlayerMovement.instance;
        npcAttacks = weaponOwner.GetComponent<NPCAttacks>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");
        itemData = GetComponent<ItemData>();
        equipment = itemData.equipment;

        transform.position += new Vector3(positionOffset.x, positionOffset.y, 0);

        if (thisWeapon != null && thisWeapon.name == "Left Weapon")
            GetComponent<SpriteRenderer>().flipX = true;

        SetSpecialAttackSlots();
    }

    void SetSpecialAttackSlots()
    {
        if (thisWeapon != null)
        {
            if (thisWeapon.name == "Left Weapon" && equipment.thisWeaponsSpecialAttacks.Length > 0)
                playerSpecialAttack.leftSpecialAttackSlot = equipment.thisWeaponsSpecialAttacks[0];
            else if (thisWeapon.name == "Right Weapon" && equipment.thisWeaponsSpecialAttacks.Length > 0)
                playerSpecialAttack.rightSpecialAttackSlot = equipment.thisWeaponsSpecialAttacks[0];
        }
    }

    void Knockback(Transform whoToKnockback, float knockbackDistance)
    {
        Vector3 dir = (weaponOwnerHeadReset.position - weaponOwner.position).normalized;
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

    IEnumerator DamageCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canDoDamage = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisWeapon.name == "Left Weapon" 
            && (playerAttack != null && (playerAttack.leftArmAttacking || playerAttack.leftQuickAttacking))
            || (npcAttacks != null && (npcAttacks.leftArmAttacking || npcAttacks.leftQuickAttacking)))
        {
            if (canDoDamage && collision.gameObject != weaponOwner.gameObject && (collision.tag == "NPC" || collision.tag == "Player"))
            {
                BasicStats basicStats = collision.GetComponent<BasicStats>();
                basicStats.TakeDamage(itemData.damage);
                canDoDamage = false;

                float percentDamage = itemData.damage / basicStats.maxHealth;
                basicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                if (playerAttack != null)
                {
                    if (playerAttack.leftArmAttacking)
                    {
                        Knockback(collision.transform, equipment.knockbackPower);
                        StartCoroutine(DamageCooldown(playerAttack.leftChargeAttackTime));
                    }
                    else if (playerAttack.leftQuickAttacking)
                        StartCoroutine(DamageCooldown(playerAttack.leftQuickAttackTime));
                }
                else if (npcAttacks != null)
                {
                    if (npcAttacks.leftArmAttacking)
                    {
                        Knockback(collision.transform, equipment.knockbackPower);
                        StartCoroutine(DamageCooldown(npcAttacks.leftHeavyAttackTime));
                    }
                    else if (npcAttacks.leftQuickAttacking)
                        StartCoroutine(DamageCooldown(npcAttacks.leftQuickAttackTime));
                }
            }
        }
        else if (thisWeapon.name == "Right Weapon" 
            && (playerAttack != null && (playerAttack.rightArmAttacking || playerAttack.rightQuickAttacking))
            || (npcAttacks != null && (npcAttacks.rightArmAttacking || npcAttacks.rightQuickAttacking)))
        {
            if (canDoDamage && collision.gameObject != weaponOwner.gameObject && (collision.tag == "NPC" || collision.tag == "Player"))
            {
                BasicStats basicStats = collision.GetComponent<BasicStats>();
                basicStats.TakeDamage(itemData.damage);
                canDoDamage = false;

                float percentDamage = itemData.damage / basicStats.maxHealth;
                basicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                if (playerAttack != null)
                {
                    if (playerAttack.rightArmAttacking)
                    {
                        Knockback(collision.transform, equipment.knockbackPower);
                        StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                    }
                    else if (playerAttack.rightQuickAttacking)
                        StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
                }
                else if (npcAttacks != null)
                {
                    if (npcAttacks.rightArmAttacking)
                    {
                        Knockback(collision.transform, equipment.knockbackPower);
                        StartCoroutine(DamageCooldown(npcAttacks.rightHeavyAttackTime));
                    }
                    else if (npcAttacks.rightQuickAttacking)
                        StartCoroutine(DamageCooldown(npcAttacks.rightQuickAttackTime));
                }
            }
        }
    }
}
