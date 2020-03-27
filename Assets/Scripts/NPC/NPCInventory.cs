using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : MonoBehaviour
{
    public bool randomizeItems = false;

    public List<GameObject> carriedItems = new List<GameObject>();

    void Start()
    {
        if (randomizeItems)
            RandomizeItems();
    }

    void RandomizeItems()
    {
        Debug.Log("TODO: Implement RandomizeItems() code");
    }

    public IEnumerator TransferObjectsToBodyContainer(Container deadBodyContainer)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (GameObject obj in carriedItems)
        {
            if (obj != null)
                deadBodyContainer.containerObjects.Add(obj);
        }

        deadBodyContainer.InitializeData();
    }
}
