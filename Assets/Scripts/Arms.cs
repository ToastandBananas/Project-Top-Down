using UnityEngine;

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
    WeaponStats leftWeapon, rightWeapon;
    Animator leftArmAnim, rightArmAnim;
    
    void Start()
    {
        leftArm = transform.Find("Left Arm");
        rightArm = transform.Find("Right Arm");

        leftArmAnim = leftArm.GetComponent<Animator>();
        rightArmAnim = rightArm.GetComponent<Animator>();

        SetAnims();
    }

    public void SetAnims()
    {
        leftWeapon  = leftArm.GetChild(0).GetChild(0).GetChild(0).GetComponent<WeaponStats>();
        rightWeapon = rightArm.GetChild(0).GetChild(0).GetChild(0).GetComponent<WeaponStats>();

        if (leftWeapon != null)
        {
            if (leftWeapon.generalClassification == GENERAL_CLASSIFICATION.SHIELD)
            {
                leftShieldEquipped = true;
                leftWeaponEquipped = false;
                leftArmAnim.SetBool("shieldEquipped", true);
            }
            else if (leftWeapon.generalClassification == GENERAL_CLASSIFICATION.WEAPON)
            {
                leftShieldEquipped = false;
                leftWeaponEquipped = true;
                leftArmAnim.SetBool("weaponEquipped", true);
            }
        }

        if (rightWeapon != null)
        {
            if (rightWeapon.generalClassification == GENERAL_CLASSIFICATION.SHIELD)
            {
                rightShieldEquipped = true;
                rightWeaponEquipped = false;
                rightArmAnim.SetBool("shieldEquipped", true);
            }
            else if (rightWeapon.generalClassification == GENERAL_CLASSIFICATION.WEAPON)
            {
                rightShieldEquipped = false;
                rightWeaponEquipped = true;
                rightArmAnim.SetBool("weaponEquipped", true);
            }
        }
    }
}
