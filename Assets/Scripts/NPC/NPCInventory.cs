using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : MonoBehaviour
{
    [Header("Randomization")]
    public bool randomizeItems = false;
    public bool randomizeGold = true;
    public int percentChangeNoGold = 20;
    public int minGoldPossible = 1;
    public int maxGoldPossible = 10;

    [Header("Inventory")]
    public int gold;
    public List<GameObject> carriedItems = new List<GameObject>();

    BasicStats basicStats;

    void Start()
    {
        basicStats = GetComponent<BasicStats>();

        if (randomizeItems)
            RandomizeItems();

        if (randomizeGold)
            RandomizeGold();

        basicStats.gold = gold;
    }

    void RandomizeItems()
    {
        Debug.Log("TODO: Implement RandomizeItems() code");
    }

    void RandomizeGold()
    {
        int randomNum = Random.Range(1, 101);
        if (randomNum <= percentChangeNoGold)
            gold = 0;
        else
            gold = Random.Range(minGoldPossible, maxGoldPossible + 1);
    }

    public IEnumerator TransferObjectsToBodyContainer(Container deadBodyContainer)
    {
        yield return new WaitForSeconds(0.05f);

        foreach (GameObject obj in carriedItems)
        {
            if (obj != null)
                deadBodyContainer.containerObjects.Add(obj);
        }

        deadBodyContainer.randomizeGold = false;
        deadBodyContainer.gold = gold;
        deadBodyContainer.InitializeData();
    }
}
