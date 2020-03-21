using System.Collections;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    public bool isLockedOn;
    public Transform lockOnTarget;

    FieldOfView fov;
    PlayerMovement playerMovementScript;
    Transform headReset;

    bool canSwitchLockOnTarget = true;
    float maxLockOnDist = 12f;

    float angleToRotateTowards;
    Vector3 dir;

    void Start()
    {
        fov = GetComponent<FieldOfView>();
        headReset = transform.Find("Head Reset");

        if (gameObject.name == "Player")
            playerMovementScript = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Update_LockOn();
    }

    void LateUpdate()
    {
        if (isLockedOn == false)
            FaceForward();
        else
            FaceLockOnTarget();
    }

    void Update_LockOn()
    {
        if ((GameControls.gamePlayActions.playerLockOn.WasPressed && playerMovementScript != null) // If this is the player and they press the lock on button
            || (playerMovementScript == null && gameObject.name != "Player")) // Or if this is an NPC
        {
            if (isLockedOn == false)
            {
                fov.closestEnemy = fov.GetClosestEnemy();

                if (fov.closestEnemy != null)
                {
                    isLockedOn = true;
                    lockOnTarget = fov.closestEnemy;
                }
            }
            else if (playerMovementScript != null) // If this is the player's lock on script
                UnLockOn();
        }

        if (isLockedOn && gameObject.name == "Player")
        {
            // Switch lock on target
            if (GameControls.gamePlayActions.playerSwitchLockOnTargetAxis.Value < -0.3f && canSwitchLockOnTarget)
            {
                fov.GetNearbyEnemies();

                if (fov.nearbyEnemies.Count > 1)
                {
                    int closestEnemyIndex = fov.nearbyEnemies.IndexOf(lockOnTarget);

                    if (fov.nearbyEnemies[closestEnemyIndex] == fov.nearbyEnemies[0])
                        lockOnTarget = fov.nearbyEnemies[fov.nearbyEnemies.Count - 1];
                    else
                        lockOnTarget = fov.nearbyEnemies[closestEnemyIndex - 1];

                    canSwitchLockOnTarget = false;
                    StartCoroutine(LockOnSwitchTargetCooldown(0.25f));
                }
            }
            else if (GameControls.gamePlayActions.playerSwitchLockOnTargetAxis.Value > 0.3f && canSwitchLockOnTarget)
            {
                fov.GetNearbyEnemies();

                if (fov.nearbyEnemies.Count > 1)
                {
                    int closestEnemyIndex = fov.nearbyEnemies.IndexOf(lockOnTarget);

                    if (fov.nearbyEnemies[closestEnemyIndex] == fov.nearbyEnemies[fov.nearbyEnemies.Count - 1])
                        lockOnTarget = fov.nearbyEnemies[0];
                    else
                        lockOnTarget = fov.nearbyEnemies[closestEnemyIndex + 1];

                    canSwitchLockOnTarget = false;
                    StartCoroutine(LockOnSwitchTargetCooldown(0.25f));
                }
            }
        }

        if (lockOnTarget != null && Vector2.Distance(transform.position, lockOnTarget.position) > maxLockOnDist)
            UnLockOn();
    }

    void UnLockOn()
    {
        isLockedOn = false;
        lockOnTarget = null;
    }

    void FaceLockOnTarget()
    {
        if (lockOnTarget != null)
        {
            dir = lockOnTarget.transform.position - transform.position;
            angleToRotateTowards = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angleToRotateTowards, Vector3.forward), 300f * Time.fixedDeltaTime);
        }
        else
            UnLockOn();
    }

    void FaceForward()
    {
        if (playerMovementScript != null && playerMovementScript.isMoving) // Only used for the player
            dir = playerMovementScript.movementInput;
        else
            dir = headReset.position - transform.position;

        angleToRotateTowards = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angleToRotateTowards, Vector3.forward), 300f * Time.fixedDeltaTime);
    }

    IEnumerator LockOnSwitchTargetCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canSwitchLockOnTarget = true;
    }
}
