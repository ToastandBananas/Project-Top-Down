using UnityEngine;

public class BasicStats : MonoBehaviour
{
    public int level = 1;

    public float maxHealth = 50f;
    public float health = 50f;
    public float maxStamina = 50f;
    public float stamina = 50f;
    public float maxMana = 50f;
    public float mana = 50f;
    public float maxEncumbrance = 100f;
    public float encumbrance = 0;
    public float defense = 0;

    public bool isPlayer;
    public bool isDead;

    public ItemDrop leftWeaponItemDrop;
    public ItemDrop rightWeaponItemDrop;

    [Header("Player Only")]
    public PlayerStatBar playerHealthStatBar;
    public PlayerStatBar playerManaStatBar;
    public PlayerStatBar playerStaminaStatBar;

    Arms arms;

    void Start()
    {
        arms = GetComponentInChildren<Arms>();
    }

    public void TakeDamage(float damageAmount)
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

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
            health = maxHealth;

        if (isPlayer)
            playerHealthStatBar.ChangeBar();
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

    public void UseStamina(float staminaAmount)
    {
        if (stamina - staminaAmount >= 0)
            stamina -= staminaAmount;
        else
            Debug.Log("Not enough stamina...");

        if (isPlayer)
            playerStaminaStatBar.ChangeBar();
    }

    public void RestoreStamina(float staminaAmount)
    {
        stamina += staminaAmount;
        if (stamina > maxStamina)
            stamina = maxStamina;

        if (isPlayer)
            playerStaminaStatBar.ChangeBar();
    }
}
