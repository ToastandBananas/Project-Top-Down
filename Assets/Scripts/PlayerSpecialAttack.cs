using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPECIAL_ATTACK { SHIELD_BASH }

public class PlayerSpecialAttack : MonoBehaviour
{
    public SPECIAL_ATTACK[] thisWeaponsSpecialAttacks;
    public SPECIAL_ATTACK specialAttackSlotQ, specialAttackSlot1, specialAttackSlot2;
    public SPECIAL_ATTACK specialAttackSlotE, specialAttackSlot3, specialAttackSlot4;

    PlayerAttack playerAttack;
    Arms arms;
    
    Animator leftArmAnim, rightArmAnim;
    
    void Start()
    {
        playerAttack = PlayerAttack.instance;
        arms = transform.root.Find("Arms").GetComponent<Arms>();
        leftArmAnim = arms.transform.Find("Left Arm").GetComponent<Animator>();
        rightArmAnim = arms.transform.Find("Right Arm").GetComponent<Animator>();

        if (transform.root.tag != "Player")
            this.enabled = false;
    }
    
    void Update()
    {
        SpecialAttackLeft();
        SpecialAttackRight();
    }

    void SpecialAttackLeft()
    {
        if (GameControls.gamePlayActions.playerLeftSpecialAttack.WasPressed 
            && playerAttack.leftArmAttacking == false 
            && playerAttack.attackTimerLeftArm == 0 
            && (arms.leftWeaponEquipped || arms.leftShieldEquipped))
        { 
            switch (specialAttackSlotQ)
            {
                case SPECIAL_ATTACK.SHIELD_BASH:
                    LeftShieldBash();
                    break;
            }
        }
    }

    void SpecialAttackRight()
    {
        if (GameControls.gamePlayActions.playerRightSpecialAttack.WasPressed 
            && playerAttack.rightArmAttacking == false 
            && playerAttack.attackTimerRightArm == 0 
            && (arms.rightWeaponEquipped || arms.rightShieldEquipped))
        {
            switch (specialAttackSlotE)
            {
                case SPECIAL_ATTACK.SHIELD_BASH:
                    RightShieldBash();
                    break;
            }
        }
    }

    void LeftShieldBash()
    {
        if (playerAttack.isBlocking)
        {
            leftArmAnim.SetBool("doShieldBash", true);
            StartCoroutine(ResetShieldBash(playerAttack.shieldBashTime));
        }
    }

    void RightShieldBash()
    {
        if (playerAttack.isBlocking)
        {
            rightArmAnim.SetBool("doShieldBash", true);
            StartCoroutine(ResetShieldBash(playerAttack.shieldBashTime));
        }
    }

    IEnumerator ResetShieldBash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        leftArmAnim.SetBool("doShieldBash", false);
        rightArmAnim.SetBool("doShieldBash", false);
    }
}
