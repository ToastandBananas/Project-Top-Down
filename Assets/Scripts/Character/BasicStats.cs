using System.Collections;
using UnityEngine;

public enum ALLIANCE { NEUTRAL, PLAYER, BANDITS, GUARDS };

public class BasicStats : MonoBehaviour
{
    [Header("General")]
    public float level = 1;
    public float gold = 0;
    public ALLIANCE currentAlliance = ALLIANCE.NEUTRAL;
    public ALLIANCE[] enemyAlliances;

    [Header("Health")]
    public float maxHealth = 50;
    public float health = 50;
    public float healthRegenRate;
    public bool healthCanRegen = true;

    [Header("Mana")]
    public float maxMana = 50;
    public float mana = 50;
    public float manaRegenRate;
    public bool manaCanRegen = true;

    [Header("Stamina")]
    public float maxStamina = 50;
    public float stamina = 50;
    public float staminaRegenRate = 5;
    public float staminaRegenEfficiency = 1f;
    public bool staminaCanRegen = true;
    float staminaRegenTimer = 0;

    [Header("Encumbrance")]
    public float maxEncumbrance = 100f;
    public float encumbrance = 0;

    [Header("Defense")]
    public float defense = 0;

    [Header("Skills")]
    [HideInInspector] public int maxSkillPoints = 100;
    public int rangedSkill = 20;

    [Header("Other")]
    public GameObject deadBodyPrefab;
    public bool isPlayer;
    public bool isDead;

    ItemDrop leftWeaponItemDrop;
    ItemDrop rightWeaponItemDrop;

    // Player only
    PlayerStatBar playerHealthStatBar;
    PlayerStatBar playerManaStatBar;
    PlayerStatBar playerStaminaStatBar;
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;

    // NPC only
    NPCInventory npcInv;
    NPCMovement npcMovement;
    NPCAttacks npcAttacks;

    AudioManager audioManager;
    BloodParticleSystemHandler bloodSystem;
    Arms arms;
    EquipmentManager equipmentManager;

    void Start()
    {
        audioManager = AudioManager.instance;
        bloodSystem = BloodParticleSystemHandler.Instance;
        arms = GetComponentInChildren<Arms>();
        equipmentManager = GetComponent<EquipmentManager>();

        if (isPlayer)
        {
            playerMovement = PlayerMovement.instance;
            playerAttack = PlayerAttack.instance;
            playerHealthStatBar = GameObject.Find("Player Health Bar").GetComponent<PlayerStatBar>();
            playerManaStatBar = GameObject.Find("Player Mana Bar").GetComponent<PlayerStatBar>();
            playerStaminaStatBar = GameObject.Find("Player Stamina Bar").GetComponent<PlayerStatBar>();
        }
        else
        {
            npcInv = GetComponent<NPCInventory>();
            npcMovement = GetComponent<NPCMovement>();
            npcAttacks = GetComponent<NPCAttacks>();
        }

        StartCoroutine(StaminaRegen());
    }

    public void SpawnBlood(Transform victim, Transform weaponOwner, float percentDamage, LayerMask obstacleMask)
    {
        Vector3 dir = (victim.position - weaponOwner.position).normalized;
        float raycastDistance = Vector3.Distance(victim.position, victim.position + dir * 3f);
        RaycastHit2D hit = Physics2D.Raycast(victim.position, dir, raycastDistance, obstacleMask);
        
        if (hit == false)
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, percentDamage, false);
        else
            bloodSystem.SpawnBlood(victim.position + dir * 0.5f, dir, percentDamage, true);
    }
    
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (isPlayer)
            playerHealthStatBar.ChangeBar();

        if (health <= 0)
            StartCoroutine(Die());
        else
            audioManager.PlayRandomSound(audioManager.humanMaleGruntSounds, transform.position);
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
            health = maxHealth;

        if (isPlayer)
            playerHealthStatBar.ChangeBar();
    }

    public IEnumerator HealthRegen(float goalHealth, int regenRate)
    {
        if (healthCanRegen && health < goalHealth && health < maxHealth)
        {
            yield return new WaitForSeconds(0.2f);
            Heal(regenRate / 5);
        }
    }

    public IEnumerator HealthDrain(float duration, float drainRate)
    {
        if (duration > 0)
        {
            float timer = 0;
            while (timer < duration && isDead == false)
            {
                TakeDamage(drainRate / 5);
                yield return new WaitForSeconds(0.2f);
                timer += 0.2f;
            }
        }
        else
        {
            while (true)
            {
                TakeDamage(drainRate / 5);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public IEnumerator Die()
    {
        isDead = true;
        audioManager.PlayRandomSound(audioManager.humanMaleDeathSounds, transform.position);

        // Get ItemDrop scripts of carried weapons/shields
        if (transform.Find("Arms").Find("Left Arm").Find("Left Forearm").Find("Left Weapon").childCount > 0 &&
            (arms.leftWeaponEquipped || arms.leftShieldEquipped || arms.rangedWeaponEquipped || arms.twoHanderEquipped))
        {
            leftWeaponItemDrop = transform.Find("Arms").Find("Left Arm").Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();
        }
        if (transform.Find("Arms").Find("Right Arm").Find("Right Forearm").Find("Right Weapon").childCount > 0 && arms.rightWeaponEquipped || arms.rightShieldEquipped)
        {
            rightWeaponItemDrop = transform.Find("Arms").Find("Right Arm").Find("Right Forearm").Find("Right Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();
        }

        yield return new WaitForSeconds(0.025f);

        GameObject deadBody = Instantiate(deadBodyPrefab, transform.position, transform.rotation, GameObject.Find("NPCs").transform);
        StartCoroutine(equipmentManager.TransferEquippedItemsToBody(deadBody.GetComponent<EquipmentManager>()));

        if (npcInv != null)
        {
            int randomNum;

            // Get arrows from the character's body and possibly add them to the dead body container
            if (transform.Find("Body").Find("Arrows") != null)
            {
                for (int i = 0; i < transform.Find("Body").Find("Arrows").childCount; i++)
                {
                    // 50% chance to add the arrow to the npcs inv
                    randomNum = Random.Range(1, 3);
                    if (randomNum == 1)
                    { 
                        GameObject prefab = Instantiate(transform.Find("Body").Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab);
                        prefab.GetComponent<ItemData>().hasBeenRandomized = true;
                        prefab.GetComponent<ItemData>().TransferData(transform.Find("Body").Find("Arrows").GetChild(i).GetComponent<ItemData>(), prefab.GetComponent<ItemData>());
                        npcInv.carriedItems.Add(prefab);
                    }
                }
            }

            // Get arrows from the character's shield (if they had one equipped) and possibly add them to the dead body container
            if (leftWeaponItemDrop != null && leftWeaponItemDrop.transform.Find("Arrows") != null)
            {
                for (int i = 0; i < leftWeaponItemDrop.transform.Find("Arrows").childCount; i++)
                {
                    // 50% chance to add the arrow to the npcs inv
                    randomNum = Random.Range(1, 3);
                    if (randomNum == 1)
                    {
                        GameObject prefab = Instantiate(leftWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab);
                        prefab.GetComponent<ItemData>().hasBeenRandomized = true;
                        prefab.GetComponent<ItemData>().TransferData(leftWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>(), prefab.GetComponent<ItemData>());
                        npcInv.carriedItems.Add(prefab);
                    }
                    
                    Destroy(leftWeaponItemDrop.transform.Find("Arrows").GetChild(i).gameObject);
                }
            }

            // Get arrows from the character's shield (if they had one equipped) and possibly add them to the dead body container
            if (rightWeaponItemDrop != null && rightWeaponItemDrop.transform.Find("Arrows") != null)
            {
                for (int i = 0; i < rightWeaponItemDrop.transform.Find("Arrows").childCount; i++)
                {
                    // 50% chance to add the arrow to the npcs inv
                    randomNum = Random.Range(1, 3);
                    if (randomNum == 1)
                    {
                        GameObject prefab = Instantiate(rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab);
                        prefab.GetComponent<ItemData>().hasBeenRandomized = true;
                        prefab.GetComponent<ItemData>().TransferData(rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>(), prefab.GetComponent<ItemData>());
                        npcInv.carriedItems.Add(prefab);
                    }

                    Destroy(rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).gameObject);
                }
            }

            StartCoroutine(npcInv.TransferObjectsToBodyContainer(deadBody.GetComponent<Container>()));
        }

        // Drop any carried weapons/shields
        if (leftWeaponItemDrop != null)
        {
            if (arms.rangedWeaponEquipped && arms.leftEquippedWeapon.transform.Find("Middle of String").childCount > 0) // If an arrow is attached to the bow string, drop it
                arms.leftEquippedWeapon.transform.Find("Middle of String").GetChild(0).GetComponent<ItemDrop>().DropItem(false);

            leftWeaponItemDrop.DropItem(false);
        }

        if (rightWeaponItemDrop != null)
            rightWeaponItemDrop.DropItem(false);

        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }

    public void UseMana(float manaAmount)
    {
        if (mana - manaAmount >= 0)
            mana -= manaAmount;
        else
            Debug.Log("Not enough mana...");

        if (isPlayer)
            playerManaStatBar.ChangeBar();
    }

    public void RestoreMana(float manaAmount)
    {
        mana += manaAmount;
        if (mana > maxMana)
            mana = maxMana;

        if (isPlayer)
            playerManaStatBar.ChangeBar();
    }

    public IEnumerator ManaRegen(float goalMana, int regenRate)
    {
        if (manaCanRegen && mana < goalMana && mana < maxMana)
        {
            yield return new WaitForSeconds(0.2f);
            RestoreMana(regenRate / 5);
        }
    }

    public IEnumerator ManaDrain(float duration, float drainRate)
    {
        if (duration > 0)
        {
            float timer = 0;
            while (timer < duration && isDead == false)
            {
                UseMana(drainRate / 5);
                yield return new WaitForSeconds(0.2f);
                timer += 0.2f;
            }
        }
        else
        {
            while (true)
            {
                UseMana(drainRate / 5);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public bool UseStamina(float staminaAmount, bool forceStaminaUse)
    {
        if (stamina - staminaAmount >= 0 || forceStaminaUse)
        {
            stamina -= staminaAmount;
            StartCoroutine(PauseStaminaRegen());
        }
        else
        {
            Debug.Log("Not enough stamina...");
            return false;
        }

        if (isPlayer)
        {
            if (stamina < 0)
            {
                stamina = 0;
                playerStaminaStatBar.ChangeBar();
                StartCoroutine(playerMovement.Stagger());
                return false;
            }
            playerStaminaStatBar.ChangeBar();
        }
        else
        {
            if (stamina < 0)
            {
                stamina = 0;
                StartCoroutine(npcMovement.Stagger());
                return false;
            }
        }

        return true;
    }

    public void RestoreStamina(float staminaAmount)
    {
        stamina += staminaAmount;
        if (stamina > maxStamina)
            stamina = maxStamina;

        if (isPlayer)
            playerStaminaStatBar.ChangeBar();
    }

    IEnumerator StaminaRegen()
    {
        while (true)
        {
            if (staminaCanRegen && stamina < maxStamina && staminaRegenRate > 0 && staminaRegenEfficiency > 0)
            {
                if ((isPlayer && playerAttack.isBlocking == false) || (isPlayer == false && npcAttacks.isBlocking == false))
                {
                    yield return new WaitForSeconds(0.2f);
                    RestoreStamina((staminaRegenRate * staminaRegenEfficiency) / 5);
                }
            }

            yield return null;
        }
    }

    IEnumerator PauseStaminaRegen()
    {
        staminaRegenTimer = 0;
        staminaCanRegen = false;

        while (staminaRegenTimer < 2f)
        {
            staminaRegenTimer += Time.smoothDeltaTime;

            if (staminaRegenTimer >= 2f)
            {
                staminaCanRegen = true;
                break;
            }

            yield return null;
        }
    }

    public IEnumerator StaminaDrain(float duration, float drainRate)
    {
        if (duration > 0)
        {
            float timer = 0;
            while (timer < duration && isDead == false)
            {
                UseStamina(drainRate / 5, true);
                yield return new WaitForSeconds(0.2f);
                timer += 0.2f;
            }
        }
        else
        {
            while (true)
            {
                UseStamina(drainRate / 5, true);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
