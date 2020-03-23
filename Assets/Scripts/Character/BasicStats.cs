using System.Collections;
using UnityEngine;

public class BasicStats : MonoBehaviour
{
    public float level = 1;

    [Header("Health")]
    public float maxHealth = 50;
    public float health = 50;

    [Header("Mana")]
    public float maxMana = 50;
    public float mana = 50;

    [Header("Stamina")]
    public float maxStamina = 50;
    public float stamina = 50;
    public int staminaRegenRate = 5;

    [Header("Encumbrance")]
    public float maxEncumbrance = 100f;
    public float encumbrance = 0;

    [Header("Defense")]
    public float defense = 0;

    [Header("Other")]
    public bool isPlayer;
    public bool isDead;

    bool healthCanRegen = true;
    bool staminaCanRegen = true;
    bool manaCanRegen = true;

    ItemDrop leftWeaponItemDrop;
    ItemDrop rightWeaponItemDrop;

    [Header("Player Only")]
    PlayerStatBar playerHealthStatBar;
    PlayerStatBar playerManaStatBar;
    PlayerStatBar playerStaminaStatBar;

    Arms arms;

    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        playerHealthStatBar = GameObject.Find("Player Health Bar").GetComponent<PlayerStatBar>();
        playerManaStatBar = GameObject.Find("Player Mana Bar").GetComponent<PlayerStatBar>();
        playerStaminaStatBar = GameObject.Find("Player Stamina Bar").GetComponent<PlayerStatBar>();
    }

    void FixedUpdate()
    {
        if (staminaCanRegen)
            StartCoroutine(StaminaRegen());
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (isPlayer)
            playerHealthStatBar.ChangeBar();

        if (health <= 0)
        {
            isDead = true;
            Die();
        }
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
        if (health < goalHealth && health < maxHealth)
        {
            healthCanRegen = false;
            yield return new WaitForSeconds(0.2f);
            Heal(regenRate / 5);
            healthCanRegen = true;
        }
    }

    public void Die()
    {
        if (arms.leftWeaponEquipped || arms.leftShieldEquipped)
            leftWeaponItemDrop = transform.Find("Arms").Find("Left Arm").Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();
        if (arms.rightWeaponEquipped || arms.rightShieldEquipped)
            rightWeaponItemDrop = transform.Find("Arms").Find("Right Arm").Find("Right Forearm").Find("Right Weapon").GetChild(0).GetChild(0).GetComponent<ItemDrop>();

        if (leftWeaponItemDrop != null)
            leftWeaponItemDrop.DropItem(false);

        if (rightWeaponItemDrop != null)
            rightWeaponItemDrop.DropItem(false);

        // TODO: Death animation
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
        if (mana < goalMana && mana < maxMana)
        {
            manaCanRegen = false;
            yield return new WaitForSeconds(0.2f);
            RestoreMana(regenRate / 5);
            manaCanRegen = true;
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
        if (stamina < maxStamina)
        {
            staminaCanRegen = false;
            yield return new WaitForSeconds(0.2f);
            RestoreStamina(staminaRegenRate / 5);
            staminaCanRegen = true;
        }
    }
}
