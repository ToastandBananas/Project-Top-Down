using System.Collections;
using UnityEngine;

public class NPCAttacks : MonoBehaviour
{
    Arms arms;
    Animator bodyAnim;
    NPCCombat npcCombat;
    NPCMovement npcMovement;
    LockOn lockOnScript;
    EquipmentManager equipmentManager;
    BasicStats stats;
    Transform headReset;
    Transform looseItemsParent;

    LayerMask obstacleMask;

    bool shouldSetArrowPosition;
    float arrowSpeed = 32f;

    float heavyAttackStaminaMultiplier = 1.75f;

    [HideInInspector] public bool leftArmAttacking, rightArmAttacking;
    [HideInInspector] public bool leftQuickAttacking, rightQuickAttacking;

    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;

    [HideInInspector] public float leftQuickAttackTime, rightQuickAttackTime;
    [HideInInspector] public float leftChargeAttackTime, rightChargeAttackTime;
    [HideInInspector] public float leftHeavyAttackTime, rightHeavyAttackTime;
    [HideInInspector] public float deflectWeaponTime;
    [HideInInspector] public float drawBowStringTime, releaseArrowTime;
    [HideInInspector] public float shieldBashTime;

    public GameObject arrowPrefab;
    
    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        bodyAnim = GetComponent<Animator>();
        npcCombat = GetComponent<NPCCombat>();
        npcMovement = GetComponent<NPCMovement>();
        lockOnScript = GetComponent<LockOn>();
        equipmentManager = GetComponent<EquipmentManager>();
        stats = GetComponent<BasicStats>();
        headReset = transform.Find("Head Reset");
        looseItemsParent = GameObject.Find("Loose Items").transform;

        obstacleMask = LayerMask.GetMask("Walls", "Doors");

        StartCoroutine(UpdateAnimClipTimes());
    }

    void Update()
    {
        if (shouldSetArrowPosition)
            arms.leftEquippedWeapon.Find("Middle of String").position = equipmentManager.rightWeaponParent.position;
    }

    #region Quick Attack Functions
    public void QuickAttack()
    {
        if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // If weapon equipped in both arms, randomly choose one to attack with
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1) // Use left weapon
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse))
                {
                    leftQuickAttacking = true;
                    arms.leftArmAnim.SetBool("doQuickAttack", true); // Left quick attack
                    StartCoroutine(ResetLeftQuickAttack());
                }
                else
                    npcCombat.determineMoveDirection = true;
            }
            else // Use right weapon
            {
                if (stats.UseStamina(arms.rightWeapon.baseStaminaUse))
                {
                    rightQuickAttacking = true;
                    arms.rightArmAnim.SetBool("doQuickAttack", true); // Right quick attack
                    StartCoroutine(ResetRightQuickAttack());
                }
                else
                    npcCombat.determineMoveDirection = true;
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm, use left weapon
        {
            if (stats.UseStamina(arms.leftWeapon.baseStaminaUse))
            {
                leftQuickAttacking = true;
                arms.leftArmAnim.SetBool("doQuickAttack", true); // Left quick attack
                StartCoroutine(ResetLeftQuickAttack());
            }
            else
                npcCombat.determineMoveDirection = true;
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm, use right weapon
        {
            if (stats.UseStamina(arms.rightWeapon.baseStaminaUse))
            {
                rightQuickAttacking = true;
                arms.rightArmAnim.SetBool("doQuickAttack", true); // Right quick attack
                StartCoroutine(ResetRightQuickAttack());
            }
            else
                npcCombat.determineMoveDirection = true;
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

            if (randomNumber == 1) // Use left weapon
            {
                if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
                {
                    leftArmAttacking = true;
                    arms.leftArmAnim.SetBool("doHeavyAttack", true); // Left heavy attack
                    StartCoroutine(AttackDash(leftHeavyAttackTime * 0.7f, 0.5f));
                    StartCoroutine(ResetLeftHeavyAttack());
                }
                else
                    npcCombat.determineMoveDirection = true;
            }
            else // Use right weapon
            {
                if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
                {
                    rightArmAttacking = true;
                    arms.rightArmAnim.SetBool("doHeavyAttack", true); // Right heavy attack
                    StartCoroutine(AttackDash(rightHeavyAttackTime * 0.7f, 0.5f));
                    StartCoroutine(ResetRightHeavyAttack());
                }
                else
                    npcCombat.determineMoveDirection = true;
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm, use left weapon
        {
            if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
            {
                leftArmAttacking = true;
                arms.leftArmAnim.SetBool("doHeavyAttack", true); // Left heavy attack
                StartCoroutine(AttackDash(leftHeavyAttackTime * 0.7f, 0.5f));
                StartCoroutine(ResetLeftHeavyAttack());
            }
            else
                npcCombat.determineMoveDirection = true;
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm, use right weapon
        {
            if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
            {
                rightArmAttacking = true;
                arms.rightArmAnim.SetBool("doHeavyAttack", true); // Right heavy attack
                StartCoroutine(AttackDash(rightHeavyAttackTime * 0.7f, 0.5f));
                StartCoroutine(ResetRightHeavyAttack());
            }
            else
                npcCombat.determineMoveDirection = true;
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

    #region Ranged Attack Function
    public IEnumerator RangedAttack()
    {
        if (arms.leftEquippedWeapon == null)
            arms.GetWeaponTransforms();

        GameObject arrow = Instantiate(arrowPrefab, arms.leftEquippedWeapon.Find("Middle of String").position, Quaternion.identity, arms.leftEquippedWeapon.Find("Middle of String"));
        arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);

        arms.rightArmAnim.SetBool("doReleaseArrow", false);
        arms.leftArmAnim.SetBool("doDrawArrow", true);
        arms.rightArmAnim.SetBool("doDrawArrow", true);
        arms.bodyAnim.SetBool("doDrawArrow", true);

        yield return new WaitForSeconds(drawBowStringTime / 2);
        shouldSetArrowPosition = true;
        yield return new WaitForSeconds(drawBowStringTime / 2);

        // Randomize wait to shoot time
        yield return new WaitForSeconds(Random.Range(0f, 2.5f));
            
        // Fire the arrow
        arms.rightArmAnim.SetBool("doReleaseArrow", true);
        arms.rightArmAnim.SetBool("doDrawArrow", false);

        // Reset the bow string to its default position
        arms.leftEquippedWeapon.Find("Middle of String").localPosition = arms.leftEquippedWeapon.GetComponent<DrawBowString>().middleOfStringOriginalPosition;
        shouldSetArrowPosition = false;
        npcCombat.determineMoveDirection = true;

        arrow.transform.SetParent(looseItemsParent);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.enabled = true;
        arrowScript.bowShotFrom = arms.leftEquippedWeapon.GetComponent<ItemData>();

        arrow.GetComponent<BoxCollider2D>().enabled = true;
        arrow.GetComponent<SpriteRenderer>().sortingOrder = 10;

        Rigidbody2D arrowRigidBody = arrow.GetComponent<Rigidbody2D>();
        arrowRigidBody.bodyType = RigidbodyType2D.Dynamic;
        arrowRigidBody.AddForce(-arrow.transform.up * arrowSpeed, ForceMode2D.Impulse);

        float shotDistance = 18f;
        if (lockOnScript.isLockedOn)
            shotDistance = Vector2.Distance(arrow.transform.position, lockOnScript.lockOnTarget.position) + 1;

        while (arrowScript.arrowShouldStop == false)
        {
            if (Vector2.Distance(arrow.transform.position, transform.position) > shotDistance)
                arrowScript.StopArrow();

            yield return null;
        }
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
                case "Weapon_Deflection_L":
                    deflectWeaponTime = clip.length;
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
                case "Draw_Arrow_R":
                    drawBowStringTime = clip.length;
                    break;
                case "Weapon_Deflection_R":
                    deflectWeaponTime = clip.length;
                    break;
                case "Release_Arrow_R":
                    releaseArrowTime = clip.length;
                    break;
                case "Shield_Bash_R":
                    shieldBashTime = clip.length;
                    break;
            }
        }
    }
}
