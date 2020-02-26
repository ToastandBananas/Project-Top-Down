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
    public float defense = 0;

    public bool isPlayer;
    public bool isDead;

    public ItemDrop leftWeapon;
    public ItemDrop rightWeapon;

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
        if (transform.Find("Arms").Find("Left Arm").GetChild(0).GetChild(0).childCount > 0)
            leftWeapon = transform.Find("Arms").Find("Left Arm").GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<ItemDrop>();
        if (transform.Find("Arms").Find("Right Arm").GetChild(0).GetChild(0).childCount > 0)
            rightWeapon = transform.Find("Arms").Find("Right Arm").GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<ItemDrop>();

        if (leftWeapon != null)
            leftWeapon.DropItem();

        if (rightWeapon != null)
            rightWeapon.DropItem();

        // TODO: Death animation
        transform.position = new Vector2(10000, 10000);
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
