using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Arms arms;
    Animator rightArmAnim, leftArmAnim;

    float timer = 0;
    float delayTime = 0.5f;

    public float minAttackTime = 0.3f;

    bool isAttacking, isBlocking;

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

        Debug.Log(timer);
    }

    void UpdateLeftArm()
    {
        if (arms.leftShieldEquipped) {
            if (Input.GetButton("Left Action"))
            {
                isBlocking = true;
                leftArmAnim.SetBool("isBlocking", true);
            }
            else if (Input.GetButtonUp("Left Action"))
            {
                isBlocking = false;
                leftArmAnim.SetBool("isBlocking", false);
            }
        }
    }

    void UpdateRightArm()
    {
        if (Input.GetButton("Right Action") && isAttacking == false) // Right click || R2 (controller)
        {
            timer += Time.deltaTime;

            rightArmAnim.SetBool("startAttack", true);
        }

        if (Input.GetButtonUp("Right Action") && timer > minAttackTime) // Right click || R2 (controller)
        {
            isAttacking = true;
            rightArmAnim.SetBool("startAttack", false);
            rightArmAnim.SetBool("doAttack", true);

            StartCoroutine(ResetTimer(delayTime));
        }
        else if (Input.GetButtonUp("Right Action"))
        {
            rightArmAnim.SetBool("startAttack", false);

            StartCoroutine(ResetTimer(delayTime));
        }
    }

    IEnumerator ResetTimer(float time)
    {
        yield return new WaitForSeconds(time);
        timer = 0;
        isAttacking = false;
        rightArmAnim.SetBool("doAttack", false);
    }
}
