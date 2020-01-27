using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE { IDLE, PATROL, PURSUE, ATTACK };
public enum ALLIANCE { NEUTRAL, PLAYER, BANDITS };

public class NPCMovement : MonoBehaviour
{
    public STATE currentState = STATE.IDLE;
    public ALLIANCE currentAlliance = ALLIANCE.NEUTRAL;

    public Transform targetPos;
    public Transform attackTarget;
    public Transform patrolPoint;

    public float walkSpeed = 2f;
    public float runSpeed = 3f;
    public float attackDistance = 1.3f;

    AstarPath AstarPath;
    Pathfinding.AIPath AIPath;
    Pathfinding.AIDestinationSetter AIDestSetter;
    Animator anim, legsAnim;

    float angle;
    Vector3 dir;
    Vector3 move;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        AstarPath = FindObjectOfType<AstarPath>();
        AIPath = GetComponent<Pathfinding.AIPath>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        AIPath.maxSpeed = walkSpeed;
        attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
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
        Movement();
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

    void LookAtTarget()
    {
        dir = targetPos.position - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
