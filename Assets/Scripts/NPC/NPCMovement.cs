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

    public bool isDodging;
    public bool isStaggered;

    AnimTimeManager animTimeManager;
    FieldOfView fov;
    AstarPath AstarPath;
    Pathfinding.AIPath AIPath;
    Pathfinding.AIDestinationSetter AIDestSetter;
    Animator bodyAnim, legsAnim;
    NPCCombat npcCombat;
    NPCAttacks npcAttacks;
    BasicStats stats;
    Arms arms;

    Transform closestTarget;
    float closestTargetDist;
    
    void Start()
    {
        bodyAnim = GetComponent<Animator>();
        legsAnim = transform.Find("Legs").GetComponent<Animator>();
        patrolPoint = transform.Find("Patrol Point");
        npcCombat = GetComponent<NPCCombat>();
        npcAttacks = GetComponent<NPCAttacks>();
        stats = GetComponent<BasicStats>();
        arms = transform.Find("Arms").GetComponent<Arms>();

        animTimeManager = GameManager.instance.GetComponent<AnimTimeManager>();
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
        
        if (AIDestSetter.target == null || isDodging)
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
            AIPath.maxSpeed = runSpeed;
            npcCombat.DetermineCombatActions();
        }
        else if (currentState == STATE.FLEE)
        {
            // TODO
        }
        else // Idle
        {
            AIDestSetter.target = null;
        }
    }

    void SetIsMoving(bool isMoving)
    {
        bodyAnim.SetBool("isMoving", isMoving);
        legsAnim.SetBool("isMoving", isMoving);
        arms.leftArmAnim.SetBool("isMoving", isMoving);
        arms.rightArmAnim.SetBool("isMoving", isMoving);
    }

    public IEnumerator Stagger()
    {
        isStaggered = true;
        AIPath.canMove = false;
        npcAttacks.isBlocking = false;
        arms.leftArmAnim.SetBool("doStagger", true);
        arms.rightArmAnim.SetBool("doStagger", true);
        bodyAnim.SetBool("doStagger", true);
        legsAnim.SetBool("doStagger", true);
        legsAnim.SetBool("isMoving", false);

        yield return new WaitForSeconds(animTimeManager.staggerTime);

        isStaggered = false;
        AIPath.canMove = true;
        arms.leftArmAnim.SetBool("doStagger", false);
        arms.rightArmAnim.SetBool("doStagger", false);
        bodyAnim.SetBool("doStagger", false);
        legsAnim.SetBool("doStagger", false);
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
