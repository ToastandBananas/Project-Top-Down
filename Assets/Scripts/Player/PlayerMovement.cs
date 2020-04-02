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

    public bool isMounted;

    Animator anim, legsAnim, rightArmAnim, leftArmAnim;
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
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        stats = GetComponent<BasicStats>();
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
    }

    void FixedUpdate()
    {
        if (canDodge)
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

    public void LookAtMouse()
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
    }

    bool CanDodge(Vector3 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance, obstacleMask).collider == null;
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

    public void StartPickUpCooldown()
    {
        StartCoroutine(PickUpCooldown());
    }

    IEnumerator PickUpCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        itemsToBePickedUpCount = 0;
    }

    public IEnumerator SmoothMovement(Vector3 targetPos)
    {
        float timer = 0;
        while (stats.isDead == false && Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            timer += Time.smoothDeltaTime;
            if (timer > 1f)
                break;

            transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * 4 * Time.deltaTime);
            yield return null; // Pause for one frame
        }
    }
}
