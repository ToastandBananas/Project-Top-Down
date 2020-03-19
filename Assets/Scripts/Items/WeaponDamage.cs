using System.Collections;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [HideInInspector] public Equipment equipment;
    [HideInInspector] public ItemData itemData;

    public Vector2 positionOffset;

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
        bloodSystem = BloodParticleSystemHandler.Instance;
        playerAttack = PlayerAttack.instance;
        playerMovement = playerAttack.GetComponent<PlayerMovement>();
        playerSpecialAttack = FindObjectOfType<PlayerSpecialAttack>();
        thisWeapon = transform.parent.parent;
        weaponOwner = transform.parent.parent.parent.parent.parent.parent;
        weaponOwnwerHeadReset = weaponOwner.Find("Head Reset");
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

    void SpawnBlood(Transform victim, float percentDamage)
    {
        Vector3 dir = (victim.position - weaponOwner.position).normalized;
        float raycastDistance = Vector3.Distance(victim.position, victim.position + dir * 3f);
        RaycastHit2D hit = Physics2D.Raycast(victim.position, dir, raycastDistance, obstacleMask);

        if (hit == false)
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, percentDamage, false);
        else
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, percentDamage, true);
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
                BasicStats NPCStats = collision.GetComponent<BasicStats>();
                NPCStats.TakeDamage(itemData.damage);
                canDoDamage = false;

                float percentDamage = itemData.damage / NPCStats.maxHealth;
                SpawnBlood(collision.transform, percentDamage);

                if (playerAttack.leftArmAttacking)
                {
                    Knockback(collision.transform, equipment.knockbackPower);
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
                BasicStats NPCStats = collision.GetComponent<BasicStats>();
                NPCStats.TakeDamage(itemData.damage);
                canDoDamage = false;

                float percentDamage = itemData.damage / NPCStats.maxHealth;
                SpawnBlood(collision.transform, percentDamage);

                if (playerAttack.rightArmAttacking)
                {
                    Knockback(collision.transform, equipment.knockbackPower);
                    StartCoroutine(DamageCooldown(playerAttack.rightChargeAttackTime));
                }
                else if (playerAttack.rightQuickAttacking)
                    StartCoroutine(DamageCooldown(playerAttack.rightQuickAttackTime));
            }
        }
    }
}
