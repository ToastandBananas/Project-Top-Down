using System.Collections;
using UnityEngine;

public class NPCCombat : MonoBehaviour
{
    public float attackDistance = 1.6f;
    public float meleeCombatDistance = 5f;
    public float rangedCombatDistance = 10f;

    [HideInInspector] public bool determineMoveDirection = true;
    [HideInInspector] public bool needsCombatAction = false;
    bool determineShieldState = true;

    Arms arms;
    BasicStats basicStats;
    NPCMovement npcMovement;
    NPCAttacks npcAttacks;
    Pathfinding.AIDestinationSetter AIDestSetter;

    void Start()
    {
        arms = GetComponentInChildren<Arms>();
        basicStats = GetComponent<BasicStats>();
        npcMovement = GetComponent<NPCMovement>();
        npcAttacks = GetComponent<NPCAttacks>();
        AIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
    }

    public void DetermineCombatActions()
    {
        if (npcMovement.isStaggered == false)
        {
            // Raise or lower shield
            DetermineShieldState();

            // Strafe left/right or move towards target
            DetermineMoveDirection();

            // If moving towards target, attack when close enough
            DetermineAttack();
        }
    }

    void DetermineShieldState()
    {
        if (determineShieldState && (arms.leftShieldEquipped || arms.rightShieldEquipped))
        {
            int randomNumber = Random.Range(1, 101);

            if (randomNumber <= 70 && basicStats.stamina / basicStats.maxStamina >= 0.15f) // 70% chance to block if the NPC has enough stamina
            {
                npcAttacks.isBlocking = true;
                arms.RaiseShield();
            }
            else // 30% chance to lower shield if already blocking
            {
                npcAttacks.isBlocking = false;
                arms.LowerShield();
            }

            determineShieldState = false;
            
            StartCoroutine(ShieldStateCooldown());
        }
    }

    void DetermineMoveDirection()
    {
        if (determineMoveDirection)
        {
            int randomNumber = Random.Range(1, 101);

            // Melee combat behaviour
            if (arms.leftWeaponEquipped || arms.rightWeaponEquipped)
            {
                if (randomNumber <= 60) // 60% chance to move towards target to attack
                {
                    AIDestSetter.target = npcMovement.attackTarget;
                    needsCombatAction = true;
                }
                else if (randomNumber > 80) // 20% chance to strafe left
                {
                    npcMovement.patrolPoint.position = transform.position - (transform.right * 2) + transform.up;
                    AIDestSetter.target = npcMovement.patrolPoint;
                    StartCoroutine(StrafeCooldown());
                }
                else // 20% chance to strafe right
                {
                    npcMovement.patrolPoint.position = transform.position + (transform.right * 2) + transform.up;
                    AIDestSetter.target = npcMovement.patrolPoint;
                    StartCoroutine(StrafeCooldown());
                }
            }
            // Ranged combat behaviour
            else if (arms.rangedWeaponEquipped)
            {
                if (randomNumber <= 80)
                    needsCombatAction = true;
                else
                {
                    arms.rightArmAnim.SetBool("doReleaseArrow", false);
                    arms.leftArmAnim.SetBool("doDrawArrow", false);
                    arms.rightArmAnim.SetBool("doDrawArrow", false);
                    arms.bodyAnim.SetBool("doDrawArrow", false);

                    StartCoroutine(MoveDirectionCooldown());
                }
            }

            determineMoveDirection = false;
        }
    }

    void DetermineAttack()
    {
        if (needsCombatAction)
        {
            if ((arms.leftWeaponEquipped || arms.rightWeaponEquipped) && npcAttacks.comboAttackOnCooldown == false 
                && Vector2.Distance(transform.position, npcMovement.attackTarget.position) <= attackDistance)
            {
                if (npcAttacks.leftComboNumber > 1 || npcAttacks.rightComboNumber > 1) // If the NPC is in the middle of a combo attack sequence, do the next combo attack in the sequence
                    npcAttacks.ComboAttack();
                else // Otherwise, randomly choose an attack
                {
                    int randomNumber = Random.Range(1, 10);
                    if (randomNumber == 1)
                        npcAttacks.QuickAttack();
                    else if (randomNumber == 2)
                        npcAttacks.HeavyAttack();
                    else
                        npcAttacks.ComboAttack();
                }
                
                needsCombatAction = false;
            }
            else if (arms.rangedWeaponEquipped && Vector2.Distance(transform.position, npcMovement.attackTarget.position) <= rangedCombatDistance)
            {
                StartCoroutine(npcAttacks.RangedAttack());
                needsCombatAction = false;
            }
        }
    }

    IEnumerator MoveDirectionCooldown()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.75f));
        determineMoveDirection = true;
    }

    IEnumerator ShieldStateCooldown()
    {
        float randomCooldownTime = Random.Range(4, 6);
        yield return new WaitForSeconds(randomCooldownTime);
        determineShieldState = true;
    }

    IEnumerator StrafeCooldown()
    {
        float randomCooldownTime = Random.Range(1.5f, 3);
        yield return new WaitForSeconds(randomCooldownTime);
        determineMoveDirection = true;
    }
}
