using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public Vector2 movementInput;
    public Vector2 lookInput;

    float walkSpeed;
    public float moveSpeed;
    public float runSpeed = 3f;
    public float dodgeDistance = 1f;
    public float dodgeCooldownTime = 1f;
    int dodgeStaminaUse = 10;

    public bool isMoving;
    public bool isDodging;
    public bool isAttackDashing;
    public bool isStaggered;
    public bool isMounted;

    AudioManager audioManager;
    AnimTimeManager animTimeManager;
    GameManager gm;
    Animator bodyAnim, legsAnim;
    Arms arms;
    Rigidbody2D rb;
    Camera cam;
    PlayerAttack playerAttack;
    BasicStats stats;
    Transform headReset;

    [HideInInspector] public Pathfinding.AIDestinationSetter AIDestSetter;
    [HideInInspector] public Pathfinding.AIPath AIPath;
    Pathfinding.RVO.RVOController RVOController;

    float angleToRotateTowards;
    Vector3 dir;
    Vector3 move;
    Vector3 lastMoveDir;
    bool canDodge = true;

    Ray ray;

    LayerMask obstacleMask;

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
        playerAttack = GetComponent<PlayerAttack>();
        stats = GetComponent<BasicStats>();
        headReset = transform.Find("Head Reset");
        audioManager = AudioManager.instance;
        animTimeManager = GameManager.instance.GetComponent<AnimTimeManager>();
        gm = GameManager.instance;
        arms = transform.Find("Arms").GetComponent<Arms>();
        bodyAnim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "OpenDoors", "ClosedDoors");

        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        AIPath = GetComponent<Pathfinding.AIPath>();
        RVOController = GetComponent<Pathfinding.RVO.RVOController>();

        CalculateMoveSpeeds();

        StartCoroutine(UpdateFootstepSounds());
    }

    void Update()
    {
        Update_Dodge();
    }

    void FixedUpdate()
    {
        if (isDodging == false && isStaggered == false)
            Update_Movement();
    }

    public IEnumerator MoveToInteractable(Transform target, float speed)
    {
        Interactable interactable = target.GetComponent<Interactable>();

        // Set pathfinding variables and player movement anims
        RVOController.enabled = true;
        AIDestSetter.target = target;
        AIPath.canMove = true;
        AIPath.maxSpeed = speed;
        SetIsMoving(true);

        while (true)
        {
            if (interactable.playerInRange)
            {
                if (interactable.TryGetComponent(out ItemPickup itemPickup) != false)
                    itemPickup.Interact();
                else if (interactable.TryGetComponent(out Container container) != false)
                    container.Interact();
                else if (interactable.TryGetComponent(out Door door) != false)
                    door.Interact();

                StopPathfinding();
                break;
            }
            else if (AIDestSetter.target == null)
            {
                StopPathfinding();
                break;
            }

            yield return null;
        }
    }

    void Update_Movement()
    {
        movementInput = GameControls.gamePlayActions.playerMovementAxis.Value;
        lookInput = GameControls.gamePlayActions.playerLookAxis.Value;

        if (GameControls.gamePlayActions.playerSprint.IsPressed)
            moveSpeed = walkSpeed;
        else
            moveSpeed = runSpeed;
        
        if (movementInput.x > 0.3f || movementInput.x < -0.3f || movementInput.y > 0.3f || movementInput.y < -0.3f)
        {
            if (AIDestSetter.target != null)
                StopPathfinding();

            SetIsMoving(true);

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
        else if (AIDestSetter.target == null)
            SetIsMoving(false);
    }

    void SetIsMoving(bool moving)
    {
        isMoving = moving;
        bodyAnim.SetBool("isMoving", moving);
        legsAnim.SetBool("isMoving", moving);
        arms.rightArmAnim.SetBool("isMoving", moving);
        arms.leftArmAnim.SetBool("isMoving", moving);
    }

    public void StopPathfinding()
    {
        AIDestSetter.target = null;
        AIPath.canMove = false;
        SetIsMoving(false);
        RVOController.enabled = false;
    }

    IEnumerator UpdateFootstepSounds()
    {
        while (true)
        {
            if (isMoving)
            {
                yield return new WaitForSeconds(animTimeManager.footstepTime);
                audioManager.PlayRandomSound(audioManager.footstepsStandard, transform.position);
            }
            else
                yield return null;
        }
    }

    void Update_Dodge()
    {
        if (GameControls.gamePlayActions.playerDodge.WasPressed && canDodge && gm.menuOpen == false && gm.dialogueUI.isOpen == false)
        {
            canDodge = CanDodge(lastMoveDir, dodgeDistance);

            if (canDodge && stats.UseStamina(dodgeStaminaUse, false))
            {
                StartCoroutine(SmoothMovement(transform.position + lastMoveDir * dodgeDistance));
                canDodge = false;
            }

            StartCoroutine(DodgeCooldown(dodgeCooldownTime));
        }
    }

    public void LookAtMouse()
    {
        dir = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
        angleToRotateTowards = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angleToRotateTowards, Vector3.forward);
    }

    void ResetAnims()
    {
        isMoving = false;
        bodyAnim.SetBool("isMoving", false);
        arms.rightArmAnim.SetBool("isMoving", false);
        arms.leftArmAnim.SetBool("isMoving", false);
        legsAnim.SetBool("isMoving", false);
        legsAnim.SetBool("isMovingLeft", false);
        legsAnim.SetBool("isMovingRight", false);
    }

    void CalculateMoveSpeeds()
    {
        walkSpeed = runSpeed / 2;
    }

    bool CanDodge(Vector3 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance, obstacleMask).collider == null;
    }

    IEnumerator DestroyEffect(Transform effect, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(effect.gameObject);
    }

    IEnumerator DodgeCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canDodge = true;
    }

    public IEnumerator Stagger()
    {
        isStaggered = true;
        playerAttack.isBlocking = false;
        arms.leftArmAnim.SetBool("doStagger", true);
        arms.rightArmAnim.SetBool("doStagger", true);
        bodyAnim.SetBool("doStagger", true);
        legsAnim.SetBool("doStagger", true);
        legsAnim.SetBool("isMoving", false);

        yield return new WaitForSeconds(animTimeManager.staggerTime);

        isStaggered = false;
        arms.leftArmAnim.SetBool("doStagger", false);
        arms.rightArmAnim.SetBool("doStagger", false);
        bodyAnim.SetBool("doStagger", false);
        legsAnim.SetBool("doStagger", false);
    }

    public IEnumerator SmoothMovement(Vector3 targetPos)
    {
        isDodging = true;
        float timer = 0;

        while (stats.isDead == false && Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            if (timer < 1f)
                timer += Time.deltaTime;
            else
            {
                isDodging = false;
                isAttackDashing = false;
                break;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * 4 * Time.deltaTime);

            yield return null; // Pause for one frame
        }

        while (isDodging)
        {
            if (Vector2.Distance(transform.position, targetPos) <= 0.1f)
            {
                isDodging = false;
                isAttackDashing = false;
            }

            yield return null;
        }
    }
}
