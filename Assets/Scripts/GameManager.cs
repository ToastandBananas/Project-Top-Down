using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextFade[] floatingTexts;
    public int floatingTextIndex = 0;

    InventoryUI invUI;

    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of Inventory found!");
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }
    #endregion

    void Start()
    {
        floatingTexts = GameObject.Find("Floating Texts").GetComponentsInChildren<TextFade>();
        invUI = InventoryUI.instance;

        if (invUI.inventoryGO.activeSelf == true)
            invUI.ToggleInventory();

        if (invUI.playerEquipmentMenuGO.activeSelf == true)
            invUI.ToggleEquipmentMenu();

        //if (invUI.containerMenuGO.activeSelf == true)
            //invUI.ToggleContainerMenu();
    }
}
