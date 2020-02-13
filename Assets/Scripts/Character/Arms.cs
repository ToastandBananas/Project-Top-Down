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

    Transform leftArm, rightArm;
    Equipment leftWeapon, rightWeapon;
    Animator leftArmAnim, rightArmAnim;
    
    void Start()
    {
        leftArm = transform.Find("Left Arm");
        rightArm = transform.Find("Right Arm");

        leftArmAnim = leftArm.GetComponent<Animator>();
        rightArmAnim = rightArm.GetComponent<Animator>();

        SetLeftAnims();
        SetRightAnims();
    }

    public void SetLeftAnims()
    {
        if (leftArm.GetChild(0).GetChild(0).childCount > 0)
            leftWeapon  = leftArm.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<WeaponDamage>().equipment;
        if (rightArm.GetChild(0).GetChild(0).childCount > 0)
            rightWeapon = rightArm.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<WeaponDamage>().equipment;

        if (leftWeapon != null)
        {
            if (leftWeapon.generalClassification == GeneralClassification.SHIELD)
            {
                leftShieldEquipped = true;
                leftWeaponEquipped = false;
                leftArmAnim.SetBool("weaponEquipped", false);
                leftArmAnim.SetBool("shieldEquipped", true);
            }
            else if (leftWeapon.generalClassification == GeneralClassification.WEAPON_1H)
            {
                leftShieldEquipped = false;
                leftWeaponEquipped = true;
                leftArmAnim.SetBool("weaponEquipped", true);
                leftArmAnim.SetBool("shieldEquipped", false);
            }
        }
        else
        {
            leftShieldEquipped = false;
            leftWeaponEquipped = false;
            leftArmAnim.SetBool("weaponEquipped", false);
            leftArmAnim.SetBool("shieldEquipped", false);
        }
    }

    public void SetRightAnims()
    {
        if (rightWeapon != null)
        {
            if (rightWeapon.generalClassification == GeneralClassification.SHIELD)
            {
                rightShieldEquipped = true;
                rightWeaponEquipped = false;
                rightArmAnim.SetBool("weaponEquipped", false);
                rightArmAnim.SetBool("shieldEquipped", true);
            }
            else if (rightWeapon.generalClassification == GeneralClassification.WEAPON_1H)
            {
                rightShieldEquipped = false;
                rightWeaponEquipped = true;
                rightArmAnim.SetBool("weaponEquipped", true);
                rightArmAnim.SetBool("shieldEquipped", false);
            }
        }
        else
        {
            rightShieldEquipped = false;
            rightWeaponEquipped = false;
            rightArmAnim.SetBool("weaponEquipped", false);
            rightArmAnim.SetBool("shieldEquipped", false);
        }
    }
}
