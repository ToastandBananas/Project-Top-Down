using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public Vector2 movementInput;
    public Vector2 lookInput;

    public float moveSpeed;
    public float runSpeed = 3f;
    float walkSpeed;
    public float dodgeDistance = 1f;
    public float dodgeCooldownTime = 1f;
    public bool isMoving;
    public bool isDodging;
    public bool isStaggered;

    public bool isMounted;

    AnimTimeManager animTimeManager;
    Animator bodyAnim, legsAnim;
    Arms arms;
    Rigidbody2D rb;
    Camera cam;
    PlayerAttack playerAttack;
    BasicStats stats;
    Transform headReset;

    float angleToRotateTowards;
    Vector3 dir;
    Vector3 move;
    Vector3 lastMoveDir;
    bool canDodge = true;

    LayerMask obstacleMask;

    [HideInInspector] public int itemsToBePickedUpCount = 0;

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
        animTimeManager = GameManager.instance.GetComponent<AnimTimeManager>();
        arms = transform.Find("Arms").GetComponent<Arms>();
        bodyAnim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");

        CalculateMoveSpeeds();
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
            isMoving = true;
            bodyAnim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            arms.rightArmAnim.SetBool("isMoving", true);
            arms.leftArmAnim.SetBool("isMoving", true);
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
            Debug.Log(move);
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

    public void StartPickUpCooldown()
    {
        StartCoroutine(PickUpCooldown());
    }

    IEnumerator PickUpCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        itemsToBePickedUpCount = 0;
    }

    IEnumerator DodgeCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canDodge = true;
    }

    public IEnumerator Stagger()
    {
        isStaggered = true;
        arms.leftArmAnim.SetBool("doStagger", true);
        arms.rightArmAnim.SetBool("doStagger", true);
        bodyAnim.SetBool("doStagger", true);

        yield return new WaitForSeconds(animTimeManager.staggerTime);

        isStaggered = false;
        arms.leftArmAnim.SetBool("doStagger", false);
        arms.rightArmAnim.SetBool("doStagger", false);
        bodyAnim.SetBool("doStagger", false);
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
                break;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * 4 * Time.deltaTime);

            yield return null; // Pause for one frame
        }

        while (isDodging)
        {
            if (Vector2.Distance(transform.position, targetPos) <= 0.1f)
                isDodging = false;

            yield return null;
        }
    }
}
