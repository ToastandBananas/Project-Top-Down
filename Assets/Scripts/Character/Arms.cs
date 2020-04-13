using System.Collections;
using UnityEngine;

/// <summary>
/// This class basically just detects what type of weapon(s) we have equipped and sets bools needed for idle and walk anims for both arms.
/// </summary>
public class Arms : MonoBehaviour
{
    [Header("Left Arm")]
    public bool leftWeaponEquipped;
    public bool leftShieldEquipped;

    [Header("Right Arm")]
    public bool rightWeaponEquipped;
    public bool rightShieldEquipped;

    [Header("Both Arms")]
    public bool twoHanderEquipped;
    public bool rangedWeaponEquipped;

    [Header("Attacking")]
    public bool isAttacking;
    public AttackType currentAttackType;
    public float currentAttackTime;
    public bool currentAttackShouldKnockback;

    public Transform leftArm, rightArm;
    public Transform leftEquippedWeapon, rightEquippedWeapon;
    public Equipment leftWeapon, rightWeapon;
    public Animator leftArmAnim, rightArmAnim, bodyAnim;

    NPCCombat npcCombat;
    NPCAttacks npcAttacks;
    
    void Awake()
    {
        StartCoroutine(SetArmAnims(0.05f));
        npcCombat = transform.parent.GetComponent<NPCCombat>();
        npcAttacks = transform.parent.GetComponent<NPCAttacks>();
    }

    public void GetWeaponTransforms()
    {
        if (leftArm.Find("Left Forearm").Find("Left Weapon").childCount > 0)
            leftEquippedWeapon = leftArm.Find("Left Forearm").Find("Left Weapon").GetChild(0).GetChild(0);

        if (rightArm.Find("Right Forearm").Find("Right Weapon").childCount > 0)
            rightEquippedWeapon = rightArm.Find("Right Forearm").Find("Right Weapon").GetChild(0).GetChild(0);
    }

    public void SetLeftAnims()
    {
        leftWeapon = null;

        leftArmAnim.SetBool("startAttack", false);
        leftArmAnim.SetBool("doDrawArrow", false);
        bodyAnim.SetBool("doDrawArrow", false);

        if (leftEquippedWeapon == null)
            GetWeaponTransforms();

        if (leftEquippedWeapon != null)
            leftWeapon = leftEquippedWeapon.GetComponent<ItemData>().equipment;

        if (leftWeapon != null)
        {
            if (leftEquippedWeapon.GetComponent<WeaponDamage>() != null)
                leftEquippedWeapon.GetComponent<WeaponDamage>().enabled = true;

            if (leftEquippedWeapon.GetComponent<BoxCollider2D>() != null)
                leftEquippedWeapon.GetComponent<BoxCollider2D>().enabled = true;

            if (leftWeapon.generalClassification == GeneralClassification.Shield)
            {
                leftShieldEquipped = true;
                leftWeaponEquipped = false;
                rangedWeaponEquipped = false;
                twoHanderEquipped = false;
                leftArmAnim.SetBool("weaponEquipped", false);
                leftArmAnim.SetBool("shieldEquipped", true);
                leftArmAnim.SetBool("rangedWeaponEquipped", false);
                rightArmAnim.SetBool("rangedWeaponEquipped", false);
            }
            else if (leftWeapon.generalClassification == GeneralClassification.Weapon1H)
            {
                leftShieldEquipped = false;
                leftWeaponEquipped = true;
                rangedWeaponEquipped = false;
                twoHanderEquipped = false;
                leftArmAnim.SetBool("weaponEquipped", true);
                leftArmAnim.SetBool("shieldEquipped", false);
                leftArmAnim.SetBool("isBlocking", false);
                leftArmAnim.SetBool("rangedWeaponEquipped", false);
                rightArmAnim.SetBool("rangedWeaponEquipped", false);
            }
            else if (leftWeapon.generalClassification == GeneralClassification.RangedWeapon)
            {
                leftShieldEquipped = false;
                leftWeaponEquipped = false;
                rangedWeaponEquipped = true;
                twoHanderEquipped = false;
                leftArmAnim.SetBool("weaponEquipped", false);
                leftArmAnim.SetBool("shieldEquipped", false);
                leftArmAnim.SetBool("isBlocking", false);
                leftArmAnim.SetBool("rangedWeaponEquipped", true);
                rightArmAnim.SetBool("rangedWeaponEquipped", true);
            }
        }
        else
        {
            leftShieldEquipped = false;
            leftWeaponEquipped = false;
            rangedWeaponEquipped = false;
            twoHanderEquipped = false;
            leftArmAnim.SetBool("weaponEquipped", false);
            leftArmAnim.SetBool("shieldEquipped", false);
            leftArmAnim.SetBool("isBlocking", false);
            leftArmAnim.SetBool("rangedWeaponEquipped", false);
            rightArmAnim.SetBool("rangedWeaponEquipped", false);
        }
    }

    public void SetRightAnims()
    {
        rightWeapon = null;
        
        rightArmAnim.SetBool("startAttack", false);
        rightArmAnim.SetBool("doDrawArrow", false);

        if (rightEquippedWeapon == null)
            GetWeaponTransforms();

        if (rightEquippedWeapon != null)
            rightWeapon = rightEquippedWeapon.GetComponent<ItemData>().equipment;
        
        if (rightWeapon != null)
        {
            if (rightEquippedWeapon.GetComponent<WeaponDamage>() != null)
                rightEquippedWeapon.GetComponent<WeaponDamage>().enabled = true;

            if (rightEquippedWeapon.GetComponent<BoxCollider2D>() != null)
                rightEquippedWeapon.GetComponent<BoxCollider2D>().enabled = true;

            if (rightWeapon.generalClassification == GeneralClassification.Shield)
            {
                rightShieldEquipped = true;
                rightWeaponEquipped = false;
                rightArmAnim.SetBool("weaponEquipped", false);
                rightArmAnim.SetBool("shieldEquipped", true);
            }
            else if (rightWeapon.generalClassification == GeneralClassification.Weapon1H)
            {
                rightShieldEquipped = false;
                rightWeaponEquipped = true;
                rightArmAnim.SetBool("weaponEquipped", true);
                rightArmAnim.SetBool("shieldEquipped", false);
                rightArmAnim.SetBool("isBlocking", false);
            }
        }
        else
        {
            rightShieldEquipped = false;
            rightWeaponEquipped = false;
            rightArmAnim.SetBool("weaponEquipped", false);
            rightArmAnim.SetBool("shieldEquipped", false);
            rightArmAnim.SetBool("isBlocking", false);
        }
    }

    public IEnumerator SetArmAnims(float waitTime = 0.1f)
    {
        yield return new WaitForSeconds(waitTime);

        leftArm = transform.Find("Left Arm");
        rightArm = transform.Find("Right Arm");

        SetLeftAnims();
        SetRightAnims();
    }

    public void RaiseShield()
    {
        if (leftShieldEquipped)
            leftArmAnim.SetBool("isBlocking", true);
        else if (rightShieldEquipped)
            rightArmAnim.SetBool("isBlocking", true);
    }

    public void LowerShield()
    {
        if (leftShieldEquipped)
            leftArmAnim.SetBool("isBlocking", false);
        if (rightShieldEquipped)
            rightArmAnim.SetBool("isBlocking", false);
    }

    public void DoAttack(Animator armAnim, string armAnimationName, string bodyAnimationName, AttackType attackType, float attackTime, bool shouldKnockback)
    {
        isAttacking = true;
        currentAttackType = attackType;
        currentAttackTime = attackTime;
        currentAttackShouldKnockback = shouldKnockback;
        armAnim.SetBool(armAnimationName, true);
        if (bodyAnimationName != null)
            bodyAnim.SetBool(bodyAnimationName, true);
    }

    public IEnumerator ResetAttack(Animator anim, string armAnimBoolName, string bodyAnimBoolName)
    {
        if (rightArmAnim.GetBool("doComboAttack1") == true || leftArmAnim.GetBool("doComboAttack1") == true)
            yield return new WaitForSeconds(currentAttackTime - 0.3f);
        else
            yield return new WaitForSeconds(currentAttackTime);

        isAttacking = false;
        if (npcCombat != null)
        {
            if (npcAttacks.leftComboNumber == 1 && npcAttacks.rightComboNumber == 1)
                npcCombat.determineMoveDirection = true;
            else
                npcCombat.needsCombatAction = true;
        }

        if (armAnimBoolName != null)
            anim.SetBool(armAnimBoolName, false);
        yield return new WaitForSeconds(0.2f);
        if (bodyAnimBoolName != null)
            bodyAnim.SetBool(bodyAnimBoolName, false);
    }
}
