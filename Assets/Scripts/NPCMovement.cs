using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState { IDLE, PATROL, PURSUE, ATTACK };

public class NPCMovement : MonoBehaviour
{
    public NPCState currentState = NPCState.PURSUE;

    public Transform target;
    public Transform patrolPoint;

    public float walkSpeed = 2f;
    public float runSpeed = 3f;
    public bool isMoving;

    AstarPath AstarPath;
    Pathfinding.AIPath AIPath;
    Pathfinding.AIDestinationSetter AIDestSetter;
    Animator anim;

    float angle;
    Vector3 dir;
    Vector3 move;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        AstarPath = FindObjectOfType<AstarPath>();
        AIPath = GetComponent<Pathfinding.AIPath>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        AIPath.maxSpeed = walkSpeed;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        patrolPoint = transform.Find("Patrol Point");

        if (target != null)
            AIDestSetter.target = target;
    }
    
    void FixedUpdate()
    {
        // AstarPath.Scan(AstarPath.graphs);
        Movement();
    }

    void Movement()
    {
        if (currentState == NPCState.PURSUE)
        {
            isMoving = true;
            anim.SetBool("isMoving", isMoving);
            AIPath.maxSpeed = runSpeed;
        }
        else if (currentState == NPCState.PATROL)
        {
            isMoving = true;
            anim.SetBool("isMoving", isMoving);
            AIPath.maxSpeed = walkSpeed;
        }
        else
        {
            isMoving = false;
            anim.SetBool("isMoving", isMoving);
        }
    }

    void LookAtTarget()
    {
        dir = target.position - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
