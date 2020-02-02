using System.Collections;
using System.Collections.Generic;
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

    float horizontalMove, verticalMove;
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
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove   = Input.GetAxisRaw("Vertical");

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
        else if (Input.GetButton("Sprint"))
            moveSpeed = walkSpeed;
        else
            moveSpeed = runSpeed;
        
        if ((Mathf.Abs(horizontalMove) != 0 || Mathf.Abs(verticalMove) != 0))
        {
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            rightArmAnim.SetBool("isMoving", true);
            leftArmAnim.SetBool("isMoving", true);
            move = Vector3.zero;

            if (verticalMove > 0)
                move += transform.up;
            else if (verticalMove < 0)
                move -= transform.up;

            if (horizontalMove > 0)
            {
                move += transform.right;
                legsAnim.SetBool("isMovingLeft", false);
                legsAnim.SetBool("isMovingRight", true);
            }
            else if (horizontalMove < 0)
            {
                move -= transform.right;
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
        if (Input.GetButtonDown("Dodge"))
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
