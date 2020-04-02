using System.Collections;
using UnityEngine;

public enum STATE { IDLE, PATROL, PURSUE, COMBAT, FLEE };
public enum ALLIANCE { NEUTRAL, PLAYER, BANDITS, GUARDS };

public class NPCMovement : MonoBehaviour
{
    public STATE defaultState = STATE.IDLE;
    public STATE currentState = STATE.IDLE;
    public ALLIANCE currentAlliance = ALLIANCE.NEUTRAL;

    public Transform targetPos;
    public Transform attackTarget;
    [HideInInspector] public Transform patrolPoint;

    public float walkSpeed = 2f;
    public float runSpeed = 3f;

    FieldOfView fov;
    AstarPath AstarPath;
    Pathfinding.AIPath AIPath;
    Pathfinding.AIDestinationSetter AIDestSetter;
    Animator anim, legsAnim, leftArmAnim, rightArmAnim;
    NPCCombat npcCombat;
    BasicStats stats;
    Arms arms;

    Transform closestTarget;
    float closestTargetDist;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        leftArmAnim = transform.Find("Arms").Find("Left Arm").GetComponent<Animator>();
        rightArmAnim = transform.Find("Arms").Find("Right Arm").GetComponent<Animator>();
        patrolPoint = transform.Find("Patrol Point");
        npcCombat = GetComponent<NPCCombat>();
        stats = GetComponent<BasicStats>();
        arms = transform.Find("Arms").GetComponent<Arms>();

        fov = GetComponent<FieldOfView>();
        AstarPath = FindObjectOfType<AstarPath>();
        AIPath = GetComponent<Pathfinding.AIPath>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();

        AIPath.maxSpeed = walkSpeed;
        AIPath.endReachedDistance = npcCombat.attackDistance;

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
        
        if (AIDestSetter.target == null)
        {
            AIPath.canMove = false;
            SetIsMoving(false);
        }
        else
        {
            AIPath.canMove = true;
            SetIsMoving(true);
        }
    }

    void DetermineState()
    {
        if (fov.visibleTargets.Count > 0)
        {
            closestTarget = null;
            closestTargetDist = 0;

            foreach (Transform target in fov.visibleTargets)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (closestTarget == null || dist < closestTargetDist)
                {
                    closestTargetDist = dist;
                    closestTarget = target;
                }
            }

            if (closestTarget != null)
            {
                attackTarget = closestTarget;
                if (currentState != STATE.COMBAT)
                {
                    AIDestSetter.target = attackTarget;
                    currentState = STATE.PURSUE;
                }
            }
        }

        if (attackTarget != null && arms.rangedWeaponEquipped)
        {
            // Move away from target if too close
            if (Vector2.Distance(attackTarget.position, transform.position) < npcCombat.rangedCombatDistance - 2)
            {
                Vector3 dir = (transform.position - attackTarget.position).normalized;
                patrolPoint.position = dir * 2;
                AIDestSetter.target = patrolPoint;
            }
            else
                AIDestSetter.target = null;
        }

        if (attackTarget != null 
            && (((arms.leftWeaponEquipped || arms.rightWeaponEquipped) && Vector2.Distance(transform.position, attackTarget.position) < npcCombat.meleeCombatDistance) 
            || (arms.rangedWeaponEquipped && Vector2.Distance(transform.position, attackTarget.position) < npcCombat.rangedCombatDistance)))
            currentState = STATE.COMBAT;
        else if (attackTarget != null
            && (((arms.leftWeaponEquipped || arms.rightWeaponEquipped) && Vector2.Distance(transform.position, attackTarget.position) >= npcCombat.meleeCombatDistance)
            || (arms.rangedWeaponEquipped && Vector2.Distance(transform.position, attackTarget.position) >= npcCombat.rangedCombatDistance)))
            currentState = STATE.PURSUE;
        else
            currentState = defaultState;
    }

    void Movement()
    {
        // NPC's don't move sideways currently, so set these to false
        legsAnim.SetBool("isMovingRight", false);
        legsAnim.SetBool("isMovingLeft", false);

        if (currentState == STATE.PURSUE)
        {
            AIPath.maxSpeed = runSpeed;
            AIDestSetter.target = attackTarget;
        }
        else if (currentState == STATE.PATROL)
        {
            AIPath.maxSpeed = walkSpeed;
        }
        else if (currentState == STATE.COMBAT)
        {
            /*if (AIDestSetter.target == null || ((arms.leftWeaponEquipped || arms.rightWeaponEquipped) && Vector2.Distance(transform.position, AIDestSetter.target.position) > npcCombat.attackDistance)
                || (arms.rangedWeaponEquipped && Vector2.Distance(transform.position, AIDestSetter.target.position) > npcCombat.rangedAttackDistance))
                SetIsMoving(true);
            else
                SetIsMoving(false);*/

            AIPath.maxSpeed = runSpeed;

            npcCombat.DetermineCombatActions();
        }
        else if (currentState == STATE.FLEE)
        {
            //SetIsMoving(true);
        }
        else // Idle
        {
            AIDestSetter.target = null;
        }
    }

    void SetIsMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
        legsAnim.SetBool("isMoving", isMoving);
        leftArmAnim.SetBool("isMoving", isMoving);
        rightArmAnim.SetBool("isMoving", isMoving);
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
