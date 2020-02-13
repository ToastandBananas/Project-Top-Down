using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QUICK_ATTACK_TURN { LEFT, RIGHT }

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    QUICK_ATTACK_TURN currentQuickAttackTurn;

    PlayerMovement playerMovement;
    Transform headReset;
    
    Arms arms;
    Animator bodyAnim, rightArmAnim, leftArmAnim;
    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;
    
    [HideInInspector] public float attackTimerLeftArm = 0;
    [HideInInspector] public float attackTimerRightArm = 0;

    [HideInInspector] public float leftQuickAttackTime;
    [HideInInspector] public float rightQuickAttackTime;
    [HideInInspector] public float leftChargeAttackTime;
    [HideInInspector] public float rightChargeAttackTime;
    [HideInInspector] public float shieldBashTime;

    public float minChargeAttackTime = 0.3f;
    public bool isBlocking;
    public bool leftArmAttacking, rightArmAttacking;
    public bool leftQuickAttacking, rightQuickAttacking;

    LayerMask obstacleMask;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        headReset = transform.Find("Head Reset");
        bodyAnim = GetComponent<Animator>();
        arms = transform.Find("Arms").GetComponent<Arms>();
        rightArmAnim = arms.transform.Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = arms.transform.Find("Left Arm").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");

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
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed)
        {
            isBlocking = true;
            leftArmAnim.SetBool("isBlocking", true);
        }
        
        if (GameControls.gamePlayActions.playerLeftAttack.WasReleased)
        {
            isBlocking = false;
            leftArmAnim.SetBool("isBlocking", false);
        }
    }

    void RightShield_Block()
    {
        if (GameControls.gamePlayActions.playerRightAttack.IsPressed)
        {
            isBlocking = true;
            rightArmAnim.SetBool("isBlocking", true);
        }

        if (GameControls.gamePlayActions.playerRightAttack.WasReleased)
        {
            isBlocking = false;
            rightArmAnim.SetBool("isBlocking", false);
        }
    }

    void Left1H_Attack()
    {
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed && leftArmAttacking == false)
        {
            attackTimerLeftArm += Time.smoothDeltaTime;
            if (attackTimerLeftArm >= minChargeAttackTime)
                leftArmAnim.SetBool("startAttack", true);
        }

        if (GameControls.gamePlayActions.playerLeftAttack.WasReleased && attackTimerLeftArm > 0)
        {
            if (attackTimerLeftArm > minChargeAttackTime)
            {
                leftArmAttacking = true;
                leftArmAnim.SetBool("startAttack", false);
                leftArmAnim.SetBool("doAttack", true);
                bodyAnim.SetBool("powerAttackLeft", true);

                AttackDash(0.5f);
            }
            else
            {
                leftArmAnim.SetBool("startAttack", false);
                leftQuickAttacking = true;
                leftArmAnim.SetBool("doQuickAttack", true);
                attackTimerLeftArm = 0;
                StartCoroutine(ResetLeftQuickAttack(leftQuickAttackTime));
            }

            StartCoroutine(ResetChargeAttackTimerLeftArm(leftChargeAttackTime));
        }
        else if (GameControls.gamePlayActions.playerLeftAttack.IsPressed == false && attackTimerLeftArm == 0)
        {
            // Failsafe in case GetButtonUp isn't detected for whatever odd reason
            leftArmAttacking = false;
            leftArmAnim.SetBool("startAttack", false);
            leftArmAnim.SetBool("doAttack", false);
            leftQuickAttacking = false;
            leftArmAnim.SetBool("doQuickAttack", false);
            bodyAnim.SetBool("powerAttackLeft", false);
        }
    }

    void Right1H_Attack()
    {
        if (GameControls.gamePlayActions.playerRightAttack.IsPressed && rightArmAttacking == false)
        {
            attackTimerRightArm += Time.smoothDeltaTime;
            if (attackTimerRightArm >= minChargeAttackTime)
                rightArmAnim.SetBool("startAttack", true);
        }

        if (GameControls.gamePlayActions.playerRightAttack.WasReleased && attackTimerRightArm > 0)
        {
            if (attackTimerRightArm > minChargeAttackTime)
            {
                rightArmAttacking = true;
                rightArmAnim.SetBool("startAttack", false);
                rightArmAnim.SetBool("doAttack", true);
                bodyAnim.SetBool("powerAttackRight", true);

                AttackDash(0.5f);
            }
            else
            {
                rightArmAnim.SetBool("startAttack", false);
                rightQuickAttacking = true;
                rightArmAnim.SetBool("doQuickAttack", true);
                attackTimerRightArm = 0;
                StartCoroutine(ResetRightQuickAttack(rightQuickAttackTime));
            }

            StartCoroutine(ResetChargeAttackTimerRightArm(rightChargeAttackTime));
        }
        else if (GameControls.gamePlayActions.playerRightAttack.IsPressed == false && attackTimerRightArm == 0)
        {
            // Failsafe in case GetButtonUp isn't detected for whatever odd reason
            rightArmAttacking = false;
            rightArmAnim.SetBool("startAttack", false);
            rightArmAnim.SetBool("doAttack", false);
            rightQuickAttacking = false;
            rightArmAnim.SetBool("doQuickAttack", false);
            bodyAnim.SetBool("powerAttackRight", false);
        }
    }

    void AttackDash(float dashDistance)
    {
        Vector3 dir = (headReset.position - transform.position).normalized;
        float raycastDistance = Vector3.Distance(transform.position, transform.position + dir * dashDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);
        if (hit == false)
            StartCoroutine(playerMovement.SmoothMovement(transform.position + dir * dashDistance));
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

    IEnumerator ResetLeftQuickAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        leftQuickAttacking = false;
        leftArmAnim.SetBool("doQuickAttack", false);
    }

    IEnumerator ResetRightQuickAttack(float waitTime)
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
        bodyAnim.SetBool("powerAttackLeft", false);
    }

    IEnumerator ResetChargeAttackTimerRightArm(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attackTimerRightArm = 0;
        rightArmAttacking = false;
        rightArmAnim.SetBool("doAttack", false);
        bodyAnim.SetBool("powerAttackRight", false);
    }
}
