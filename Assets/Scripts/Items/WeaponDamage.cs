using System.Collections;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public Equipment equipment;
    [HideInInspector] public ItemData itemData;

    public Vector2 positionOffset;

    public bool canDoDamage = true;

    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    PlayerSpecialAttack playerSpecialAttack;
    NPCAttacks npcAttacks;
    Arms arms;
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
        arms = weaponOwner.Find("Arms").GetComponent<Arms>();
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

    IEnumerator WeaponDeflect(Animator anim, float waitTime)
    {
        anim.SetBool("doDeflectWeapon", true);
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("doDeflectWeapon", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDoDamage)
        {
            if (thisWeapon.name == "Left Weapon"
            && (playerAttack != null && (playerAttack.leftArmAttacking || playerAttack.leftQuickAttacking))
            || (npcAttacks != null && (npcAttacks.leftArmAttacking || npcAttacks.leftQuickAttacking)))
            {
                if (collision.transform.parent != null && collision.transform.parent != weaponOwner && (collision.tag == "NPC Body" || collision.tag == "Player Body"))
                {
                    BasicStats basicStats = collision.GetComponentInParent<BasicStats>();
                    basicStats.TakeDamage(itemData.damage);
                    canDoDamage = false;

                    float percentDamage = itemData.damage / basicStats.maxHealth;
                    basicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                    if (playerAttack != null)
                    {
                        if (playerAttack.leftArmAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(playerAttack.leftChargeAttackTime));
                        }
                        else if (playerAttack.leftQuickAttacking)
                            StartCoroutine(DamageCooldown(playerAttack.leftQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.leftArmAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(npcAttacks.leftHeavyAttackTime));
                        }
                        else if (npcAttacks.leftQuickAttacking)
                            StartCoroutine(DamageCooldown(npcAttacks.leftQuickAttackTime));
                    }
                }
                else if (collision.tag == "Shield" && collision.transform.parent.parent.parent.parent.parent.parent != weaponOwner)
                {
                    Debug.Log("Damage blocked");
                    canDoDamage = false;
                    collision.GetComponent<ItemData>().durability -= itemData.damage;

                    if (playerAttack != null)
                    {
                        if (playerAttack.leftArmAttacking)
                        {
                            StartCoroutine(DamageCooldown(playerAttack.leftChargeAttackTime));
                            StartCoroutine(WeaponDeflect(arms.leftArmAnim, playerAttack.deflectWeaponTime));
                        }
                        else if (playerAttack.leftQuickAttacking)
                            StartCoroutine(DamageCooldown(playerAttack.leftQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.leftArmAttacking)
                        {
                            StartCoroutine(DamageCooldown(npcAttacks.leftHeavyAttackTime));
                            StartCoroutine(WeaponDeflect(arms.leftArmAnim, npcAttacks.deflectWeaponTime));
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
                if (collision.transform.parent != null && collision.transform.parent != weaponOwner && (collision.tag == "NPC Body" || collision.tag == "Player Body"))
                {
                    BasicStats basicStats = collision.GetComponentInParent<BasicStats>();
                    basicStats.TakeDamage(itemData.damage);
                    canDoDamage = false;

                    float percentDamage = itemData.damage / basicStats.maxHealth;
                    basicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                    if (playerAttack != null)
                    {
                        if (playerAttack.rightArmAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                        }
                        else if (playerAttack.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.rightArmAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(npcAttacks.rightHeavyAttackTime));
                        }
                        else if (npcAttacks.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(npcAttacks.rightQuickAttackTime));
                    }
                }
                else if (collision.tag == "Shield" && collision.transform.parent.parent.parent.parent.parent.parent != weaponOwner)
                {
                    Debug.Log("Damage blocked");
                    canDoDamage = false;
                    collision.GetComponent<ItemData>().durability -= itemData.damage;

                    if (playerAttack != null)
                    {
                        if (playerAttack.rightArmAttacking)
                        {
                            StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                            StartCoroutine(WeaponDeflect(arms.rightArmAnim, playerAttack.deflectWeaponTime));
                        }
                        else if (playerAttack.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.rightArmAttacking)
                        {
                            StartCoroutine(DamageCooldown(npcAttacks.rightHeavyAttackTime));
                            StartCoroutine(WeaponDeflect(arms.rightArmAnim, npcAttacks.deflectWeaponTime));
                        }
                        else if (npcAttacks.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(npcAttacks.rightQuickAttackTime));
                    }
                }
            }
        }
    }
}
