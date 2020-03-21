using System.Collections;
using UnityEngine;

public class NPCAttacks : MonoBehaviour
{
    Arms arms;
    Animator bodyAnim;
    NPCCombat npcCombat;
    NPCMovement npcMovement;
    Transform headReset;

    LayerMask obstacleMask;

    [HideInInspector] public bool leftArmAttacking, rightArmAttacking;
    [HideInInspector] public bool leftQuickAttacking, rightQuickAttacking;

    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;

    [HideInInspector] public float leftQuickAttackTime, rightQuickAttackTime;
    [HideInInspector] public float leftChargeAttackTime, rightChargeAttackTime;
    [HideInInspector] public float leftHeavyAttackTime, rightHeavyAttackTime;
    [HideInInspector] public float shieldBashTime;
    
    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        bodyAnim = GetComponent<Animator>();
        npcCombat = GetComponent<NPCCombat>();
        npcMovement = GetComponent<NPCMovement>();
        headReset = transform.Find("Head Reset");

        obstacleMask = LayerMask.GetMask("Walls", "Doors");

        StartCoroutine(UpdateAnimClipTimes());
    }

    #region Quick Attack Functions
    public void QuickAttack()
    {
        if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // If weapon equipped in both arms, randomly choose one to attack with
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1)
            {
                leftQuickAttacking = true;
                arms.leftArmAnim.SetBool("doQuickAttack", true); // Left quick attack
                StartCoroutine(ResetLeftQuickAttack());
            }
            else
            {
                rightQuickAttacking = true;
                arms.rightArmAnim.SetBool("doQuickAttack", true); // Right quick attack
                StartCoroutine(ResetRightQuickAttack());
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm
        {
            leftQuickAttacking = true;
            arms.leftArmAnim.SetBool("doQuickAttack", true); // Left quick attack
            StartCoroutine(ResetLeftQuickAttack());
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm
        {
            rightQuickAttacking = true;
            arms.rightArmAnim.SetBool("doQuickAttack", true); // Right quick attack
            StartCoroutine(ResetRightQuickAttack());
        }
    }

    IEnumerator ResetLeftQuickAttack()
    {
        yield return new WaitForSeconds(leftQuickAttackTime);
        leftQuickAttacking = false;
        arms.leftArmAnim.SetBool("doQuickAttack", false);
        npcCombat.determineMoveDirection = true;
    }

    IEnumerator ResetRightQuickAttack()
    {
        yield return new WaitForSeconds(rightQuickAttackTime);
        rightQuickAttacking = false;
        arms.rightArmAnim.SetBool("doQuickAttack", false);
        npcCombat.determineMoveDirection = true;
    }
    #endregion

    #region Heavy Attack Functions
    public void HeavyAttack()
    {
        if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // If weapon equipped in both arms, randomly choose one to attack with
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1)
            {
                leftArmAttacking = true;
                arms.leftArmAnim.SetBool("doHeavyAttack", true); // Left heavy attack
                StartCoroutine(AttackDash(leftHeavyAttackTime * 0.7f, 0.5f));
                StartCoroutine(ResetLeftHeavyAttack());
            }
            else
            {
                rightArmAttacking = true;
                arms.rightArmAnim.SetBool("doHeavyAttack", true); // Right heavy attack
                StartCoroutine(AttackDash(rightHeavyAttackTime * 0.7f, 0.5f));
                StartCoroutine(ResetRightHeavyAttack());
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm
        {
            leftArmAttacking = true;
            arms.leftArmAnim.SetBool("doHeavyAttack", true); // Left heavy attack
            StartCoroutine(AttackDash(leftHeavyAttackTime * 0.7f, 0.5f));
            StartCoroutine(ResetLeftHeavyAttack());
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm
        {
            rightArmAttacking = true;
            arms.rightArmAnim.SetBool("doHeavyAttack", true); // Right heavy attack
            StartCoroutine(AttackDash(rightHeavyAttackTime * 0.7f, 0.5f));
            StartCoroutine(ResetRightHeavyAttack());
        }
    }

    IEnumerator ResetLeftHeavyAttack()
    {
        yield return new WaitForSeconds(leftHeavyAttackTime);
        leftArmAttacking = false;
        arms.leftArmAnim.SetBool("doHeavyAttack", false);
        bodyAnim.SetBool("powerAttackLeft", false);
        npcCombat.determineMoveDirection = true;
    }

    IEnumerator ResetRightHeavyAttack()
    {
        yield return new WaitForSeconds(rightHeavyAttackTime);
        rightArmAttacking = false;
        arms.rightArmAnim.SetBool("doHeavyAttack", false);
        bodyAnim.SetBool("powerAttackRight", false);
        npcCombat.determineMoveDirection = true;
    }
    #endregion

    IEnumerator AttackDash(float waitTime, float dashDistance)
    {
        yield return new WaitForSeconds(waitTime);

        if (arms.leftArmAnim.GetBool("doHeavyAttack") == true)
            bodyAnim.SetBool("powerAttackLeft", true);
        else if (arms.rightArmAnim.GetBool("doHeavyAttack") == true)
            bodyAnim.SetBool("powerAttackRight", true);

        Vector3 dir = (headReset.position - transform.position).normalized;
        float raycastDistance = Vector3.Distance(transform.position, transform.position + dir * dashDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);
        if (hit == false)
            StartCoroutine(npcMovement.SmoothMovement(transform.position + dir * dashDistance));
    }

    public IEnumerator UpdateAnimClipTimes()
    {
        yield return new WaitForSeconds(0.1f);
        leftArmAnimClips = arms.leftArmAnim.runtimeAnimatorController.animationClips;
        rightArmAnimClips = arms.rightArmAnim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in leftArmAnimClips)
        {
            switch (clip.name)
            {
                case "Quick_Attack_1H_L":
                    leftQuickAttackTime = clip.length;
                    break;
                case "Heavy_Attack_1H_L":
                    leftHeavyAttackTime = clip.length;
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
                case "Heavy_Attack_1H_R":
                    rightHeavyAttackTime = clip.length;
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
}
