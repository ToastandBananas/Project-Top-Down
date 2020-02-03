using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform effectsPrefab;

    float dodgeDistance = 1f;
    public float moveSpeed;
    public float runSpeed = 3f;
    float walkSpeed;
    float blockSpeed;

    Animator anim, legsAnim, rightArmAnim, leftArmAnim;
    Rigidbody2D rb;
    Camera cam;
    PlayerAttack playerAttack;

    Vector2 movementInput;
    float angle;
    Vector3 dir;
    Vector3 move;
    Vector3 lastMoveDir;
    bool canDodge;

    LayerMask obstacleMask;
    
    void Start()
    {
        cam  = Camera.main;
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerAttack = PlayerAttack.instance;
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        rightArmAnim = transform.Find("Arms").Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = transform.Find("Arms").Find("Left Arm").GetComponent<Animator>();
        obstacleMask = LayerMask.GetMask("Walls", "Doors");

        CalculateMoveSpeeds();
    }

    void Update()
    {
        Dodge();
    }

    void FixedUpdate()
    {
        movementInput = GameControls.gamePlayActions.playerMovementAxis.Value;

        if (Vector2.Distance(transform.position, cam.ScreenToWorldPoint(Input.mousePosition)) > 0.25f)
        {
            Movement();
            LookAtMouse();
        }
        else
            ResetAnims();
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
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            rightArmAnim.SetBool("isMoving", true);
            leftArmAnim.SetBool("isMoving", true);
            move = Vector3.zero;

            if (movementInput.y > 0.3f)
                move += Vector3.up;
            else if (movementInput.y < -0.3f)
                move += Vector3.down;

            if (movementInput.x > 0.3f)
            {
                move += Vector3.right;
                legsAnim.SetBool("isMovingLeft", false);
                legsAnim.SetBool("isMovingRight", true);
            }
            else if (movementInput.x < -0.3f)
            {
                move += Vector3.left;
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
        if (GameControls.gamePlayActions.playerDodge.WasPressed)
        {
            canDodge = CanDodge(lastMoveDir, dodgeDistance);

            if (canDodge)
                StartCoroutine(SmoothMovement(transform.position + lastMoveDir * dodgeDistance));
        }
    }

    void LookAtMouse()
    {
        dir = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ResetAnims()
    {
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
