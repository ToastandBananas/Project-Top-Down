using System.Collections;
using UnityEngine;

public class Needs : MonoBehaviour
{
    [Header("Needs")]
    public int nourishment = 100;
    public int hydration = 100;
    public int energy = 100;

    [Header("Needs Drain Times (in minutes)")]
    public float nourishmentDrainRate = 2.5f;
    public float hydrationDrainRate = 1.5f;
    public float energyDrainRate = 5f;

    int hungerLevel, thirstLevel, tirednessLevel;

    Database database;
    Status status;

    #region Singleton
    public static Needs instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of Needs found!");
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        database = Database.instance;
        status = GetComponent<Status>();

        StartCoroutine(DrainNourishment());
        StartCoroutine(DrainHydration());
        StartCoroutine(DrainEnergy());
    }

    public void ReplenishNourishment(int amount)
    {
        if (hungerLevel > 0 && (nourishment == 0 || (nourishment <= 25 && nourishment + amount > 25) || (nourishment <= 50 && nourishment + amount > 50)
            || (nourishment <= 75 && nourishment + amount > 75)))
        {
            status.RemovePreviousHungerStatusEffect();
            nourishment += amount;
            if (nourishment <= 25 && hungerLevel != 3)
            {
                status.AddStatusEffect(database.hungerStatusEffects[2], false);
                hungerLevel = 3;
            }
            else if (nourishment <= 50 && hungerLevel != 2)
            {
                status.AddStatusEffect(database.hungerStatusEffects[1], false);
                hungerLevel = 2;
            }
            else if (nourishment <= 75 && hungerLevel != 1)
            {
                status.AddStatusEffect(database.hungerStatusEffects[0], false);
                hungerLevel = 1;
            }
        }
        else
        {
            nourishment += amount;
            if (nourishment > 75 && hungerLevel != 0)
                hungerLevel = 0;
        }
    }

    public void ReplenishHydration(int amount)
    {
        if (thirstLevel > 0 && (hydration == 0 || (hydration <= 25 && hydration + amount > 25) || (hydration <= 50 && hydration + amount > 50)
            || (hydration <= 75 && hydration + amount > 75)))
        {
            status.RemovePreviousThirstStatusEffect();
            hydration += amount;
            if (hydration <= 25 && thirstLevel != 3)
            {
                status.AddStatusEffect(database.thirstStatusEffects[2], false);
                thirstLevel = 3;
            }
            else if (hydration <= 50 && thirstLevel != 2)
            {
                status.AddStatusEffect(database.thirstStatusEffects[1], false);
                thirstLevel = 2;
            }
            else if (hydration <= 75 && thirstLevel != 1)
            {
                status.AddStatusEffect(database.thirstStatusEffects[0], false);
                thirstLevel = 1;
            }
        }
        else
        {
            hydration += amount;
            if (hydration > 75 && thirstLevel != 0)
                thirstLevel = 0;
        }
    }

    public void ReplenishEnergy(int amount)
    {
        if (tirednessLevel > 0 && (energy == 0 || (energy <= 25 && energy + amount > 25) || (energy <= 50 && energy + amount > 50)
            || (energy <= 75 && energy + amount > 75)))
        {
            status.RemovePreviousTirednessStatusEffect();
            energy += amount;
            if (energy <= 25 && tirednessLevel != 3)
            {
                status.AddStatusEffect(database.tirednessStatusEffects[2], false);
                tirednessLevel = 3;
            }
            else if (energy <= 50 && tirednessLevel != 2)
            {
                status.AddStatusEffect(database.tirednessStatusEffects[1], false);
                tirednessLevel = 2;
            }
            else if (energy <= 75 && tirednessLevel != 1)
            {
                status.AddStatusEffect(database.tirednessStatusEffects[0], false);
                tirednessLevel = 1;
            }
        }
        else
        {
            energy += amount;
            if (energy > 75 && tirednessLevel != 0)
                tirednessLevel = 0;
        }
    }

    IEnumerator DrainNourishment()
    {
        while (true)
        {
            yield return new WaitForSeconds(nourishmentDrainRate * 60f);
            if (nourishment > 1)
            {
                nourishment--;
                if (nourishment <= 25 && hungerLevel < 3)
                {
                    status.RemovePreviousHungerStatusEffect();
                    status.AddStatusEffect(database.hungerStatusEffects[2], true);
                    hungerLevel = 3;
                }
                else if (nourishment <= 50 && hungerLevel < 2)
                {
                    status.RemovePreviousHungerStatusEffect();
                    status.AddStatusEffect(database.hungerStatusEffects[1], true);
                    hungerLevel = 2;
                }
                else if (nourishment <= 75 && hungerLevel < 1)
                {
                    status.AddStatusEffect(database.hungerStatusEffects[0], true);
                    hungerLevel = 1;
                }
                else if (nourishment > 75 && hungerLevel != 0)
                    hungerLevel = 0;
            }
            else if (nourishment == 1)
            {
                nourishment--;
                status.RemovePreviousHungerStatusEffect();
                status.AddStatusEffect(database.hungerStatusEffects[3], true);
                hungerLevel = 4;
            }
        }
    }

    IEnumerator DrainHydration()
    {
        while (true)
        {
            yield return new WaitForSeconds(hydrationDrainRate * 60f);
            if (hydration > 1)
            {
                hydration--;
                if (hydration <= 25 && thirstLevel < 3)
                {
                    status.RemovePreviousThirstStatusEffect();
                    status.AddStatusEffect(database.thirstStatusEffects[2], true);
                    thirstLevel = 3;
                }
                else if (hydration <= 50 && thirstLevel < 2)
                {
                    status.RemovePreviousThirstStatusEffect();
                    status.AddStatusEffect(database.thirstStatusEffects[1], true);
                    thirstLevel = 2;
                }
                else if (hydration <= 75 && thirstLevel < 1)
                {
                    status.AddStatusEffect(database.thirstStatusEffects[0], true);
                    thirstLevel = 1;
                }
                else if (hydration > 75 && thirstLevel != 0)
                    thirstLevel = 0;
            }
            else if (hydration == 1)
            {
                hydration--;
                status.RemovePreviousThirstStatusEffect();
                status.AddStatusEffect(database.thirstStatusEffects[3], true);
                thirstLevel = 4;
            }
        }
    }

    IEnumerator DrainEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(energyDrainRate * 60f);
            if (energy > 1)
            {
                energy--;
                if (energy <= 25 && tirednessLevel < 3)
                {
                    status.RemovePreviousThirstStatusEffect();
                    status.AddStatusEffect(database.tirednessStatusEffects[2], true);
                    tirednessLevel = 3;
                }
                else if (energy <= 50 && tirednessLevel < 2)
                {
                    status.RemovePreviousThirstStatusEffect();
                    status.AddStatusEffect(database.tirednessStatusEffects[1], true);
                    tirednessLevel = 2;
                }
                else if (energy <= 75 && tirednessLevel < 1)
                {
                    status.AddStatusEffect(database.tirednessStatusEffects[0], true);
                    tirednessLevel = 1;
                }
                else if (energy > 75 && tirednessLevel != 0)
                    tirednessLevel = 0;
            }
            else if (energy == 1)
            {
                energy--;
                status.RemovePreviousThirstStatusEffect();
                status.AddStatusEffect(database.tirednessStatusEffects[3], true);
                tirednessLevel = 4;
            }
        }
    }
}
