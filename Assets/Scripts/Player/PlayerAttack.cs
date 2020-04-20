using System.Collections;
using UnityEngine;

public enum AttackType { Slash, Thrust, Blunt };

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    public GameObject arrowPrefab;

    AudioManager audioManager;
    GameManager gm;
    PlayerMovement playerMovement;
    LockOn lockOnScript;
    BasicStats stats;
    AnimTimeManager animTimeManager;
    BasicStats basicStats;
    EquipmentManager equipmentManager;
    Arms arms;
    Animator bodyAnim;
    Transform headReset;
    Transform looseItemsParent;

    [HideInInspector] public float leftAttackTimer = 0;
    [HideInInspector] public float rightAttackTimer = 0;

    float heavyAttackStaminaMultiplier = 1.75f;

    public float minChargeAttackTime = 0.3f;
    public bool isBlocking, isAttackDashing;
    public bool leftArmHeavyAttacking, rightArmHeavyAttacking;
    public bool leftQuickAttacking, rightQuickAttacking;
    public bool firingArrow, bowStringFullyDrawn, arrowSpawned;
    public bool shouldStartDrawingBowString = true;

    bool bowStringNeedsReset;
    float arrowSpeed = 32f;
    float minArrowRotation = 6f;
    float maxArrowRotation = 12f;
    
    bool comboTimerStarted;
    bool comboAttackOnCooldown;
    int leftComboNumber = 1;
    int rightComboNumber = 1;
    float comboTimer;
    float maxComboTimer = 1f;

    Vector3 dir;
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
        audioManager = AudioManager.instance;
        gm = GameManager.instance;
        playerMovement = PlayerMovement.instance;
        lockOnScript = GetComponent<LockOn>();
        stats = GetComponent<BasicStats>();
        animTimeManager = gm.GetComponent<AnimTimeManager>();
        basicStats = GetComponent<BasicStats>();
        equipmentManager = GetComponent<EquipmentManager>();
        headReset = transform.Find("Head Reset");
        looseItemsParent = GameObject.Find("Loose Items").transform;
        arms = transform.Find("Arms").GetComponent<Arms>();
        bodyAnim = GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "OpenDoors", "ClosedDoors");
    }
    
    void Update()
    {
        if (gm.menuOpen == false && playerMovement.isStaggered == false && GameControls.gamePlayActions.leftCtrl.IsPressed == false)
        {
            Update_LeftArmAnims();
            Update_RightArmAnims();
            Update_BothArmAnims();
        }
    }

    void Update_LeftArmAnims()
    {
        if (arms.leftShieldEquipped)
        {
            LeftShield_Block();
        }
        else if (arms.leftWeaponEquipped && leftQuickAttacking == false)
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
        else if (arms.rightWeaponEquipped && rightQuickAttacking == false)
        {
            Right1H_Attack();
        }
    }

    void Update_BothArmAnims()
    {
        if (arms.rangedWeaponEquipped)
        {
            DrawArrow();
        }
    }

    void LeftShield_Block()
    {
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed)
        {
            isBlocking = true;
            arms.leftArmAnim.SetBool("isBlocking", true);
        }
        
        if (GameControls.gamePlayActions.playerLeftAttack.WasReleased)
        {
            isBlocking = false;
            arms.leftArmAnim.SetBool("isBlocking", false);
        }
    }

    void RightShield_Block()
    {
        if (GameControls.gamePlayActions.playerRightAttack.IsPressed)
        {
            isBlocking = true;
            arms.rightArmAnim.SetBool("isBlocking", true);
        }

        if (GameControls.gamePlayActions.playerRightAttack.WasReleased)
        {
            isBlocking = false;
            arms.rightArmAnim.SetBool("isBlocking", false);
        }
    }

    void Left1H_Attack()
    {
        if (GameControls.gamePlayActions.playerRightAttack.IsPressed == false 
            || (GameControls.gamePlayActions.playerRightAttack.IsPressed && (arms.rightArmAnim.GetBool("isBlocking") == true || rightAttackTimer > 0)))
        {
            if (GameControls.gamePlayActions.playerLeftAttack.IsPressed && leftAttackTimer <= minChargeAttackTime && arms.isAttacking == false && comboAttackOnCooldown == false)
            {
                leftAttackTimer += Time.smoothDeltaTime;
                if (leftAttackTimer >= minChargeAttackTime && leftComboNumber == 1) // Start charge attack
                    arms.leftArmAnim.SetBool("startAttack", true);
            }

            if (GameControls.gamePlayActions.playerLeftAttack.WasReleased && leftAttackTimer > 0 && arms.isAttacking == false && comboAttackOnCooldown == false)
            {
                if (leftAttackTimer > minChargeAttackTime && arms.leftArmAnim.GetBool("doComboAttack1") == false) // Do power attack
                {
                    if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                    {
                        audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
                        arms.DoAttack(arms.leftArmAnim, "doAttack", "doPowerAttackLeft", AttackType.Slash, animTimeManager.leftChargeAttackTime, true);
                        StartCoroutine(arms.ResetAttack(arms.leftArmAnim, "doAttack", "doPowerAttackLeft"));

                        AttackDash(0.5f);
                    }

                    arms.leftArmAnim.SetBool("startAttack", false);
                    StartCoroutine(ResetChargeAttackTimer(animTimeManager.leftChargeAttackTime, true));
                }
                else // Do combo attack
                {
                    if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                    {
                        StartCoroutine(StartComboTimer());

                        if (comboTimer < maxComboTimer)
                        {
                            if (leftComboNumber == 1)
                            {
                                audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
                                arms.DoAttack(arms.leftArmAnim, "doComboAttack1", null, AttackType.Slash, animTimeManager.comboAttack1HLeft1Time, false);

                                StartCoroutine(DelayedAttackDash(0.25f, animTimeManager.comboAttack1HLeft1Time / 2));
                                StartCoroutine(arms.ResetAttack(arms.leftArmAnim, null, null));
                            }
                            else if (leftComboNumber == 2)
                            {
                                audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);
                                arms.DoAttack(arms.leftArmAnim, "doComboAttack2", null, AttackType.Slash, animTimeManager.comboAttack1HLeft2Time, false);

                                StartCoroutine(DelayedAttackDash(0.25f, animTimeManager.comboAttack1HLeft2Time / 2));
                                StartCoroutine(arms.ResetAttack(arms.leftArmAnim, null, null));
                            }
                            else if (leftComboNumber == 3)
                            {
                                // Reset the opposite arm's combo anim bools
                                arms.rightArmAnim.SetBool("doComboAttack1", false);
                                arms.rightArmAnim.SetBool("doComboAttack2", false);
                                arms.rightArmAnim.SetBool("doComboAttack3", false);

                                audioManager.PlayRandomSound(audioManager.swordStabSounds, transform.position);
                                arms.DoAttack(arms.leftArmAnim, "doComboAttack3", "doThrustAttackLeft", AttackType.Thrust, animTimeManager.comboAttack1HLeft3Time, true);

                                StartCoroutine(DelayedAttackDash(0.5f, animTimeManager.comboAttack1HLeft3Time / 2));
                                StartCoroutine(arms.ResetAttack(arms.leftArmAnim, null, "doThrustAttackLeft"));
                                StartCoroutine(ComboCooldown());
                            }

                            if (leftComboNumber == 3) leftComboNumber = 1; else leftComboNumber++;
                            comboTimer = 0;
                        }
                    }

                    arms.leftArmAnim.SetBool("startAttack", false);
                    leftAttackTimer = 0;
                }
            }
        }
    }

    void Right1H_Attack()
    {
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed == false 
            || (GameControls.gamePlayActions.playerLeftAttack.IsPressed && (arms.leftArmAnim.GetBool("isBlocking") == true || leftAttackTimer > 0)))
        {
            if (GameControls.gamePlayActions.playerRightAttack.IsPressed && rightAttackTimer <= minChargeAttackTime && arms.isAttacking == false && comboAttackOnCooldown == false)
            {
                rightAttackTimer += Time.smoothDeltaTime;
                if (rightAttackTimer >= minChargeAttackTime && rightComboNumber == 1) // Start charge attack
                    arms.rightArmAnim.SetBool("startAttack", true);
            }

            if (GameControls.gamePlayActions.playerRightAttack.WasReleased && rightAttackTimer > 0 && arms.isAttacking == false && comboAttackOnCooldown == false)
            {
                if (rightAttackTimer > minChargeAttackTime && arms.rightArmAnim.GetBool("doComboAttack1") == false) // Do power attack
                {
                    // If we have enough stamina, do the attack
                    if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                    {
                        // Play attack sound
                        audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);

                        // Do the attack (set bools and arm's current attack info)
                        arms.DoAttack(arms.rightArmAnim, "doAttack", "doPowerAttackRight", AttackType.Slash, animTimeManager.rightChargeAttackTime, true);

                        // Reset attack bools at the appropriate time
                        StartCoroutine(arms.ResetAttack(arms.rightArmAnim, "doAttack", "doPowerAttackRight"));

                        AttackDash(0.5f); // Dash towards target
                    }

                    // Turn off startAttack and reset the charge attack timer
                    arms.rightArmAnim.SetBool("startAttack", false);
                    StartCoroutine(ResetChargeAttackTimer(animTimeManager.rightChargeAttackTime, false));
                }
                else // Do combo attack
                {
                    // If we have enough stamina, do the attack
                    if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                    {
                        // Start up/reset combo timer (time in which we can continue the combo with another attack)
                        StartCoroutine(StartComboTimer());

                        if (comboTimer < maxComboTimer)
                        {
                            if (rightComboNumber == 1)
                            {
                                // Play attack sound
                                audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);

                                // Do the attack (set bools and arm's current attack info)
                                arms.DoAttack(arms.rightArmAnim, "doComboAttack1", null, AttackType.Slash, animTimeManager.comboAttack1HRight1Time, false);

                                // Dash towards target
                                StartCoroutine(DelayedAttackDash(0.25f, animTimeManager.comboAttack1HRight1Time / 2));

                                // Reset attack bools at the appropriate time
                                StartCoroutine(arms.ResetAttack(arms.rightArmAnim, null, null));
                            }
                            else if (rightComboNumber == 2)
                            {
                                // Play attack sound
                                audioManager.PlayRandomSound(audioManager.swordSlashSounds, transform.position);

                                // Do the attack (set bools and arm's current attack info)
                                arms.DoAttack(arms.rightArmAnim, "doComboAttack2", null, AttackType.Slash, animTimeManager.comboAttack1HRight2Time, false);

                                // Dash towards target
                                StartCoroutine(DelayedAttackDash(0.25f, animTimeManager.comboAttack1HRight2Time / 2));

                                // Reset attack bools at the appropriate time
                                StartCoroutine(arms.ResetAttack(arms.rightArmAnim, null, null));
                            }
                            else if (rightComboNumber == 3)
                            {
                                // Reset the opposite arm's combo anim bools
                                arms.leftArmAnim.SetBool("doComboAttack1", false);
                                arms.leftArmAnim.SetBool("doComboAttack2", false);
                                arms.leftArmAnim.SetBool("doComboAttack3", false);

                                // Play attack sound
                                audioManager.PlayRandomSound(audioManager.swordStabSounds, transform.position);

                                // Do the attack (set bools and arm's current attack info)
                                arms.DoAttack(arms.rightArmAnim, "doComboAttack3", "doThrustAttackRight", AttackType.Thrust, animTimeManager.comboAttack1HRight3Time, true);

                                // Dash towards target
                                StartCoroutine(DelayedAttackDash(0.5f, animTimeManager.comboAttack1HRight3Time / 2));

                                // Reset attack bools at the appropriate time
                                StartCoroutine(arms.ResetAttack(arms.rightArmAnim, null, "doThrustAttackRight"));

                                // Set a cooldown before we can combo attack again
                                StartCoroutine(ComboCooldown());
                            }

                            // Set the current combo index and reset the combo timer
                            if (rightComboNumber == 3) rightComboNumber = 1; else rightComboNumber++;
                            comboTimer = 0;
                        }
                    }
                    
                    // Reset startAttack and attackTimer
                    arms.rightArmAnim.SetBool("startAttack", false);
                    rightAttackTimer = 0;
                }
            }
        }
    }

    void DrawArrow()
    {
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed 
            && equipmentManager.currentEquipment[(int)EquipmentSlot.Quiver] != null && equipmentManager.currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount > 0)
        {
            if (lockOnScript.isLockedOn == false && gm.isUsingController == false)
                playerMovement.LookAtMouse();

            if (GameControls.gamePlayActions.playerRightAttack.WasReleased == false && firingArrow == false)
            {
                if (shouldStartDrawingBowString)
                {
                    // Spawn an arrow
                    if (arrowSpawned == false)
                    {
                        if (arms.leftEquippedWeapon == null)
                            arms.GetWeaponTransforms();

                        GameObject arrow = Instantiate(arrowPrefab, arms.leftEquippedWeapon.Find("Middle of String").position, Quaternion.identity, arms.leftEquippedWeapon.Find("Middle of String"));
                        arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        arrowSpawned = true;
                        arrow.GetComponent<ItemData>().hasBeenRandomized = true;
                        arrow.GetComponent<SpriteRenderer>().sprite = arrow.GetComponent<ItemData>().item.possibleSprites[0];
                    }

                    audioManager.PlayRandomSound(audioManager.bowDrawSounds, transform.position);

                    // Draw the bow string
                    arms.bodyAnim.SetBool("doDrawArrow", true);
                    arms.rightArmAnim.SetBool("doReleaseArrow", false);
                    arms.leftArmAnim.SetBool("doDrawArrow", true);
                    arms.rightArmAnim.SetBool("doDrawArrow", true);

                    StartCoroutine(DrawBowString());
                }
                else // If we already are drawing the bow string
                    arms.leftEquippedWeapon.Find("Middle of String").position = equipmentManager.rightWeaponParent.position;
            }
            else if (GameControls.gamePlayActions.playerRightAttack.WasReleased && firingArrow == false && bowStringFullyDrawn)
            {
                StartCoroutine(ShootArrow());

                // Fire the arrow
                firingArrow = true;
                bowStringFullyDrawn = false;
                arms.rightArmAnim.SetBool("doReleaseArrow", true);
                arms.rightArmAnim.SetBool("doDrawArrow", false);
                arms.leftEquippedWeapon.Find("Middle of String").localPosition = arms.leftEquippedWeapon.GetComponent<DrawBowString>().middleOfStringOriginalPosition;

                StartCoroutine(ResetFireArrow(animTimeManager.releaseArrowTime));
            }
        }
        else
        {
            if (bowStringNeedsReset)
            {
                if (arms.leftEquippedWeapon == null)
                    arms.GetWeaponTransforms();

                arms.leftEquippedWeapon.Find("Middle of String").localPosition = arms.leftEquippedWeapon.GetComponent<DrawBowString>().middleOfStringOriginalPosition;
                bowStringNeedsReset = false;
            }

            shouldStartDrawingBowString = true;
            firingArrow = false;
            bowStringFullyDrawn = false;
            arms.bodyAnim.SetBool("doDrawArrow", false);
            arms.leftArmAnim.SetBool("doDrawArrow", false);
            arms.rightArmAnim.SetBool("doDrawArrow", false);
            arms.rightArmAnim.SetBool("doReleaseArrow", false);
        }
    }

    void AttackDash(float dashDistance)
    {
        playerMovement.isAttackDashing = true;
        dir = (headReset.position - transform.position).normalized;
        float raycastDistance = Vector3.Distance(transform.position, transform.position + dir * dashDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);
        if (hit == false)
            StartCoroutine(playerMovement.SmoothMovement(transform.position + dir * dashDistance));
    }

    IEnumerator DelayedAttackDash(float dashDistance, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        AttackDash(dashDistance);
    }

    IEnumerator DrawBowString()
    {
        yield return new WaitForSeconds(animTimeManager.drawBowStringTime / 2);
        bowStringNeedsReset = true;
        shouldStartDrawingBowString = false;

        yield return new WaitForSeconds(animTimeManager.drawBowStringTime / 2);
        bowStringFullyDrawn = true;
    }

    IEnumerator ShootArrow()
    {
        audioManager.PlayRandomSound(audioManager.bowReleaseSounds, transform.position);

        equipmentManager.currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount--;
        equipmentManager.quiverSlot.quiverText.text = equipmentManager.currentEquipment[(int)EquipmentSlot.Quiver].currentAmmoCount.ToString();

        Transform arrow = arms.leftEquippedWeapon.Find("Middle of String").GetChild(0);
        arrow.SetParent(looseItemsParent);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.enabled = true;
        arrowScript.bowShotFrom = arms.leftEquippedWeapon.GetComponent<ItemData>();

        arrow.GetComponent<BoxCollider2D>().enabled = true;

        SpriteRenderer arrowSR = arrow.GetComponent<SpriteRenderer>();
        arrowSR.sortingLayerID = SortingLayer.GetLayerValueFromName("Default");
        arrowSR.sortingOrder = 1;

        DeterminShotAccuracy(arrow);

        Rigidbody2D arrowRigidBody = arrow.GetComponent<Rigidbody2D>();
        arrowRigidBody.bodyType = RigidbodyType2D.Dynamic;
        arrowRigidBody.AddForce(-arrow.up * arrowSpeed, ForceMode2D.Impulse);

        float shotDistance = 18f;
        if (lockOnScript.isLockedOn)
            shotDistance = Vector2.Distance(arrow.position, lockOnScript.lockOnTarget.position) + 1;
        
        while (arrowScript.arrowShouldStop == false)
        {
            if (Vector2.Distance(arrow.position, transform.position) > shotDistance)
                arrowScript.StopArrow();

            yield return null;
        }
    }

    void DeterminShotAccuracy(Transform arrow)
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
            arrow.transform.Rotate(new Vector3(0, 0, rotationAmount), Space.Self);
        }
    }

    IEnumerator ResetFireArrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shouldStartDrawingBowString = true;
        arrowSpawned = false;
        arms.rightArmAnim.SetBool("doReleaseArrow", false);
        firingArrow = false;
        bowStringFullyDrawn = false;
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed)
            arms.rightArmAnim.SetBool("doDrawArrow", true);
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
        leftAttackTimer = 0;
        rightAttackTimer = 0;
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

    IEnumerator ResetChargeAttackTimer(float waitTime, bool isLeftAttackTimer)
    {
        yield return new WaitForSeconds(waitTime);
        if (isLeftAttackTimer) leftAttackTimer = 0; else rightAttackTimer = 0;
    }

    public IEnumerator SmoothMovement(Transform objectToMove, Vector3 targetPos)
    {
        while (Vector2.Distance(objectToMove.position, targetPos) > 0.1f)
        {
            objectToMove.position = Vector2.MoveTowards(objectToMove.position, targetPos, 4 * Time.deltaTime);
            yield return null; // Pause for one frame
        }
    }
}
