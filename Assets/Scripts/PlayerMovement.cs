using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public Transform effectsPrefab;

    public float moveSpeed;
    public float runSpeed = 3f;
    public float dodgeDistance = 1f;
    public float dodgeCooldownTime = 1f;
    float walkSpeed;
    float blockSpeed;

    Animator anim, legsAnim, rightArmAnim, leftArmAnim;
    Rigidbody2D rb;
    Camera cam;
    PlayerAttack playerAttack;
    FieldOfView fov;

    Transform headReset;

    Vector2 movementInput;
    float angleToRotateTowards;
    Vector3 dir;
    Vector3 move;
    Vector3 lastMoveDir;
    bool canDodge = true;
    bool isMoving;
    bool isLockedOn;
    bool canSwitchLockOnTarget = true;
    float maxLockOnDist = 12f;

    LayerMask obstacleMask;
    
    public Transform closestEnemy;
    public Transform lockOnTarget;

    List<Transform> nearbyEnemies;

    void Start()
    {
        cam  = Camera.main;
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        fov = GetComponent<FieldOfView>();
        headReset = transform.Find("Head Reset");
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        rightArmAnim = transform.Find("Arms").Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = transform.Find("Arms").Find("Left Arm").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");
        nearbyEnemies = new List<Transform>();

        CalculateMoveSpeeds();
    }

    void Update()
    {
        Dodge();
        LockOn();
    }

    void FixedUpdate()
    {
        movementInput = GameControls.gamePlayActions.playerMovementAxis.Value;

        Movement();

        if (isLockedOn == false)
            FaceForward();
        else
            FaceLockOnTarget();

        /*if (Vector2.Distance(transform.position, cam.ScreenToWorldPoint(Input.mousePosition)) > 0.25f)
        {
            Movement();
            LookAtMouse();
        }
        else
            ResetAnims();*/
    }

    void Movement()
    {
        if (playerAttack.isBlocking)
            moveSpeed = blockSpeed;
        else if (GameControls.gamePlayActions.playerSprint.IsPressed)
            moveSpeed = walkSpeed;
        else
            moveSpeed = runSpeed;
        
        if (movementInput.x > 0.3f || movementInput.x < -0.3f || movementInput.y > 0.3f || movementInput.y < -0.3f)
        {
            isMoving = true;
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            rightArmAnim.SetBool("isMoving", true);
            leftArmAnim.SetBool("isMoving", true);
            move = Vector3.zero;

            if (movementInput.y > 0.3f)
                move += new Vector3(0, movementInput.y, 0);
            else if (movementInput.y < -0.3f)
                move += new Vector3(0, movementInput.y, 0);

            if (movementInput.x > 0.3f)
            {
                move += new Vector3(movementInput.x, 0, 0);
                legsAnim.SetBool("isMovingLeft", false);
                legsAnim.SetBool("isMovingRight", true);
            }
            else if (movementInput.x < -0.3f)
            {
                move += new Vector3(movementInput.x, 0, 0);
                legsAnim.SetBool("isMovingLeft", true);
                legsAnim.SetBool("isMovingRight", false);
            }
            else
            {
                legsAnim.SetBool("isMovingLeft", false);
                legsAnim.SetBool("isMovingRight", false);
            }

            lastMoveDir = move.normalized;

            move = move.normalized * moveSpeed * Time.fixedDeltaTime;
            transform.position += move;
        }
        else
            ResetAnims();
    }

    void Dodge()
    {
        if (canDodge && GameControls.gamePlayActions.playerDodge.WasPressed)
        {
            canDodge = CanDodge(lastMoveDir, dodgeDistance);

            if (canDodge)
                StartCoroutine(SmoothMovement(transform.position + lastMoveDir * dodgeDistance));

            canDodge = false;
            StartCoroutine(DodgeCooldown(dodgeCooldownTime));
        }
    }

    void LockOn()
    {
        if (GameControls.gamePlayActions.playerLockOn.WasPressed)
        {
            if (isLockedOn == false)
            {
                closestEnemy = GetClosestEnemy();

                if (closestEnemy != null)
                {
                    isLockedOn = true;
                    lockOnTarget = closestEnemy;
                }
            }
            else
                UnLockOn();
        }

        if (GameControls.gamePlayActions.playerSwitchLockOnTargetAxis.Value < -0.3f && canSwitchLockOnTarget)
        {
            GetNearbyEnemies();

            if (nearbyEnemies.Count > 1)
            {
                int closestEnemyIndex = nearbyEnemies.IndexOf(lockOnTarget);

                if (nearbyEnemies[closestEnemyIndex] == nearbyEnemies[0])
                    lockOnTarget = nearbyEnemies[nearbyEnemies.Count - 1];
                else
                    lockOnTarget = nearbyEnemies[closestEnemyIndex - 1];

                canSwitchLockOnTarget = false;
                StartCoroutine(LockOnSwitchTargetCooldown(0.25f));
            }
        }
        else if (GameControls.gamePlayActions.playerSwitchLockOnTargetAxis.Value > 0.3f && canSwitchLockOnTarget)
        {
            GetNearbyEnemies();

            if (nearbyEnemies.Count > 1)
            {
                int closestEnemyIndex = nearbyEnemies.IndexOf(lockOnTarget);

                if (nearbyEnemies[closestEnemyIndex] == nearbyEnemies[nearbyEnemies.Count - 1])
                    lockOnTarget = nearbyEnemies[0];
                else
                    lockOnTarget = nearbyEnemies[closestEnemyIndex + 1];

                canSwitchLockOnTarget = false;
                StartCoroutine(LockOnSwitchTargetCooldown(0.25f));
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

    void GetNearbyEnemies()
    {
        nearbyEnemies.Clear();

        if (fov.targetsInViewRadius.Length > 0)
        {
            float distToTarget;
            foreach (Collider2D target in fov.targetsInViewRadius)
            {
                distToTarget = Vector2.Distance(transform.position, target.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position, distToTarget, obstacleMask);

                if (hit == false)
                    nearbyEnemies.Add(target.transform);
            }
        }
    }

    Transform GetClosestEnemy()
    {
        Transform possibleClosestEnemy = null;
        float distToTarget;
        closestEnemy = null;

        GetNearbyEnemies();
        foreach (Transform enemy in nearbyEnemies)
        {
            if (closestEnemy == null)
                possibleClosestEnemy = enemy.transform;
            else
            {
                distToTarget = Vector2.Distance(enemy.transform.position, transform.position);
                if (distToTarget < Vector2.Distance(possibleClosestEnemy.position, transform.position))
                    possibleClosestEnemy = enemy.transform;
            }
        }

        if (possibleClosestEnemy != null)
            return possibleClosestEnemy.transform;

        return null;
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
        if (isMoving)
            dir = movementInput;
        else
            dir = headReset.position - transform.position;

        angleToRotateTowards = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angleToRotateTowards, Vector3.forward), 300f * Time.fixedDeltaTime);
    }

    void LookAtMouse()
    {
        dir = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
        angleToRotateTowards = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angleToRotateTowards, Vector3.forward);
    }

    void ResetAnims()
    {
        isMoving = false;
        anim.SetBool("isMoving", false);
        rightArmAnim.SetBool("isMoving", false);
        leftArmAnim.SetBool("isMoving", false);
        legsAnim.SetBool("isMoving", false);
        legsAnim.SetBool("isMovingLeft", false);
        legsAnim.SetBool("isMovingRight", false);
    }

    void CalculateMoveSpeeds()
    {
        walkSpeed = runSpeed / 2;
        blockSpeed = (runSpeed / 3) * 2;
    }

    bool CanDodge(Vector3 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance, obstacleMask).collider == null;
    }

    IEnumerator LockOnSwitchTargetCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canSwitchLockOnTarget = true;
    }

    IEnumerator DodgeCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canDodge = true;
    }

    IEnumerator DestroyEffect(Transform effect, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(effect.gameObject);
    }

    IEnumerator SmoothMovement(Vector3 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * 4 * Time.deltaTime);
            yield return null; // Pause for one frame
        }
    }
}
