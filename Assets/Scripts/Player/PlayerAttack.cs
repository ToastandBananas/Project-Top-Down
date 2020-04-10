using System.Collections;
using UnityEngine;

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
    EquipmentManager equipmentManager;
    Arms arms;
    Transform headReset;
    Transform looseItemsParent;
    
    [HideInInspector] public float attackTimerLeftArm = 0;
    [HideInInspector] public float attackTimerRightArm = 0;

    float heavyAttackStaminaMultiplier = 1.75f;

    public float minChargeAttackTime = 0.3f;
    public bool isBlocking;
    public bool leftArmHeavyAttacking, rightArmHeavyAttacking;
    public bool leftQuickAttacking, rightQuickAttacking;
    public bool firingArrow, bowStringFullyDrawn, arrowSpawned;
    public bool shouldStartDrawingBowString = true;

    bool bowStringNeedsReset;
    float arrowSpeed = 32f;

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
        equipmentManager = GetComponent<EquipmentManager>();
        headReset = transform.Find("Head Reset");
        looseItemsParent = GameObject.Find("Loose Items").transform;
        arms = transform.Find("Arms").GetComponent<Arms>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");
    }
    
    void Update()
    {
        if (gm.menuOpen == false && playerMovement.isStaggered == false)
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
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed && leftArmHeavyAttacking == false)
        {
            attackTimerLeftArm += Time.smoothDeltaTime;
            if (attackTimerLeftArm >= minChargeAttackTime)
                arms.leftArmAnim.SetBool("startAttack", true);
        }

        if (GameControls.gamePlayActions.playerLeftAttack.WasReleased && attackTimerLeftArm > 0)
        {
            if (attackTimerLeftArm > minChargeAttackTime)
            {
                if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                {
                    audioManager.PlayRandomSound(audioManager.swordSlashSounds);
                    leftArmHeavyAttacking = true;
                    arms.leftArmAnim.SetBool("doAttack", true);
                    arms.bodyAnim.SetBool("powerAttackLeft", true);

                    AttackDash(0.5f);
                }

                arms.leftArmAnim.SetBool("startAttack", false);
            }
            else
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse, false))
                {
                    audioManager.PlayRandomSound(audioManager.swordStabSounds);
                    leftQuickAttacking = true;
                    arms.leftArmAnim.SetBool("doQuickAttack", true);
                    StartCoroutine(ResetLeftQuickAttack(animTimeManager.leftQuickAttackTime));
                }

                arms.leftArmAnim.SetBool("startAttack", false);
                attackTimerLeftArm = 0;
            }

            StartCoroutine(ResetChargeAttackTimerLeftArm(animTimeManager.leftChargeAttackTime));
        }
        else if (GameControls.gamePlayActions.playerLeftAttack.IsPressed == false && attackTimerLeftArm == 0)
        {
            // Failsafe in case GetButtonUp isn't detected for whatever odd reason
            leftArmHeavyAttacking = false;
            arms.leftArmAnim.SetBool("startAttack", false);
            arms.leftArmAnim.SetBool("doAttack", false);
            leftQuickAttacking = false;
            arms.leftArmAnim.SetBool("doQuickAttack", false);
            arms.bodyAnim.SetBool("powerAttackLeft", false);
        }
    }

    void Right1H_Attack()
    {
        if (GameControls.gamePlayActions.playerRightAttack.IsPressed && rightArmHeavyAttacking == false)
        {
            attackTimerRightArm += Time.smoothDeltaTime;
            if (attackTimerRightArm >= minChargeAttackTime)
                arms.rightArmAnim.SetBool("startAttack", true);
        }

        if (GameControls.gamePlayActions.playerRightAttack.WasReleased && attackTimerRightArm > 0)
        {
            if (attackTimerRightArm > minChargeAttackTime)
            {
                if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier), false))
                {
                    audioManager.PlayRandomSound(audioManager.swordSlashSounds);
                    rightArmHeavyAttacking = true;
                    arms.rightArmAnim.SetBool("doAttack", true);
                    arms.bodyAnim.SetBool("powerAttackRight", true);

                    AttackDash(0.5f);
                }

                arms.rightArmAnim.SetBool("startAttack", false);
            }
            else
            {
                if (stats.UseStamina(arms.rightWeapon.baseStaminaUse, false))
                {
                    audioManager.PlayRandomSound(audioManager.swordStabSounds);
                    rightQuickAttacking = true;
                    arms.rightArmAnim.SetBool("doQuickAttack", true);
                    StartCoroutine(ResetRightQuickAttack(animTimeManager.rightQuickAttackTime));
                }

                arms.rightArmAnim.SetBool("startAttack", false);
                attackTimerRightArm = 0;
            }

            StartCoroutine(ResetChargeAttackTimerRightArm(animTimeManager.rightChargeAttackTime));
        }
        else if (GameControls.gamePlayActions.playerRightAttack.IsPressed == false && attackTimerRightArm == 0)
        {
            // Failsafe in case GetButtonUp isn't detected for whatever odd reason
            rightArmHeavyAttacking = false;
            arms.rightArmAnim.SetBool("startAttack", false);
            arms.rightArmAnim.SetBool("doAttack", false);
            rightQuickAttacking = false;
            arms.rightArmAnim.SetBool("doQuickAttack", false);
            arms.bodyAnim.SetBool("powerAttackRight", false);
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
                    }

                    audioManager.PlayRandomSound(audioManager.bowDrawSounds);

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
        dir = (headReset.position - transform.position).normalized;
        float raycastDistance = Vector3.Distance(transform.position, transform.position + dir * dashDistance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);
        if (hit == false)
            StartCoroutine(playerMovement.SmoothMovement(transform.position + dir * dashDistance));
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
        audioManager.PlayRandomSound(audioManager.bowReleaseSounds);

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
        arrowSR.sortingOrder = 10;

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

    IEnumerator ResetLeftQuickAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        leftQuickAttacking = false;
        arms.leftArmAnim.SetBool("doQuickAttack", false);
    }

    IEnumerator ResetRightQuickAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        rightQuickAttacking = false;
        arms.rightArmAnim.SetBool("doQuickAttack", false);
    }

    IEnumerator ResetChargeAttackTimerLeftArm(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attackTimerLeftArm = 0;
        leftArmHeavyAttacking = false;
        arms.leftArmAnim.SetBool("doAttack", false);
        arms.bodyAnim.SetBool("powerAttackLeft", false);
    }

    IEnumerator ResetChargeAttackTimerRightArm(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attackTimerRightArm = 0;
        rightArmHeavyAttacking = false;
        arms.rightArmAnim.SetBool("doAttack", false);
        arms.bodyAnim.SetBool("powerAttackRight", false);
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
