using System.Collections;
using UnityEngine;

public class NPCAttacks : MonoBehaviour
{
    AudioManager audioManager;
    AnimTimeManager animTimeManager;
    Arms arms;
    Animator bodyAnim;
    BasicStats basicStats;
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
    float minArrowRotation = 6f;
    float maxArrowRotation = 12f;

    [HideInInspector] public int leftComboNumber = 1;
    [HideInInspector] public int rightComboNumber = 1;
    public bool comboAttackOnCooldown;
    bool comboTimerStarted;
    float comboTimer;
    float maxComboTimer = 1f;

    float heavyAttackStaminaMultiplier = 1.75f;

    [HideInInspector] public bool isBlocking;
    [HideInInspector] public bool leftArmHeavyAttacking, rightArmHeavyAttacking;
    [HideInInspector] public bool leftQuickAttacking, rightQuickAttacking;

    public GameObject arrowPrefab;
    
    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        bodyAnim = GetComponent<Animator>();
        basicStats = GetComponent<BasicStats>();
        npcCombat = GetComponent<NPCCombat>();
        npcMovement = GetComponent<NPCMovement>();
        lockOnScript = GetComponent<LockOn>();
        audioManager = AudioManager.instance;
        animTimeManager = GameManager.instance.GetComponent<AnimTimeManager>();
        equipmentManager = GetComponent<EquipmentManager>();
        stats = GetComponent<BasicStats>();
        headReset = transform.Find("Head Reset");
        looseItemsParent = GameObject.Find("Loose Items").transform;

        obstacleMask = LayerMask.GetMask("Walls", "OpenDoors", "ClosedDoors");
    }

    void Update()
    {
        if (shouldSetArrowPosition)
            arms.leftEquippedWeapon.Find("Middle of String").position = equipmentManager.rightWeaponParent.position;
    }

    #region Combo Attack Functions
    public void ComboAttack()
    {
        StartCoroutine(StartComboTimer());

        if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // If weapons are equipped in both arms
        {
            int randomNum = Random.Range(1, 3); // Randomly choose left or right arm to attack with
            
            if (randomNum == 1) // Use left weapon
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                    DoComboAttack(arms.leftArmAnim);
                else
                {
                    npcCombat.determineMoveDirection = true;
                    ResetCombo();
                }
            }
            else // Use right weapon
            {
                if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                    DoComboAttack(arms.rightArmAnim);
                else
                {
                    npcCombat.determineMoveDirection = true;
                    ResetCombo();
                }
            }
        }
        else if (arms.leftWeaponEquipped)
        {
            if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                DoComboAttack(arms.leftArmAnim);
            else
            {
                npcCombat.determineMoveDirection = true;
                ResetCombo();
            }
        }
        else if (arms.rightWeaponEquipped)
        {
            if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                DoComboAttack(arms.rightArmAnim);
            else
            {
                npcCombat.determineMoveDirection = true;
                ResetCombo();
            }
        }
    }

    void DoComboAttack(Animator armAnim)
    {
        float attackTime;
        float comboNumber;
        if (armAnim == arms.leftArmAnim) comboNumber = leftComboNumber; else comboNumber = rightComboNumber;

        if (comboNumber == 1)
        {
            if (armAnim == arms.leftArmAnim) attackTime = animTimeManager.comboAttack1HLeft1Time; else attackTime = animTimeManager.comboAttack1HRight1Time;

            audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
            arms.DoAttack(armAnim, "doComboAttack1", null, AttackType.Slash, attackTime, false);
            StartCoroutine(AttackDash(0.1f, 0.25f));
            StartCoroutine(arms.ResetAttack(armAnim, null, null));
        }
        else if (comboNumber == 2)
        {
            if (armAnim == arms.leftArmAnim) attackTime = animTimeManager.comboAttack1HLeft2Time; else attackTime = animTimeManager.comboAttack1HRight2Time;

            audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
            arms.DoAttack(armAnim, "doComboAttack2", null, AttackType.Slash, attackTime, false);
            StartCoroutine(AttackDash(0.1f, 0.25f));
            StartCoroutine(arms.ResetAttack(armAnim, null, null));
        }
        else if (comboNumber == 3)
        {
            string bodyThrustAnimationName = null;

            // Reset the opposite arm's combo anim bools
            if (armAnim == arms.leftArmAnim)
            {
                attackTime = animTimeManager.comboAttack1HLeft3Time;
                bodyThrustAnimationName = "doThrustAttackLeft";
                arms.rightArmAnim.SetBool("doComboAttack1", false);
                arms.rightArmAnim.SetBool("doComboAttack2", false);
                arms.rightArmAnim.SetBool("doComboAttack3", false);
            }
            else
            {
                attackTime = animTimeManager.comboAttack1HRight3Time;
                bodyThrustAnimationName = "doThrustAttackRight";
                arms.leftArmAnim.SetBool("doComboAttack1", false);
                arms.leftArmAnim.SetBool("doComboAttack2", false);
                arms.leftArmAnim.SetBool("doComboAttack3", false);
            }

            audioManager.PlayRandomSound(audioManager.swordStabSounds, transform.position);
            arms.DoAttack(armAnim, "doComboAttack3", bodyThrustAnimationName, AttackType.Thrust, attackTime, true);
            StartCoroutine(AttackDash(attackTime / 2, 0.5f));
            StartCoroutine(arms.ResetAttack(armAnim, null, bodyThrustAnimationName));

            StartCoroutine(ComboCooldown());
        }
        
        // Set the current combo index and reset the combo timer
        if (armAnim == arms.leftArmAnim)
            if (leftComboNumber == 3) leftComboNumber = 1; else leftComboNumber++;
        else
            if (rightComboNumber == 3) rightComboNumber = 1; else rightComboNumber++;
        comboTimer = 0;
    }

    IEnumerator StartComboTimer()
    {
        if (comboTimerStarted == false)
        {
            comboTimerStarted = true;
            comboTimer = 0;

            while (comboTimerStarted)
            {
                comboTimer += Time.smoothDeltaTime;
                if (comboTimer >= maxComboTimer)
                {
                    ResetCombo();
                    break;
                }
                yield return null;
            }
        }
    }

    IEnumerator ComboCooldown()
    {
        comboAttackOnCooldown = true;
        yield return new WaitForSeconds(0.25f + arms.currentAttackTime);
        comboAttackOnCooldown = false;
        ResetCombo();
    }

    void ResetCombo()
    {
        npcCombat.determineMoveDirection = true;
        comboTimerStarted = false;
        leftComboNumber = 1;
        rightComboNumber = 1;
        arms.leftArmAnim.SetBool("doComboAttack1", false);
        arms.leftArmAnim.SetBool("doComboAttack2", false);
        arms.leftArmAnim.SetBool("doComboAttack3", false);
        arms.rightArmAnim.SetBool("doComboAttack1", false);
        arms.rightArmAnim.SetBool("doComboAttack2", false);
        arms.rightArmAnim.SetBool("doComboAttack3", false);
    }
    #endregion

    #region Quick Attack Functions
    public void QuickAttack()
    {
        if (arms.leftWeaponEquipped && arms.rightWeaponEquipped) // If weapon equipped in both arms, randomly choose one to attack with
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1) // Use left weapon
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                    DoQuickAttack(arms.leftArmAnim, animTimeManager.leftQuickAttackTime);
                else
                    npcCombat.determineMoveDirection = true;
            }
            else // Use right weapon
            {
                if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                    DoQuickAttack(arms.rightArmAnim, animTimeManager.rightQuickAttackTime);
                else
                    npcCombat.determineMoveDirection = true;
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm, use left weapon
        {
            if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                DoQuickAttack(arms.leftArmAnim, animTimeManager.leftQuickAttackTime);
            else
                npcCombat.determineMoveDirection = true;
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm, use right weapon
        {
            if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                DoQuickAttack(arms.rightArmAnim, animTimeManager.rightQuickAttackTime);
            else
                npcCombat.determineMoveDirection = true;
        }
    }

    void DoQuickAttack(Animator armAnim, float attackTime)
    {
        audioManager.PlayRandomSound(audioManager.swordStabSounds, transform.position);
        arms.DoAttack(armAnim, "doQuickAttack", null, AttackType.Thrust, attackTime, false);
        StartCoroutine(AttackDash(0.1f, 0.25f));
        StartCoroutine(arms.ResetAttack(armAnim, "doQuickAttack", null));
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
                if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                    DoHeavyAttack(arms.leftArmAnim, "doPowerAttackLeft", animTimeManager.leftHeavyAttackTime);
                else
                    npcCombat.determineMoveDirection = true;
            }
            else // Use right weapon
            {
                if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                    DoHeavyAttack(arms.rightArmAnim, "doPowerAttackRight", animTimeManager.rightHeavyAttackTime);
                else
                    npcCombat.determineMoveDirection = true;
            }
        }
        else if (arms.leftWeaponEquipped && arms.rightWeaponEquipped == false) // If weapon equipped only in left arm, use left weapon
        {
            if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                DoHeavyAttack(arms.leftArmAnim, "doPowerAttackLeft", animTimeManager.leftHeavyAttackTime);
            else
                npcCombat.determineMoveDirection = true;
        }
        else if (arms.rightWeaponEquipped && arms.leftWeaponEquipped == false) // If weapon equipped only in right arm, use right weapon
        {
            if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                DoHeavyAttack(arms.rightArmAnim, "doPowerAttackRight", animTimeManager.rightHeavyAttackTime);
            else
                npcCombat.determineMoveDirection = true;
        }
    }

    void DoHeavyAttack(Animator armAnim, string bodyAnimName, float attackTime)
    {
        audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
        arms.DoAttack(armAnim, "doHeavyAttack", bodyAnimName, AttackType.Slash, attackTime, true);
        StartCoroutine(AttackDash(attackTime * 0.7f, 0.5f));
        StartCoroutine(arms.ResetAttack(armAnim, "doHeavyAttack", bodyAnimName));
    }
    #endregion

    #region Ranged Attack Function
    public IEnumerator RangedAttack()
    {
        if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse), false))
        {
            if (arms.leftEquippedWeapon == null)
                arms.GetWeaponTransforms();

            // Spawn an arrow
            GameObject arrow = Instantiate(arrowPrefab, arms.leftEquippedWeapon.Find("Middle of String").position, Quaternion.identity, arms.leftEquippedWeapon.Find("Middle of String"));
            arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
            SpriteRenderer arrowSR = arrow.GetComponent<SpriteRenderer>();
            arrow.GetComponent<ItemData>().hasBeenRandomized = true;
            arrowSR.sprite = arrow.GetComponent<ItemData>().item.possibleSprites[0];

            // Draw the bow string
            audioManager.PlayRandomSound(audioManager.bowDrawSounds, transform.position);
            arms.rightArmAnim.SetBool("doReleaseArrow", false);
            arms.leftArmAnim.SetBool("doDrawArrow", true);
            arms.rightArmAnim.SetBool("doDrawArrow", true);
            arms.bodyAnim.SetBool("doDrawArrow", true);

            yield return new WaitForSeconds(animTimeManager.drawBowStringTime / 2);
            shouldSetArrowPosition = true;
            yield return new WaitForSeconds(animTimeManager.drawBowStringTime / 2);

            // Randomize wait to shoot time
            yield return new WaitForSeconds(Random.Range(0f, 2.5f));

            // Fire the arrow
            audioManager.PlayRandomSound(audioManager.bowReleaseSounds, transform.position);
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

            arrowSR.sortingLayerID = SortingLayer.GetLayerValueFromName("Default");
            arrowSR.sortingOrder = 1;

            DetermineShotAccuracy(arrow.transform);

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
        else // Not enough stamina
        {
            arms.rightArmAnim.SetBool("doReleaseArrow", false);
            arms.leftArmAnim.SetBool("doDrawArrow", false);
            arms.rightArmAnim.SetBool("doDrawArrow", false);
            arms.bodyAnim.SetBool("doDrawArrow", false);
            npcCombat.determineMoveDirection = true;
        }
    }

    void DetermineShotAccuracy(Transform arrow)
    {
        bool accurateShot = true;
        int randomNum = Random.Range(1, 101);
        if (randomNum > basicStats.rangedSkill)
            accurateShot = false;

        float rotationAmount;
        if (accurateShot == false)
        {
            randomNum = Random.Range(1, 3);
            if (randomNum == 1) rotationAmount = Random.Range(-maxArrowRotation, -minArrowRotation); else rotationAmount = Random.Range(minArrowRotation, maxArrowRotation);
            arrow.Rotate(new Vector3(0, 0, rotationAmount), Space.Self);
        }
        else
        {
            randomNum = Random.Range(1, 3);
            if (randomNum == 1) rotationAmount = Random.Range(-2f, 0f); else rotationAmount = Random.Range(0f, 2f);
            arrow.Rotate(new Vector3(0, 0, rotationAmount), Space.Self);
        }
    }
    #endregion

    IEnumerator AttackDash(float waitTime, float dashDistance)
    {
        yield return new WaitForSeconds(waitTime);

        npcMovement.isAttackDashing = true;

        if (arms.leftArmAnim.GetBool("doHeavyAttack") == true)
            bodyAnim.SetBool("doPowerAttackLeft", true);
        else if (arms.rightArmAnim.GetBool("doHeavyAttack") == true)
            bodyAnim.SetBool("doPowerAttackRight", true);

        Vector3 dir = (headReset.position - transform.position).normalized;
        float raycastDistance = Vector3.Distance(transform.position, transform.position + dir * dashDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);
        if (hit == false)
            StartCoroutine(npcMovement.SmoothMovement(transform.position + dir * dashDistance));
    }
}
