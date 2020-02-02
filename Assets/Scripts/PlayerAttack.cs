using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QUICK_ATTACK_TURN { LEFT, RIGHT }

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    QUICK_ATTACK_TURN currentQuickAttackTurn;

    Arms arms;
    Animator rightArmAnim, leftArmAnim;
    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;
    
    [HideInInspector]
    public float attackTimerLeftArm = 0;
    [HideInInspector]
    public float attackTimerRightArm = 0;

    float leftQuickAttackTime, rightQuickAttackTime;
    float leftChargeAttackTime, rightChargeAttackTime;

    [HideInInspector]
    public float shieldBashTime;

    public float minChargeAttackTime = 0.3f;
    public bool isBlocking;
    public bool leftArmAttacking, rightArmAttacking;

    bool leftQuickAttacking, rightQuickAttacking;
    bool isUsingController;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {
        arms = transform.Find("Arms").GetComponent<Arms>();
        rightArmAnim = arms.transform.Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = arms.transform.Find("Left Arm").GetComponent<Animator>();

        UpdateAnimClipTimes();
    }
    
    void Update()
    {
        Update_LeftArmAnims();
        Update_RightArmAnims();

        // Debug.Log(attackTimerLeftArm);
        // Debug.Log(attackTimerRightArm);
    }

    /*void Update_QuickAttack()
    {
        // TODO: Set cooldowns
        if (Input.GetButton("Quick Attack") && quickAttacking == false)
        {
            if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // Alternate left/right quick attacks
            {
                if (currentQuickAttackTurn == QUICK_ATTACK_TURN.LEFT && leftArmAttacking == false && attackTimerLeftArm == 0)
                {
                    quickAttacking = true;
                    leftArmAnim.SetBool("doQuickAttack", true);
                    currentQuickAttackTurn = QUICK_ATTACK_TURN.RIGHT;
                    StartCoroutine(ResetQuickAttackTimer(leftQuickAttackTime / 2));
                }
                else if (rightArmAttacking == false && attackTimerRightArm == 0)
                {
                    quickAttacking = true;
                    rightArmAnim.SetBool("doQuickAttack", true);
                    currentQuickAttackTurn = QUICK_ATTACK_TURN.LEFT;
                    StartCoroutine(ResetQuickAttackTimer(rightQuickAttackTime / 2));
                }
            }
            else if (arms.leftWeaponEquipped && leftArmAttacking == false && attackTimerLeftArm == 0)
            {
                quickAttacking = true;
                leftArmAnim.SetBool("doQuickAttack", true);
                StartCoroutine(ResetQuickAttackTimer(leftQuickAttackTime));
            }
            else if (arms.rightWeaponEquipped && rightArmAttacking == false && attackTimerRightArm == 0)
            {
                quickAttacking = true;
                rightArmAnim.SetBool("doQuickAttack", true);
                StartCoroutine(ResetQuickAttackTimer(rightQuickAttackTime));
            }
        }
    }*/

    void Update_LeftArmAnims()
    {
        if (arms.leftShieldEquipped)
        {
            LeftShield_Block();
        }

        if (arms.leftWeaponEquipped && leftQuickAttacking == false)
        {
            Left1H_Attack();
        }
    }

    void Update_RightArmAnims()
    {
        if (arms.rightShieldEquipped)
        {
            RightShield_Block();
        }

        if (arms.rightWeaponEquipped && rightQuickAttacking == false)
        {
            Right1H_Attack();
        }
    }

    void LeftShield_Block()
    {
        if (Input.GetButton("Left Charge Attack") || Input.GetAxisRaw("Controller Left Charge Attack") == 1)
        {
            if (Input.GetAxisRaw("Controller Left Charge Attack") == 1)
                isUsingController = true;
            else
                isUsingController = false;

            isBlocking = true;
            leftArmAnim.SetBool("isBlocking", true);
        }

        if (Input.GetButtonUp("Left Charge Attack") || (isUsingController && Input.GetAxisRaw("Controller Left Charge Attack") == 0))
        {
            isBlocking = false;
            leftArmAnim.SetBool("isBlocking", false);
        }
    }

    void RightShield_Block()
    {
        if (Input.GetButton("Right Charge Attack") || Input.GetAxisRaw("Controller Right Charge Attack") == 1)
        {
            if (Input.GetAxisRaw("Controller Right Charge Attack") == 1)
                isUsingController = true;
            else
                isUsingController = false;

            isBlocking = true;
            rightArmAnim.SetBool("isBlocking", true);
        }

        if (Input.GetButtonUp("Right Charge Attack") || (isUsingController && Input.GetAxisRaw("Controller Right Charge Attack") == 0))
        {
            isBlocking = false;
            rightArmAnim.SetBool("isBlocking", false);
        }
    }

    void Left1H_Attack()
    {
        if ((Input.GetButton("Left Charge Attack") || Input.GetAxisRaw("Controller Left Charge Attack") == 1) && leftArmAttacking == false) // Left click || L2 (controller)
        {
            if (Input.GetAxisRaw("Controller Left Charge Attack") == 1)
                isUsingController = true;
            else
                isUsingController = false;

            attackTimerLeftArm += Time.smoothDeltaTime;

            leftArmAnim.SetBool("startAttack", true);
        }

        if (Input.GetButtonUp("Left Charge Attack") || (isUsingController && Input.GetAxisRaw("Controller Left Charge Attack") == 0)) // Left click || L2 (controller)
        {
            if (attackTimerLeftArm > minChargeAttackTime)
            {
                leftArmAttacking = true;
                leftArmAnim.SetBool("startAttack", false);
                leftArmAnim.SetBool("doAttack", true);
            }
            else
            {
                leftArmAnim.SetBool("startAttack", false);
                leftQuickAttacking = true;
                leftArmAnim.SetBool("doQuickAttack", true);
                StartCoroutine(ResetLeftQuickAttackTimer(leftQuickAttackTime));
            }

            StartCoroutine(ResetChargeAttackTimerLeftArm(leftChargeAttackTime));
        }
    }

    void Right1H_Attack()
    {
        if ((Input.GetButton("Right Charge Attack") || Input.GetAxis("Controller Right Charge Attack") == 1) && rightArmAttacking == false) // Right click || R2 (controller)
        {
            if (Input.GetAxisRaw("Controller Right Charge Attack") == 1)
                isUsingController = true;
            else
                isUsingController = false;

            attackTimerRightArm += Time.smoothDeltaTime;

            rightArmAnim.SetBool("startAttack", true);
        }

        if (Input.GetButtonUp("Right Charge Attack") || (isUsingController && Input.GetAxisRaw("Controller Right Charge Attack") == 0)) // Right click || R2 (controller)
        {
            if (attackTimerRightArm > minChargeAttackTime)
            {
                rightArmAttacking = true;
                rightArmAnim.SetBool("startAttack", false);
                rightArmAnim.SetBool("doAttack", true);
            }
            else
            {
                rightArmAnim.SetBool("startAttack", false);
                rightQuickAttacking = true;
                rightArmAnim.SetBool("doQuickAttack", true);
                StartCoroutine(ResetRightQuickAttackTimer(rightQuickAttackTime));
            }

            StartCoroutine(ResetChargeAttackTimerRightArm(rightChargeAttackTime));
        }
    }

    public void UpdateAnimClipTimes()
    {
        leftArmAnimClips = leftArmAnim.runtimeAnimatorController.animationClips;
        rightArmAnimClips = rightArmAnim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in leftArmAnimClips)
        {
            switch (clip.name)
            {
                case "Quick_Attack_1H_L":
                    leftQuickAttackTime = clip.length;
                    break;
                case "Attack_1H_Close_L":
                    leftChargeAttackTime = clip.length;
                    break;
                case "Shield_Bash_L":
                    shieldBashTime = clip.length;
                    break;
            }
        }

        foreach (AnimationClip clip in rightArmAnimClips)
        {
            switch (clip.name)
            {
                case "Quick_Attack_1H_R":
                    rightQuickAttackTime = clip.length;
                    break;
                case "Attack_1H_Close_R":
                    rightChargeAttackTime = clip.length;
                    break;
                case "Shield_Bash_R":
                    shieldBashTime = clip.length;
                    break;
            }
        }
    }

    IEnumerator ResetLeftQuickAttackTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        leftQuickAttacking = false;
        leftArmAnim.SetBool("doQuickAttack", false);
    }

    IEnumerator ResetRightQuickAttackTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        rightQuickAttacking = false;
        rightArmAnim.SetBool("doQuickAttack", false);
    }

    IEnumerator ResetChargeAttackTimerLeftArm(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attackTimerLeftArm = 0;
        leftArmAttacking = false;
        leftArmAnim.SetBool("doAttack", false);
    }

    IEnumerator ResetChargeAttackTimerRightArm(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attackTimerRightArm = 0;
        rightArmAttacking = false;
        rightArmAnim.SetBool("doAttack", false);
    }
}
