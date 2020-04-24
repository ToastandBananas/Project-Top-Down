using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public GameObject statusEffectIconPrefab;

    Transform statusEffectsIconsParent;
    BasicStats basicStats;
    Needs needs;
    
    void Start()
    {
        statusEffectsIconsParent = GameObject.Find("Status Effects").transform;
        basicStats = GetComponent<BasicStats>();
        needs = GetComponent<Needs>();
    }

    public void AddStatusEffect(StatusEffect statusEffect, bool shouldAdjustStats)
    {
        if (needs != null)
            statusEffect.StartStatusEffect(basicStats, needs, shouldAdjustStats);
        else
            statusEffect.StartStatusEffect(basicStats, null, shouldAdjustStats);

        if (basicStats.isPlayer)
        {
            GameObject statusEffectIcon = Instantiate(statusEffectIconPrefab, statusEffectsIconsParent);
            statusEffectIcon.GetComponent<Image>().sprite = statusEffect.effectIcon;
            statusEffectIcon.name = statusEffect.effectName;
        }
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (needs != null)
            statusEffect.RemoveStatusEffect(basicStats, needs);
        else
            statusEffect.RemoveStatusEffect(basicStats, null);

        if (basicStats.isPlayer)
        {
            for (int i = 0; i < statusEffectsIconsParent.childCount; i++)
            {
                if (statusEffectsIconsParent.GetChild(i).name == statusEffect.effectName)
                {
                    Destroy(statusEffectsIconsParent.GetChild(i).gameObject);
                    break;
                }
            }
        }
    }

    public void RemovePreviousHungerStatusEffect()
    {
        for (int i = 0; i < statusEffectsIconsParent.childCount; i++)
        {
            if (statusEffectsIconsParent.GetChild(i).name == Database.instance.hungerStatusEffects[0].effectName)
            {
                RemoveStatusEffect(Database.instance.hungerStatusEffects[0]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.hungerStatusEffects[1].effectName)
            {
                RemoveStatusEffect(Database.instance.hungerStatusEffects[1]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.hungerStatusEffects[2].effectName)
            {
                RemoveStatusEffect(Database.instance.hungerStatusEffects[2]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.hungerStatusEffects[3].effectName)
            {
                RemoveStatusEffect(Database.instance.hungerStatusEffects[3]);
                break;
            }
        }
    }

    public void RemovePreviousThirstStatusEffect()
    {
        for (int i = 0; i < statusEffectsIconsParent.childCount; i++)
        {
            if (statusEffectsIconsParent.GetChild(i).name == Database.instance.thirstStatusEffects[0].effectName)
            {
                RemoveStatusEffect(Database.instance.thirstStatusEffects[0]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.thirstStatusEffects[1].effectName)
            {
                RemoveStatusEffect(Database.instance.thirstStatusEffects[1]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.thirstStatusEffects[2].effectName)
            {
                RemoveStatusEffect(Database.instance.thirstStatusEffects[2]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.thirstStatusEffects[3].effectName)
            {
                RemoveStatusEffect(Database.instance.thirstStatusEffects[3]);
                break;
            }
        }
    }

    public void RemovePreviousTirednessStatusEffect()
    {
        for (int i = 0; i < statusEffectsIconsParent.childCount; i++)
        {
            if (statusEffectsIconsParent.GetChild(i).name == Database.instance.tirednessStatusEffects[0].effectName)
            {
                RemoveStatusEffect(Database.instance.tirednessStatusEffects[0]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.tirednessStatusEffects[1].effectName)
            {
                RemoveStatusEffect(Database.instance.tirednessStatusEffects[1]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.tirednessStatusEffects[2].effectName)
            {
                RemoveStatusEffect(Database.instance.tirednessStatusEffects[2]);
                break;
            }
            else if (statusEffectsIconsParent.GetChild(i).name == Database.instance.tirednessStatusEffects[3].effectName)
            {
                RemoveStatusEffect(Database.instance.tirednessStatusEffects[3]);
                break;
            }
        }
    }
}
