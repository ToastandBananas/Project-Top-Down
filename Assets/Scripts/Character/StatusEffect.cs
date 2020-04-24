using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [Header("General")]
    public string effectName = "New Status Effect";
    public string description;
    public float duration;
    public Sprite effectIcon;

    [Header("Needs")]
    public float nourishmentDrainRateChange; // Positive num good, negative num bad
    public float hydrationDrainRateChange;
    public float energyDrainRateChange;

    [Header("Basics Stats")]
    public float staminaRegenEfficiencyChange;
    public float staminaDrainRate;
    public float healthDrainRate;
    public float manaDrainRate;

    [HideInInspector] public Coroutine staminaDrainCoroutine;
    [HideInInspector] public Coroutine healthDrainCoroutine;
    [HideInInspector] public Coroutine manaDrainCoroutine;

    public void StartStatusEffect(BasicStats stats, Needs needs, bool shouldAdjustStats)
    {
        if (shouldAdjustStats)
        {
            if (needs != null)
            {
                needs.nourishmentDrainRate += nourishmentDrainRateChange;
                needs.hydrationDrainRate += hydrationDrainRateChange;
                needs.energyDrainRate += energyDrainRateChange;
            }

            stats.staminaRegenEfficiency += staminaRegenEfficiencyChange;
        }

        if (staminaDrainRate > 0)
            staminaDrainCoroutine = stats.StartCoroutine(stats.StaminaDrain(duration, healthDrainRate));
        if (healthDrainRate > 0)
            healthDrainCoroutine = stats.StartCoroutine(stats.HealthDrain(duration, healthDrainRate));
        if (manaDrainRate > 0)
            manaDrainCoroutine = stats.StartCoroutine(stats.ManaDrain(duration, healthDrainRate));
    }

    public void RemoveStatusEffect(BasicStats basicStats, Needs needs)
    {
        if (needs != null)
        {
            needs.nourishmentDrainRate -= nourishmentDrainRateChange;
            needs.hydrationDrainRate -= hydrationDrainRateChange;
            needs.energyDrainRate -= energyDrainRateChange;
        }

        basicStats.staminaRegenEfficiency -= staminaRegenEfficiencyChange;

        if (staminaDrainRate > 0)
        {
            basicStats.StopCoroutine(staminaDrainCoroutine);
            staminaDrainCoroutine = null;
        }
        if (healthDrainRate > 0)
        {
            basicStats.StopCoroutine(healthDrainCoroutine);
            healthDrainCoroutine = null;
        }
        if (manaDrainRate > 0)
        {
            basicStats.StopCoroutine(manaDrainCoroutine);
            manaDrainCoroutine = null;
        }
    }
}
