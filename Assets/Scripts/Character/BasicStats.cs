using System.Collections;
using UnityEngine;

public class BasicStats : MonoBehaviour
{
    public float level = 1;

    [Header("Health")]
    public float maxHealth = 50;
    public float health = 50;
    public bool healthCanRegen = true;

    [Header("Mana")]
    public float maxMana = 50;
    public float mana = 50;
    public bool manaCanRegen = true;

    [Header("Stamina")]
    public float maxStamina = 50;
    public float stamina = 50;
    public int staminaRegenRate = 5;
    public bool staminaCanRegen = true;

    [Header("Encumbrance")]
    public float maxEncumbrance = 100f;
    public float encumbrance = 0;

    [Header("Defense")]
    public float defense = 0;

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

    // NPC only
    NPCInventory npcInv;

    BloodParticleSystemHandler bloodSystem;
    Arms arms;
    EquipmentManager equipmentManager;

    void Start()
    {
        bloodSystem = BloodParticleSystemHandler.Instance;
        arms = GetComponentInChildren<Arms>();
        equipmentManager = GetComponent<EquipmentManager>();
        npcInv = GetComponent<NPCInventory>();

        if (isPlayer)
        {
            playerHealthStatBar = GameObject.Find("Player Health Bar").GetComponent<PlayerStatBar>();
            playerManaStatBar = GameObject.Find("Player Mana Bar").GetComponent<PlayerStatBar>();
            playerStaminaStatBar = GameObject.Find("Player Stamina Bar").GetComponent<PlayerStatBar>();
        }
    }

    void FixedUpdate()
    {
        if (staminaCanRegen)
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

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (isPlayer)
            playerHealthStatBar.ChangeBar();

        if (health <= 0)
            StartCoroutine(Die());
    }

    public void Heal(int healAmount)
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

    public IEnumerator Die()
    {
        isDead = true;

        if (arms.leftWeaponEquipped || arms.leftShieldEquipped)
            leftWeaponItemDrop = transform.Find("Arms").Find("Left Arm").Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();
        if (arms.rightWeaponEquipped || arms.rightShieldEquipped)
            rightWeaponItemDrop = transform.Find("Arms").Find("Right Arm").Find("Right Forearm").Find("Right Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();

        yield return new WaitForSeconds(0.05f);

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
                        GameObject prefab = transform.Find("Body").Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab;
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
                        GameObject prefab = leftWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab;
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
                        GameObject prefab = rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>().item.prefab;
                        prefab.GetComponent<ItemData>().hasBeenRandomized = true;
                        prefab.GetComponent<ItemData>().TransferData(rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).GetComponent<ItemData>(), prefab.GetComponent<ItemData>());
                        npcInv.carriedItems.Add(prefab);
                    }

                    Destroy(rightWeaponItemDrop.transform.Find("Arrows").GetChild(i).gameObject);
                }
            }

            StartCoroutine(npcInv.TransferObjectsToBodyContainer(deadBody.GetComponent<Container>()));
        }

        if (leftWeaponItemDrop != null)
            leftWeaponItemDrop.DropItem(false);

        if (rightWeaponItemDrop != null)
            rightWeaponItemDrop.DropItem(false);

        yield return new WaitForSeconds(0.15f);
        
        Destroy(gameObject);
    }

    public void UseMana(int manaAmount)
    {
        if (mana - manaAmount >= 0)
            mana -= manaAmount;
        else
            Debug.Log("Not enough mana...");

        if (isPlayer)
            playerManaStatBar.ChangeBar();
    }

    public void RestoreMana(int manaAmount)
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

    public bool UseStamina(int staminaAmount)
    {
        if (stamina - staminaAmount >= 0)
            stamina -= staminaAmount;
        else
        {
            Debug.Log("Not enough stamina...");
            return false;
        }

        if (isPlayer)
            playerStaminaStatBar.ChangeBar();

        return true;
    }

    public void RestoreStamina(int staminaAmount)
    {
        stamina += staminaAmount;
        if (stamina > maxStamina)
            stamina = maxStamina;

        if (isPlayer)
            playerStaminaStatBar.ChangeBar();
    }

    public IEnumerator StaminaRegen()
    {
        if (staminaCanRegen && stamina < maxStamina)
        {
            yield return new WaitForSeconds(0.2f);
            RestoreStamina(staminaRegenRate / 5);
        }
    }
}
