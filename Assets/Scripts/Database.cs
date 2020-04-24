using UnityEngine;

public class Database : MonoBehaviour
{
    [Header("Status Effects")]
    public StatusEffect[] hungerStatusEffects;
    public StatusEffect[] thirstStatusEffects;
    public StatusEffect[] tirednessStatusEffects;

    #region Singleton
    public static Database instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of Database found!");
            Destroy(gameObject);
        }
    }
    #endregion
}
