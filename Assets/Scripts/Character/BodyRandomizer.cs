using UnityEngine;

public class BodyRandomizer : MonoBehaviour
{
    public SpriteRenderer head, hair, helmet;
    public Transform leftUpperArm, leftForearm, rightUpperArm, rightForearm;
    public Transform leftThigh, leftLowerLeg, rightThigh, rightLowerLeg;

    float minUpperArmRotation = -125f;
    float maxUpperArmRotation = 70f;

    float minForearmRotationUpper = -70f;
    float maxForearmRotationUpper = 70f;
    float minForearmRotationLower = 90f;
    float maxForearmRotationLower = 215f;

    float minThighRotation = -55f;
    float maxThighRotation = 20f;

    float minLowerLegRotation = -20f;
    float maxLowerLegRotation = 90f;

    void Start()
    {
        if (Random.Range(1, 3) == 1)
        {
            head.flipX = true;
            hair.flipX = true;
            helmet.flipX = true;
        }

        leftUpperArm.eulerAngles  += new Vector3(0, 0, Random.Range(minUpperArmRotation, maxUpperArmRotation));
        rightUpperArm.eulerAngles += new Vector3(0, 0, Random.Range(-maxUpperArmRotation, -minUpperArmRotation));
        
        if (leftUpperArm.localRotation.z > 0)
            leftForearm.eulerAngles += new Vector3(0, 0, Random.Range(minForearmRotationLower, maxForearmRotationLower));
        else
            leftForearm.eulerAngles += new Vector3(0, 0, Random.Range(minForearmRotationUpper, maxForearmRotationUpper));

        if (rightUpperArm.localRotation.z > 0)
            rightForearm.eulerAngles  += new Vector3(0, 0, Random.Range(minForearmRotationUpper, maxForearmRotationUpper));
        else
            rightForearm.eulerAngles += new Vector3(0, 0, Random.Range(-maxForearmRotationLower, -minForearmRotationLower));

        leftThigh.eulerAngles     += new Vector3(0, 0, Random.Range(minThighRotation, maxThighRotation));
        rightThigh.eulerAngles    += new Vector3(0, 0, Random.Range(-maxThighRotation, -minThighRotation));

        leftLowerLeg.eulerAngles  += new Vector3(0, 0, Random.Range(minLowerLegRotation, maxLowerLegRotation));
        rightLowerLeg.eulerAngles += new Vector3(0, 0, Random.Range(-maxLowerLegRotation, -minLowerLegRotation));
    }
}
