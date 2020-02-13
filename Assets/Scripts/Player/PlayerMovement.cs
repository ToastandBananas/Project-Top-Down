using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

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
    bool canSwitchLockOnTarget = true;
    float maxLockOnDist = 12f;

    LayerMask obstacleMask;

    public bool isMounted;
    public bool isLockedOn;
    public Transform lockOnTarget;

    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one PlayerMovement script is active.");
            Destroy(gameObject);
        }
    }
    #endregion

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

        CalculateMoveSpeeds();
    }

    void Update()
    {
        Update_Dodge();
        Update_LockOn();
    }

    void FixedUpdate()
    {
        movementInput = GameControls.gamePlayActions.playerMovementAxis.Value;

        Update_Movement();

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

    void Update_Movement()
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
                move += new Vector3(0, movementInput.y, 0); // Up
            else if (movementInput.y < -0.3f)
                move += new Vector3(0, movementInput.y, 0); // Down

            if (movementInput.x > 0.3f) // Right
            {
                move += new Vector3(movementInput.x, 0, 0);
                /*if (isLockedOn)
                {
                    legsAnim.SetBool("isMovingLeft", false);
                    legsAnim.SetBool("isMovingRight", true);
                }*/
            }
            else if (movementInput.x < -0.3f) // Left
            {
                move += new Vector3(movementInput.x, 0, 0);
                /*if (isLockedOn)
                {
                    legsAnim.SetBool("isMovingLeft", true);
                    legsAnim.SetBool("isMovingRight", false);
                }*/
            }
            else // Not moving left or right
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

    void Update_Dodge()
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

    void Update_LockOn()
    {
        if (GameControls.gamePlayActions.playerLockOn.WasPressed)
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
            else
                UnLockOn();
        }

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

    public IEnumerator SmoothMovement(Vector3 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * 4 * Time.deltaTime);
            yield return null; // Pause for one frame
        }
    }
}
