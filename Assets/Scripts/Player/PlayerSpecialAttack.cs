using System.Collections;
using UnityEngine;

public enum SpecialAttack { NONE, SHIELD_BASH }

public class PlayerSpecialAttack : MonoBehaviour
{
    public SpecialAttack leftSpecialAttackSlot, rightSpecialAttackSlot;
    // TODO: Potentially two special attack slots? ie: leftSpecialAttackSlot1 & 2 and righSpecialAttackSlot1 & 2 (using triangle and circle buttons?)
    
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    AnimTimeManager animTimeManager;
    LockOn playerLockOnScript;
    Arms arms;
    Transform headReset;
    
    Animator leftArmAnim, rightArmAnim;

    //bool canDoDamage = true;
    
    void Start()
    {
        playerMovement = PlayerMovement.instance;
        playerAttack = PlayerAttack.instance;
        animTimeManager = GetComponent<AnimTimeManager>();
        playerLockOnScript = GetComponent<LockOn>();
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
            && playerAttack.leftArmHeavyAttacking == false 
            && playerAttack.attackTimer == 0 
            && (arms.leftWeaponEquipped || arms.leftShieldEquipped))
        { 
            switch (leftSpecialAttackSlot)
            {
                case SpecialAttack.SHIELD_BASH:
                    LeftShieldBash();
                    break;
            }
        }
    }

    void SpecialAttackRight()
    {
        if (GameControls.gamePlayActions.playerRightSpecialAttack.WasPressed 
            && playerAttack.rightArmHeavyAttacking == false 
            && playerAttack.attackTimer == 0 
            && (arms.rightWeaponEquipped || arms.rightShieldEquipped))
        {
            switch (rightSpecialAttackSlot)
            {
                case SpecialAttack.SHIELD_BASH:
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

            StartCoroutine(ResetShieldBash(animTimeManager.shieldBashTime));
        }
    }

    void RightShieldBash()
    {
        if (playerAttack.isBlocking)
        {
            rightArmAnim.SetBool("doShieldBash", true);

            if (playerLockOnScript.isLockedOn)
                StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (playerLockOnScript.lockOnTarget.transform.position - playerMovement.transform.position).normalized * 1f));
            else
                StartCoroutine(playerMovement.SmoothMovement(playerMovement.transform.position + (headReset.position - playerMovement.transform.position).normalized * 1f));

            StartCoroutine(ResetShieldBash(animTimeManager.shieldBashTime));
        }
    }

    IEnumerator ResetShieldBash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        leftArmAnim.SetBool("doShieldBash", false);
        rightArmAnim.SetBool("doShieldBash", false);
    }
}
