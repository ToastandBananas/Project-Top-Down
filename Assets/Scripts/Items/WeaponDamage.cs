using System.Collections;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public Equipment equipment;
    [HideInInspector] public ItemData itemData;

    public Vector2 positionOffset;

    public bool canDoDamage = true;

    int blockStaminaUse = 10;

    AnimTimeManager animTimeManager;
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    PlayerSpecialAttack playerSpecialAttack;
    BasicStats basicStats;
    NPCAttacks npcAttacks;
    Arms weaponOwnerArms;
    Transform thisWeapon;
    Transform weaponOwner;
    Transform weaponOwnerHeadReset;

    LayerMask obstacleMask;

    void Start()
    {
        thisWeapon = transform.parent.parent;
        weaponOwner = transform.parent.parent.parent.parent.parent.parent;
        weaponOwnerHeadReset = weaponOwner.Find("Head Reset");

        animTimeManager = GameManager.instance.GetComponent<AnimTimeManager>();
        playerAttack = weaponOwner.GetComponent<PlayerAttack>();
        playerSpecialAttack = weaponOwner.GetComponent<PlayerSpecialAttack>();
        basicStats = weaponOwner.GetComponent<BasicStats>();
        playerMovement = PlayerMovement.instance;
        npcAttacks = weaponOwner.GetComponent<NPCAttacks>();
        weaponOwnerArms = weaponOwner.Find("Arms").GetComponent<Arms>();
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

    IEnumerator ShieldRecoil(Animator anim, float waitTime)
    {
        anim.SetBool("doShieldRecoil", true);
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("doShieldRecoil", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDoDamage)
        {
            if (thisWeapon.name == "Left Weapon"
            && (playerAttack != null && (playerAttack.leftArmHeavyAttacking || playerAttack.leftQuickAttacking))
            || (npcAttacks != null && (npcAttacks.leftArmHeavyAttacking || npcAttacks.leftQuickAttacking)))
            {
                if (collision.transform.parent != null && collision.transform.parent != weaponOwner && (collision.tag == "NPC Body" || collision.tag == "Player Body"))
                {
                    BasicStats collisionBasicStats = collision.GetComponentInParent<BasicStats>();
                    collisionBasicStats.TakeDamage(itemData.damage);
                    canDoDamage = false;

                    float percentDamage = itemData.damage / collisionBasicStats.maxHealth;
                    collisionBasicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                    if (playerAttack != null)
                    {
                        if (playerAttack.leftArmHeavyAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(animTimeManager.leftChargeAttackTime));
                        }
                        else if (playerAttack.leftQuickAttacking)
                            StartCoroutine(DamageCooldown(animTimeManager.leftQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.leftArmHeavyAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(animTimeManager.leftHeavyAttackTime));
                        }
                        else if (npcAttacks.leftQuickAttacking)
                            StartCoroutine(DamageCooldown(animTimeManager.leftQuickAttackTime));
                    }
                }
                else if (collision.tag == "Shield" && collision.transform.parent.parent.parent.parent.parent.parent != weaponOwner)
                {
                    BasicStats shieldOwnerBasicStats = collision.transform.parent.parent.parent.parent.parent.parent.GetComponent<BasicStats>();

                    if (shieldOwnerBasicStats.UseStamina(blockStaminaUse, true))
                    {
                        Animator shieldOwnersArmAnim = collision.transform.parent.parent.parent.parent.GetComponent<Animator>();
                        StartCoroutine(ShieldRecoil(shieldOwnersArmAnim, animTimeManager.shieldRecoilTime));
                    }
                    
                    canDoDamage = false;
                    collision.GetComponent<ItemData>().durability -= itemData.damage;

                    if (playerAttack != null)
                    {
                        if (playerAttack.leftArmHeavyAttacking)
                        {
                            basicStats.UseStamina(Mathf.RoundToInt(blockStaminaUse * 1.5f), true);
                            StartCoroutine(DamageCooldown(animTimeManager.leftChargeAttackTime));
                            StartCoroutine(WeaponDeflect(weaponOwnerArms.leftArmAnim, animTimeManager.deflectWeaponTime));
                        }
                        else if (playerAttack.leftQuickAttacking)
                        {
                            basicStats.UseStamina(blockStaminaUse, true);
                            StartCoroutine(DamageCooldown(animTimeManager.leftQuickAttackTime));
                            weaponOwnerArms.leftArmAnim.SetBool("doQuickAttack", false);
                        }
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.leftArmHeavyAttacking)
                        {
                            basicStats.UseStamina(Mathf.RoundToInt(blockStaminaUse * 1.5f), true);
                            StartCoroutine(DamageCooldown(animTimeManager.leftHeavyAttackTime));
                            StartCoroutine(WeaponDeflect(weaponOwnerArms.leftArmAnim, animTimeManager.deflectWeaponTime));
                        }
                        else if (npcAttacks.leftQuickAttacking)
                        {
                            basicStats.UseStamina(blockStaminaUse, true);
                            StartCoroutine(DamageCooldown(animTimeManager.leftQuickAttackTime));
                            weaponOwnerArms.leftArmAnim.SetBool("doQuickAttack", false);
                        }
                    }
                }

            }
            else if (thisWeapon.name == "Right Weapon"
                && (playerAttack != null && (playerAttack.rightArmHeavyAttacking || playerAttack.rightQuickAttacking))
                || (npcAttacks != null && (npcAttacks.rightArmHeavyAttacking || npcAttacks.rightQuickAttacking)))
            {
                if (collision.transform.parent != null && collision.transform.parent != weaponOwner && (collision.tag == "NPC Body" || collision.tag == "Player Body"))
                {
                    BasicStats collisionBasicStats = collision.GetComponentInParent<BasicStats>();
                    collisionBasicStats.TakeDamage(itemData.damage);
                    canDoDamage = false;

                    float percentDamage = itemData.damage / collisionBasicStats.maxHealth;
                    collisionBasicStats.SpawnBlood(collision.transform, weaponOwner, percentDamage, obstacleMask);

                    if (playerAttack != null)
                    {
                        if (playerAttack.rightArmHeavyAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(animTimeManager.rightChargeAttackTime));
                        }
                        else if (playerAttack.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(animTimeManager.rightQuickAttackTime));
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.rightArmHeavyAttacking)
                        {
                            Knockback(collision.transform.parent, equipment.knockbackPower);
                            StartCoroutine(DamageCooldown(animTimeManager.rightHeavyAttackTime));
                        }
                        else if (npcAttacks.rightQuickAttacking)
                            StartCoroutine(DamageCooldown(animTimeManager.rightQuickAttackTime));
                    }
                }
                else if (collision.tag == "Shield" && collision.transform.parent.parent.parent.parent.parent.parent != weaponOwner)
                {
                    BasicStats shieldOwnerBasicStats = collision.transform.parent.parent.parent.parent.parent.parent.GetComponent<BasicStats>();

                    if (shieldOwnerBasicStats.UseStamina(blockStaminaUse, true))
                    {
                        Animator shieldOwnersArmAnim = collision.transform.parent.parent.parent.parent.GetComponent<Animator>();
                        StartCoroutine(ShieldRecoil(shieldOwnersArmAnim, animTimeManager.shieldRecoilTime));
                    }
                    
                    canDoDamage = false;
                    collision.GetComponent<ItemData>().durability -= itemData.damage;

                    if (playerAttack != null)
                    {
                        if (playerAttack.rightArmHeavyAttacking)
                        {
                            basicStats.UseStamina(Mathf.RoundToInt(blockStaminaUse * 1.5f), true);
                            StartCoroutine(DamageCooldown(animTimeManager.rightChargeAttackTime));
                            StartCoroutine(WeaponDeflect(weaponOwnerArms.rightArmAnim, animTimeManager.deflectWeaponTime));
                        }
                        else if (playerAttack.rightQuickAttacking)
                        {
                            basicStats.UseStamina(blockStaminaUse, true);
                            StartCoroutine(DamageCooldown(animTimeManager.rightQuickAttackTime));
                            weaponOwnerArms.rightArmAnim.SetBool("doQuickAttack", false);
                        }
                    }
                    else if (npcAttacks != null)
                    {
                        if (npcAttacks.rightArmHeavyAttacking)
                        {
                            basicStats.UseStamina(Mathf.RoundToInt(blockStaminaUse * 1.5f), true);
                            StartCoroutine(DamageCooldown(animTimeManager.rightHeavyAttackTime));
                            StartCoroutine(WeaponDeflect(weaponOwnerArms.rightArmAnim, animTimeManager.deflectWeaponTime));
                        }
                        else if (npcAttacks.rightQuickAttacking)
                        {
                            basicStats.UseStamina(blockStaminaUse, true);
                            StartCoroutine(DamageCooldown(animTimeManager.rightQuickAttackTime));
                            weaponOwnerArms.rightArmAnim.SetBool("doQuickAttack", false);
                        }
                    }
                }
            }
        }
    }
}
