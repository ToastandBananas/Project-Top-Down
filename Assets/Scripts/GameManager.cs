using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using CodeMonkey.Utils;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        /*FunctionPeriodic.Create(() =>
        {
            if (health > 0)
            {
                health -= 1f;
                playerHealthBar.SetSize(health / maxHealth);
            }
        }, 0.3f);*/
    }
}
