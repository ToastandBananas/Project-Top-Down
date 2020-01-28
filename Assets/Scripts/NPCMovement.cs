using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE { IDLE, PATROL, PURSUE, ATTACK };
public enum ALLIANCE { NEUTRAL, PLAYER, BANDITS };

public class NPCMovement : MonoBehaviour
{
    public STATE currentState;
    public ALLIANCE currentAlliance = ALLIANCE.NEUTRAL;

    public Transform targetPos;
    public Transform attackTarget;
    public Transform patrolPoint;

    public float walkSpeed = 2f;
    public float runSpeed = 3f;
    public float attackDistance = 1.3f;

    FieldOfView fov;
    AstarPath AstarPath;
    Pathfinding.AIPath AIPath;
    Pathfinding.AIDestinationSetter AIDestSetter;
    Animator anim, legsAnim;

    float angle;
    Vector3 dir;
    Vector3 move;

    Transform closestTarget;
    float closestTargetDist;
    
    void Start()
    {
        currentState = STATE.IDLE;

        anim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();

        fov = GetComponent<FieldOfView>();
        AstarPath = FindObjectOfType<AstarPath>();
        AIPath = GetComponent<Pathfinding.AIPath>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        AIPath.maxSpeed = walkSpeed;
        // attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
        patrolPoint = transform.Find("Patrol Point");

        if (attackTarget != null)
        {
            AIDestSetter.target = attackTarget;
            currentState = STATE.PURSUE;
        }
        else if (targetPos != null)
            AIDestSetter.target = targetPos;
    }
    
    void FixedUpdate()
    {
        // AstarPath.Scan(AstarPath.graphs);
        DetermineState();
        Movement();
    }

    void DetermineState()
    {
        if (fov.visibleTargets.Count > 0)
        {
            closestTarget = null;
            closestTargetDist = -100;

            foreach (Transform target in fov.visibleTargets)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (closestTargetDist == -100 || dist < closestTargetDist)
                {
                    closestTargetDist = dist;
                    closestTarget = target;
                }
            }

            if (closestTarget != null)
            {
                attackTarget = closestTarget;
                AIDestSetter.target = attackTarget;
                currentState = STATE.PURSUE;
            }
        }
    }

    void Movement()
    {
        if (attackTarget != null && Vector2.Distance(transform.position, attackTarget.position) < attackDistance)
            currentState = STATE.ATTACK;
        else if (attackTarget != null && Vector2.Distance(transform.position, attackTarget.position) >= attackDistance)
            currentState = STATE.PURSUE;

        // NPC's don't move sideways currently, so set these to false
        legsAnim.SetBool("isMovingRight", false);
        legsAnim.SetBool("isMovingLeft", false);

        if (currentState == STATE.PURSUE)
        {
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            AIPath.maxSpeed = runSpeed;
        }
        else if (currentState == STATE.PATROL)
        {
            anim.SetBool("isMoving", true);
            legsAnim.SetBool("isMoving", true);
            AIPath.maxSpeed = walkSpeed;
        }
        else if (currentState == STATE.ATTACK)
        {
            anim.SetBool("isMoving", false);
            legsAnim.SetBool("isMoving", false);

            Debug.Log(name + " attacked you.");
        }
        else
        {
            anim.SetBool("isMoving", false);
            legsAnim.SetBool("isMoving", false);
        }
    }
}
