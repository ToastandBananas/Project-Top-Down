using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float walkSpeed = 2f;
    public float runSpeed = 3f;
    public bool isMovingSideways;

    Animator anim;
    Animator legsAnim;
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
    }
    
    void FixedUpdate()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove   = Input.GetAxisRaw("Vertical");

        Movement();
        LookAtMouse();
    }

    void Movement()
    {
        if (Input.GetButton("Sprint"))
            moveSpeed = walkSpeed;
        else
            moveSpeed = runSpeed;

        if (Mathf.Abs(horizontalMove) != 0 || Mathf.Abs(verticalMove) != 0)
        {
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            move = Vector3.zero;

            if (verticalMove > 0)
                move += transform.up;
            else if (verticalMove < 0)
                move -= transform.up;

            if (horizontalMove > 0)
            {
                move += transform.right;
                legsAnim.SetBool("isMovingSideways", true);
            }
            else if (horizontalMove < 0)
            {
                move -= transform.right;
                legsAnim.SetBool("isMovingSideways", true);
            }
            else
                legsAnim.SetBool("isMovingSideways", false);

            move = move.normalized * moveSpeed * Time.fixedDeltaTime;
            transform.position += move;
        }
        else
        {
            anim.SetBool("isMoving", false);
            legsAnim.SetBool("isMoving", false);
            legsAnim.SetBool("isMovingSideways", false);
        }
    }

    void LookAtMouse()
    {
        dir = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
