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

    public bool isPlayer;
    public bool isDead;

    [Header("Player Only")]
    public PlayerStatBar playerHealthStatBar;
    public PlayerStatBar playerManaStatBar;
    public PlayerStatBar playerStaminaStatBar;

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
        // TODO: Death animation
        Destroy(this.gameObject);
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
