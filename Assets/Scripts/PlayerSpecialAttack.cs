using System.Collections;
using UnityEngine;

public enum SPECIAL_ATTACK { NONE, SHIELD_BASH }

public class PlayerSpecialAttack : MonoBehaviour
{
    public SPECIAL_ATTACK leftSpecialAttackSlot, rightSpecialAttackSlot;
    // TODO: Potentially two special attack slots? ie: leftSpecialAttackSlot1 & 2 and righSpecialAttackSlot1 & 2 (using triangle and circle buttons?)
    
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    Arms arms;
    Transform headReset;
    
    Animator leftArmAnim, rightArmAnim;

    bool canDoDamage = true;
    
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = PlayerAttack.instance;
        arms = transform.root.Find("Arms").GetComponent<Arms>();
        headReset = playerMovement.transform.Find("Head Reset");
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
            switch (leftSpecialAttackSlot)
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
            switch (rightSpecialAttackSlot)
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

            //if (playerMovement.isLockedOn)
                //StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (playerMovement.lockOnTarget.transform.position - playerMovement.transform.position).normalized * 1f));
            //else
            StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (headReset.position - playerMovement.transform.position).normalized * 1f));

            StartCoroutine(ResetShieldBash(playerAttack.shieldBashTime));
        }
    }

    void RightShieldBash()
    {
        if (playerAttack.isBlocking)
        {
            rightArmAnim.SetBool("doShieldBash", true);

            if (playerMovement.isLockedOn)
                StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (playerMovement.lockOnTarget.transform.position - playerMovement.transform.position).normalized * 1f));
            else
                StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (headReset.position - playerMovement.transform.position).normalized * 1f));

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
