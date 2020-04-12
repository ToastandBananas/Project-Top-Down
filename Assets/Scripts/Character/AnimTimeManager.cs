using System.Collections;
using UnityEngine;

public class AnimTimeManager : MonoBehaviour
{
    public RuntimeAnimatorController leftArmAnimController, rightArmAnimController, legsAnimController, bodyAnimController;

    AnimationClip[] leftArmAnimClips;
    AnimationClip[] rightArmAnimClips;
    AnimationClip[] legAnimClips;
    AnimationClip[] bodyAnimClips;

    // Legs
    [HideInInspector] public float footstepTime;

    // Body
    [HideInInspector] public float bodyThrustLeftTime, bodyThrustRightTime;

    // Arms
    [HideInInspector] public float comboAttack1HLeft1Time, comboAttack1HLeft2Time, comboAttack1HLeft3Time;
    [HideInInspector] public float comboAttack1HRight1Time, comboAttack1HRight2Time, comboAttack1HRight3Time;
    [HideInInspector] public float leftQuickAttackTime, rightQuickAttackTime;
    [HideInInspector] public float leftChargeAttackTime, rightChargeAttackTime;
    [HideInInspector] public float leftHeavyAttackTime, rightHeavyAttackTime;
    [HideInInspector] public float deflectWeaponTime;
    [HideInInspector] public float drawBowStringTime, releaseArrowTime;
    [HideInInspector] public float shieldBashTime, shieldRecoilTime;
    [HideInInspector] public float staggerTime;

    void Start()
    {
        StartCoroutine(UpdateAnimClipTimes());
    }

    public IEnumerator UpdateAnimClipTimes()
    {
        yield return new WaitForSeconds(0.1f);
        leftArmAnimClips = leftArmAnimController.animationClips;
        rightArmAnimClips = rightArmAnimController.animationClips;
        legAnimClips = legsAnimController.animationClips;
        bodyAnimClips = bodyAnimController.animationClips;

        foreach (AnimationClip clip in leftArmAnimClips)
        {
            switch (clip.name)
            {
                case "Combo_Attack_1H_L_1":
                    comboAttack1HLeft1Time = clip.length;
                    break;
                case "Combo_Attack_1H_L_2":
                    comboAttack1HLeft2Time = clip.length;
                    break;
                case "Combo_Attack_1H_L_3":
                    comboAttack1HLeft3Time = clip.length;
                    break;
                case "Quick_Attack_1H_L":
                    leftQuickAttackTime = clip.length;
                    break;
                case "Heavy_Attack_1H_L":
                    leftHeavyAttackTime = clip.length;
                    break;
                case "Attack_1H_Close_L":
                    leftChargeAttackTime = clip.length;
                    break;
                case "Weapon_Deflection_L":
                    deflectWeaponTime = clip.length;
                    break;
                case "Shield_Bash_L":
                    shieldBashTime = clip.length;
                    break;
                case "Shield_Recoil_L":
                    shieldRecoilTime = clip.length;
                    break;
                case "Stagger_L":
                    staggerTime = clip.length;
                    break;
            }
        }

        foreach (AnimationClip clip in rightArmAnimClips)
        {
            switch (clip.name)
            {
                case "Combo_Attack_1H_R_1":
                    comboAttack1HRight1Time = clip.length;
                    break;
                case "Combo_Attack_1H_R_2":
                    comboAttack1HRight2Time = clip.length;
                    break;
                case "Combo_Attack_1H_R_3":
                    comboAttack1HRight3Time = clip.length;
                    break;
                case "Quick_Attack_1H_R":
                    rightQuickAttackTime = clip.length;
                    break;
                case "Heavy_Attack_1H_R":
                    rightHeavyAttackTime = clip.length;
                    break;
                case "Attack_1H_Close_R":
                    rightChargeAttackTime = clip.length;
                    break;
                case "Weapon_Deflection_R":
                    deflectWeaponTime = clip.length;
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
                case "Shield_Recoil_R":
                    shieldRecoilTime = clip.length;
                    break;
                case "Stagger_R":
                    staggerTime = clip.length;
                    break;
            }
        }

        foreach (AnimationClip clip in legAnimClips)
        {
            switch (clip.name)
            {
                case "Walk_Straight":
                    footstepTime = clip.length / 2;
                    break;
            }
        }

        foreach (AnimationClip clip in legAnimClips)
        {
            switch (clip.name)
            {
                case "Walk_Straight":
                    footstepTime = clip.length / 2;
                    break;
                case "Body_Thrust_Attack_L":
                    bodyThrustLeftTime = clip.length;
                    break;
                case "Body_Thrust_Attack_R":
                    bodyThrustRightTime = clip.length;
                    break;
            }
        }
    }
}
