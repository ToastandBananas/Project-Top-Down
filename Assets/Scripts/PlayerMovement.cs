using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float walkSpeed = 2f;
    public float runSpeed = 3f;

    Animator anim, legsAnim, rightArmAnim, leftArmAnim;
    Rigidbody2D rb;
    Camera cam;

    float horizontalMove, verticalMove;
    float angle;
    Vector3 dir;
    Vector3 move;
    
    void Start()
    {
        cam  = Camera.main;
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        rightArmAnim = transform.Find("Arms").Find("Right Arm").GetComponent<Animator>();
        leftArmAnim = transform.Find("Arms").Find("Left Arm").GetComponent<Animator>();
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
        if (Input.GetButton("Sprint"))
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

            move = move.normalized * moveSpeed * Time.fixedDeltaTime;
            transform.position += move;
        }
        else
        {
            ResetAnims();
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
}
