using System.Collections;
using UnityEngine;

public class NPCCombat : MonoBehaviour
{
    public float attackDistance = 1.6f;
    public float combatDistance = 4f;

    [HideInInspector] public bool determineMoveDirection = true;
    bool determineShieldState = true;
    bool needsCombatAction = false;

    Arms arms;
    NPCMovement npcMovement;
    NPCAttacks npcAttacks;
    Pathfinding.AIDestinationSetter AIDestSetter;

    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        npcMovement = GetComponent<NPCMovement>();
        npcAttacks = GetComponent<NPCAttacks>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
    }

    public void DetermineCombatActions()
    {
        DetermineShieldState();

        // strafe left/right or move towards target
        DetermineMoveDirection();

        // if moving towards target, attack when close enough
        DetermineAttack();
    }

    void DetermineShieldState()
    {
        if (determineShieldState && (arms.leftShieldEquipped || arms.rightShieldEquipped))
        {
            int randomNumber = Random.Range(1, 101);
            
            if (randomNumber <= 70) // 70% chance to block
                arms.RaiseShield();
            else // 30% chance to lower shield if already blocking
                arms.LowerShield();

            determineShieldState = false;
            
            StartCoroutine(ShieldStateCooldown());
        }
    }

    void DetermineMoveDirection()
    {
        if (determineMoveDirection)
        {
            int randomNumber = Random.Range(1, 101);

            if (randomNumber <= 60) // 60% chance to move towards target to attack
            {
                AIDestSetter.target = npcMovement.attackTarget;
                needsCombatAction = true;
            }
            else if (randomNumber > 80) // 20% chance to strafe left
            {
                npcMovement.patrolPoint.position = transform.position - (transform.right * 2) + transform.up;
                AIDestSetter.target = npcMovement.patrolPoint;
                StartCoroutine(MoveDirectionCooldown());
            }
            else // 20% chance to strafe right
            {
                npcMovement.patrolPoint.position = transform.position + (transform.right * 2) + transform.up;
                AIDestSetter.target = npcMovement.patrolPoint;
                StartCoroutine(MoveDirectionCooldown());
            }

            determineMoveDirection = false;
        }
    }

    void DetermineAttack()
    {
        if (needsCombatAction && Vector2.Distance(transform.position, npcMovement.attackTarget.position) <= attackDistance)
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1)
                npcAttacks.QuickAttack();
            else if (randomNumber == 2)
                npcAttacks.HeavyAttack();

            needsCombatAction = false;
        }
    }

    IEnumerator ShieldStateCooldown()
    {
        float randomCooldownTime = Random.Range(4, 8);
        yield return new WaitForSeconds(randomCooldownTime);
        determineShieldState = true;
    }

    IEnumerator MoveDirectionCooldown()
    {
        float randomCooldownTime = Random.Range(2, 4);
        yield return new WaitForSeconds(randomCooldownTime);
        determineMoveDirection = true;
    }
}
