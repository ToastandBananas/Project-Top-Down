using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator armsAnim;

    float timer = 0;
    float delayTime = 0.25f;

    bool isAttacking;

    void Start()
    {
        armsAnim = transform.Find("Arms").GetComponent<Animator>();
    }
    
    void Update()
    {
        if (Input.GetButton("Right Action") && isAttacking == false) // Right click || R2 (controller)
        {
            timer += Time.deltaTime;

            armsAnim.SetBool("startAttack", true);
        }

        if (Input.GetButtonUp("Right Action") && timer > 0.25f) // Right click || R2 (controller)
        {
            isAttacking = true;
            armsAnim.SetBool("startAttack", false);
            armsAnim.SetBool("doAttack", true);

            StartCoroutine(ResetTimer(delayTime));
        }
        else if (Input.GetButtonUp("Right Action"))
        {
            armsAnim.SetBool("startAttack", false);

            StartCoroutine(ResetTimer(delayTime));
        }

        Debug.Log(timer);
    }

    IEnumerator ResetTimer(float time)
    {
        yield return new WaitForSeconds(time);
        timer = 0;
        isAttacking = false;
        armsAnim.SetBool("doAttack", false);
    }
}
