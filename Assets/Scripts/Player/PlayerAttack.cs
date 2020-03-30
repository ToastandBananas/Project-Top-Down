using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;

    public GameObject arrowPrefab;

    GameManager gm;
    PlayerMovement playerMovement;
    BasicStats stats;
    EquipmentManager equipmentManager;
    Transform headReset;
    Transform looseItemsParent;
    
    Arms arms;
    Animator bodyAnim, rightArmAnim, leftArmAnim;
    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;
    
    [HideInInspector] public float attackTimerLeftArm = 0;
    [HideInInspector] public float attackTimerRightArm = 0;

    [HideInInspector] public float leftQuickAttackTime;
    [HideInInspector] public float rightQuickAttackTime;
    [HideInInspector] public float leftChargeAttackTime;
    [HideInInspector] public float leftHeavyAttackTime;
    [HideInInspector] public float rightHeavyAttackTime;
    [HideInInspector] public float rightChargeAttackTime;
    [HideInInspector] public float drawBowStringTime;
    [HideInInspector] public float releaseArrowTime;
    [HideInInspector] public float shieldBashTime;

    float heavyAttackStaminaMultiplier = 1.75f;

    public float minChargeAttackTime = 0.3f;
    public bool isBlocking;
    public bool leftArmAttacking, rightArmAttacking;
    public bool leftQuickAttacking, rightQuickAttacking;
    public bool firingArrow, bowStringFullyDrawn, arrowSpawned;
    public bool shouldStartDrawingBowString = true;

    bool bowStringNeedsReset;

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
        gm = GameManager.instance;
        playerMovement = PlayerMovement.instance;
        stats = GetComponent<BasicStats>();
        equipmentManager = GetComponent<EquipmentManager>();
        headReset = transform.Find("Head Reset");
        looseItemsParent = GameObject.Find("Loose Items").transform;
        bodyAnim = GetComponent<Animator>();
        arms = transform.Find("Arms").GetComponent<Arms>();
        rightArmAnim = arms.transform.Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = arms.transform.Find("Left Arm").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");

        StartCoroutine(UpdateAnimClipTimes());
    }
    
    void Update()
    {
        if (gm.menuOpen == false)
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
                if (stats.UseStamina(Mathf.RoundToInt(arms.leftWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
                {
                    leftArmAttacking = true;
                    leftArmAnim.SetBool("doAttack", true);
                    bodyAnim.SetBool("powerAttackLeft", true);

                    AttackDash(0.5f);
                }

                leftArmAnim.SetBool("startAttack", false);
            }
            else
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse))
                {
                    leftQuickAttacking = true;
                    leftArmAnim.SetBool("doQuickAttack", true);
                    StartCoroutine(ResetLeftQuickAttack(leftQuickAttackTime));
                }

                leftArmAnim.SetBool("startAttack", false);
                attackTimerLeftArm = 0;
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
                if (stats.UseStamina(Mathf.RoundToInt(arms.rightWeapon.baseStaminaUse * heavyAttackStaminaMultiplier)))
                {
                    rightArmAttacking = true;
                    rightArmAnim.SetBool("doAttack", true);
                    bodyAnim.SetBool("powerAttackRight", true);

                    AttackDash(0.5f);
                }

                rightArmAnim.SetBool("startAttack", false);
            }
            else
            {
                if (stats.UseStamina(arms.leftWeapon.baseStaminaUse))
                {
                    rightQuickAttacking = true;
                    rightArmAnim.SetBool("doQuickAttack", true);
                    StartCoroutine(ResetRightQuickAttack(rightQuickAttackTime));
                }

                rightArmAnim.SetBool("startAttack", false);
                attackTimerRightArm = 0;
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

    void DrawArrow()
    {
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed)
        {
            if (GameControls.gamePlayActions.playerRightAttack.WasReleased == false && firingArrow == false)
            {
                if (shouldStartDrawingBowString)
                {
                    // Spawn an arrow
                    if (arrowSpawned == false)
                    {
                        GameObject arrow = Instantiate(arrowPrefab, arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).Find("Middle of String").position, Quaternion.identity, arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).Find("Middle of String"));
                        arrowSpawned = true;
                    }
                    // Draw the bow string
                    bodyAnim.SetBool("doDrawArrow", true);
                    rightArmAnim.SetBool("doReleaseArrow", false);
                    leftArmAnim.SetBool("doDrawArrow", true);
                    rightArmAnim.SetBool("doDrawArrow", true);

                    StartCoroutine(DrawBowString());
                }
                else // If we already are drawing the bow string
                    arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).Find("Middle of String").position = equipmentManager.rightWeaponParent.position;
            }
            else if (GameControls.gamePlayActions.playerRightAttack.WasReleased && firingArrow == false && bowStringFullyDrawn)
            {
                // Fire the arrow
                firingArrow = true;
                bowStringFullyDrawn = false;
                rightArmAnim.SetBool("doReleaseArrow", true);
                rightArmAnim.SetBool("doDrawArrow", false);
                arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).Find("Middle of String").localPosition = arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).GetComponent<DrawBowString>().middleOfStringOriginalPosition;

                StartCoroutine(ResetFireArrow(releaseArrowTime));
            }
        }
        else
        {
            if (bowStringNeedsReset)
            {
                arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).Find("Middle of String").localPosition = arms.leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0).GetComponent<DrawBowString>().middleOfStringOriginalPosition;
                bowStringNeedsReset = false;
            }

            shouldStartDrawingBowString = true;
            firingArrow = false;
            bowStringFullyDrawn = false;
            bodyAnim.SetBool("doDrawArrow", false);
            leftArmAnim.SetBool("doDrawArrow", false);
            rightArmAnim.SetBool("doDrawArrow", false);
            rightArmAnim.SetBool("doReleaseArrow", false);
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

    public IEnumerator UpdateAnimClipTimes()
    {
        yield return new WaitForSeconds(0.1f);
        leftArmAnimClips = leftArmAnim.runtimeAnimatorController.animationClips;
        rightArmAnimClips = rightArmAnim.runtimeAnimatorController.animationClips;

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
                case "Draw_Arrow_R":
                    drawBowStringTime = clip.length;
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

    IEnumerator DrawBowString()
    {
        yield return new WaitForSeconds(drawBowStringTime / 2);
        bowStringNeedsReset = true;
        shouldStartDrawingBowString = false;

        yield return new WaitForSeconds(drawBowStringTime / 2);
        bowStringFullyDrawn = true;
    }

    IEnumerator ResetFireArrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shouldStartDrawingBowString = true;
        arrowSpawned = false;
        rightArmAnim.SetBool("doReleaseArrow", false);
        firingArrow = false;
        bowStringFullyDrawn = false;
        if (GameControls.gamePlayActions.playerLeftAttack.IsPressed)
            rightArmAnim.SetBool("doDrawArrow", true);
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
