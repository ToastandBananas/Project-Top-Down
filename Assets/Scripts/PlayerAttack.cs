using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Arms arms;
    Animator rightArmAnim, leftArmAnim;

    float attackTimerLeftArm = 0;
    float attackTimerRightArm = 0;
    float delayTime = 0.5f;

    public float minAttackTime = 0.3f;
    public bool isBlocking;

    bool leftArmAttacking, rightArmAttacking;
    bool isUsingController;

    void Start()
    {
        arms = transform.Find("Arms").GetComponent<Arms>();
        rightArmAnim = arms.transform.Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = arms.transform.Find("Left Arm").GetComponent<Animator>();
    }
    
    void Update()
    {
        UpdateLeftArm();
        UpdateRightArm();

        Debug.Log(Input.GetAxis("Controller Right Action"));
        // Debug.Log(attackTimerLeftArm);
        Debug.Log(attackTimerRightArm);
    }

    void UpdateLeftArm()
    {
        if (arms.leftShieldEquipped) {
            if (Input.GetButton("Left Action") || Input.GetAxisRaw("Controller Left Action") == 1)
            {
                if (Input.GetAxisRaw("Controller Left Action") == 1)
                    isUsingController = true;
                else
                    isUsingController = false;

                isBlocking = true;
                leftArmAnim.SetBool("isBlocking", true);
            }

            if (Input.GetButtonUp("Left Action") || (isUsingController && Input.GetAxisRaw("Controller Left Action") == 0))
            {
                isBlocking = false;
                leftArmAnim.SetBool("isBlocking", false);
            }
        }

        if (arms.leftWeaponEquipped)
        {
            if ((Input.GetButton("Left Action") || Input.GetAxisRaw("Controller Left Action") == 1) && leftArmAttacking == false) // Left click || L2 (controller)
            {
                if (Input.GetAxisRaw("Controller Left Action") == 1)
                    isUsingController = true;
                else
                    isUsingController = false;

                attackTimerLeftArm += Time.smoothDeltaTime;

                leftArmAnim.SetBool("startAttack", true);
            }

            if (Input.GetButtonUp("Left Action") || (isUsingController && Input.GetAxisRaw("Controller Left Action") == 0)) // Left click || L2 (controller)
            {
                if (attackTimerLeftArm > minAttackTime)
                {
                    leftArmAttacking = true;
                    leftArmAnim.SetBool("startAttack", false);
                    leftArmAnim.SetBool("doAttack", true);
                }
                else
                    leftArmAnim.SetBool("startAttack", false);

                StartCoroutine(ResetAttackTimerLeftArm(delayTime));
            }
        }
    }

    void UpdateRightArm()
    {
        if (arms.rightShieldEquipped)
        {
            if (Input.GetButton("Right Action") || Input.GetAxisRaw("Controller Right Action") == 1)
            {
                if (Input.GetAxisRaw("Controller Right Action") == 1)
                    isUsingController = true;
                else
                    isUsingController = false;

                isBlocking = true;
                rightArmAnim.SetBool("isBlocking", true);
            }

            if (Input.GetButtonUp("Right Action") || (isUsingController && Input.GetAxisRaw("Controller Right Action") == 0))
            {
                isBlocking = false;
                rightArmAnim.SetBool("isBlocking", false);
            }
        }

        if (arms.rightWeaponEquipped)
        {
            if ((Input.GetButton("Right Action") || Input.GetAxis("Controller Right Action") == 1) && rightArmAttacking == false) // Right click || R2 (controller)
            {
                if (Input.GetAxisRaw("Controller Right Action") == 1)
                    isUsingController = true;
                else
                    isUsingController = false;

                attackTimerRightArm += Time.smoothDeltaTime;

                rightArmAnim.SetBool("startAttack", true);
            }

            if (Input.GetButtonUp("Right Action") || (isUsingController && Input.GetAxisRaw("Controller Right Action") == 0)) // Right click || R2 (controller)
            {
                if (attackTimerRightArm > minAttackTime)
                {
                    rightArmAttacking = true;
                    rightArmAnim.SetBool("startAttack", false);
                    rightArmAnim.SetBool("doAttack", true);
                }
                else
                    rightArmAnim.SetBool("startAttack", false);
                
                StartCoroutine(ResetAttackTimerRightArm(delayTime));
            }
        }
    }

    IEnumerator ResetAttackTimerLeftArm(float time)
    {
        yield return new WaitForSeconds(time);
        attackTimerLeftArm = 0;
        leftArmAttacking = false;
        leftArmAnim.SetBool("doAttack", false);
    }

    IEnumerator ResetAttackTimerRightArm(float time)
    {
        yield return new WaitForSeconds(time);
        attackTimerRightArm = 0;
        rightArmAttacking = false;
        rightArmAnim.SetBool("doAttack", false);
    }
}
