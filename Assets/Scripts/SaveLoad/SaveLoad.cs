using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public bool isLoading;

    public GameObject weaponsParent;
    public GameObject equipmentParent;

    ES3AutoSaveMgr autoSaveManager;
    GameManager gm;
    Inventory inv;
    InventoryUI invUI;
    EquipmentManager playerEquipmentManager;
    GameObject NPCsParent;
    GameObject looseItemsParent;
    GameObject containersParent;
    GameObject doorsParent;

    public string charactersSaveFileName = "Player.es3";
    string autoSaveFileName;

    string autosaveSceneName;

    #region Singleton
    public static SaveLoad instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    #endregion

    void Start()
    {
        autoSaveFileName = "AutoSave_" + charactersSaveFileName;

        autoSaveManager = FindObjectOfType<ES3AutoSaveMgr>();
        gm = GameManager.instance;
        inv = Inventory.instance;
        invUI = InventoryUI.instance;
        playerEquipmentManager = GameObject.Find("Player").GetComponent<EquipmentManager>();
        NPCsParent = GameObject.Find("NPCs");
        looseItemsParent = GameObject.Find("Loose Items");
        containersParent = GameObject.Find("Containers");
        doorsParent = GameObject.Find("Doors");
    }

    public void SaveAndQuit()
    {
        autoSaveManager.Save(autoSaveFileName);

        StartCoroutine(Quit());
    }

    public void AutoSave()
    {
        autosaveSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Autosaving Game - Scene Name: " + autosaveSceneName);

        ES3.Save<string>("autosaveSceneName", autosaveSceneName, "AutoSaveSceneName.es3");

        autoSaveManager.Save(autoSaveFileName);
    }

    #region Modified Save() backup (a function in the autoSaveManager script)
    /*public void Save(string filePath = null)
	{
		if(autoSaves == null || autoSaves.Count == 0)
			return;

		var gameObjects = new GameObject[autoSaves.Count];
		for (int i = 0; i < autoSaves.Count; i++) 
			gameObjects [i] = autoSaves [i].gameObject;

        if (filePath == null || filePath == "")
		    ES3.Save<GameObject[]>(key, gameObjects, settings);
        else
            ES3.Save<GameObject[]>(key, gameObjects, filePath, settings);
    }*/
    #endregion

    public void Save()
    {
        // Turn the Pause Menu off
        gm.TogglePauseMenu();

        autosaveSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Saving Game - Scene Name: " + autosaveSceneName);

        ES3.Save<string>("autosaveSceneName", autosaveSceneName, "AutoSaveSceneName.es3");

        // Save all objects with the autosave component on them (also, if they involve prefabs, they need to be enabled for Easy Save)
        //autoSaveManager.Save(filePath);
        autoSaveManager.Save(autoSaveFileName);
    }

    public void Load()
    {
        Debug.Log("Loading Game");

        isLoading = true;

        // Turn the Pause Menu off
        gm.TogglePauseMenu();

        autosaveSceneName = ES3.Load<string>("autosaveSceneName", "AutoSaveSceneName.es3");

        if (autosaveSceneName != SceneManager.GetActiveScene().name)
            SceneManager.LoadScene(autosaveSceneName);

        // Destroy all inventory slots before loading in the ones from the save file...
        // (We do this because since we Instantiate the slots when starting up the game, the load function won't 
        //      recognize the current inv slots in our scene when starting a new game instance).

        // Destroy Pocket Slots
        if (invUI.pocketsParent.childCount > 0)
        {
            invUI.pocketsSlots.Clear();
            for (int i = 0; i < inv.pocketsSlotCount; i++)
            {
                Destroy(invUI.pocketsParent.GetChild(i).gameObject);
            }
        }

        // Destroy Bag Slots
        if (invUI.bagParent.childCount > 0)
        {
            invUI.bagSlots.Clear();
            for (int i = 0; i < inv.bagSlotCount; i++)
            {
                Destroy(invUI.bagParent.GetChild(i).gameObject);
            }
        }

        // Destroy Horse Bag Slots
        if (invUI.horseBagParent.childCount > 0)
        {
            invUI.horseBagSlots.Clear();
            for (int i = 0; i < inv.horseBagSlotCount; i++)
            {
                Destroy(invUI.horseBagParent.GetChild(i).gameObject);
            }
        }

        // Destroy Container Slots
        if (invUI.currentlyActiveContainer != null)
        {
            invUI.containerSlots.Clear();
            for (int i = 0; i < invUI.currentlyActiveContainer.slotCount; i++)
            {
                Destroy(invUI.containerParent.GetChild(i).gameObject);
            }
            invUI.currentlyActiveContainer = null;
        }

        // Clear out the weapon slots
        foreach(EquipSlot slot in invUI.weaponSlots)
        {
            if (slot.isEmpty == false)
                slot.ClearSlot(slot);
        }

        // Clear out the equipment slots
        foreach (EquipSlot slot in invUI.equipSlots)
        {
            if (slot.isEmpty == false)
                slot.ClearSlot(slot);
        }

        // Destroy any weapons equipped in the player's arms
        if (playerEquipmentManager.leftWeaponParent.childCount > 0)
            Destroy(playerEquipmentManager.leftWeaponParent.GetChild(0).gameObject);
        if (playerEquipmentManager.rightWeaponParent.childCount > 0)
            Destroy(playerEquipmentManager.rightWeaponParent.GetChild(0).gameObject);

        // Destroy Temp Slot
        if (invUI.tempSlot != null)
            Destroy(invUI.tempSlot.gameObject);

        // Destroy current Loose Items in the scene, just in case there's more loose items in the scene than when we saved
        for (int i = 0; i < looseItemsParent.transform.childCount; i++)
        {
            Destroy(looseItemsParent.transform.GetChild(i).gameObject);
        }
        
        // Destroy current NPCs in the scene, just in case there's more NPC's in the scene than when we saved
        for (int i = 0; i < NPCsParent.transform.childCount; i++)
        {
            Destroy(NPCsParent.transform.GetChild(i).gameObject);
        }

        // Load game data and instantiate necessary GameObjects
        StartCoroutine(LoadGame());

        // Recalculate any necessary slot data
        StartCoroutine(SetSlots());

        // Set anims for player and NPC arms (so they know if they have a weapon equipped or not)
        StartCoroutine(SetArmAnims());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.1f);
        autoSaveManager.Load(autoSaveFileName);
        isLoading = false;

        /*for (int i = 0; i < NPCsParent.transform.childCount; i++)
        {
            if (NPCsParent.transform.GetChild(i).childCount > 6)
            {
                for (int j = 0; j < NPCsParent.transform.GetChild(i).childCount - 6; j++)
                {
                    if (NPCsParent.transform.GetChild(i).GetChild(j + 6) != null)
                        Destroy(NPCsParent.transform.GetChild(i).GetChild(j + 6).gameObject);
                }
            }
        }*/
    }

    IEnumerator SetSlots()
    {
        yield return new WaitForSeconds(0.5f);

        invUI.SetInventorySlotLists();
        invUI.tempSlot = GameObject.Find("Temp Slot").GetComponent<InventorySlot>();

        // Each inventory slot and icon in the game needs a few changes at this point
        foreach (InventorySlot slot in invUI.pocketsSlots)
        {
            if (slot.isEmpty == false && slot.item != null && slot.parentSlot == null)
                RecalculateSlotData(slot, invUI.pocketsSlots);
        }

        foreach (InventorySlot slot in invUI.bagSlots)
        {
            if (slot.isEmpty == false && slot.parentSlot == null)
                RecalculateSlotData(slot, invUI.bagSlots);
        }

        foreach (InventorySlot slot in invUI.horseBagSlots)
        {
            if (slot.isEmpty == false && slot.parentSlot == null)
                RecalculateSlotData(slot, invUI.horseBagSlots);
        }

        foreach (InventorySlot slot in invUI.containerSlots)
        {
            if (slot.isEmpty == false && slot.parentSlot == null)
                RecalculateSlotData(slot, invUI.containerSlots);
        }

        // Equipment slots will need to be set to the appropriate color
        foreach (EquipSlot slot in invUI.weaponSlots)
        {
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }

        foreach (EquipSlot slot in invUI.equipSlots)
        {
            if (slot.isEmpty == false)
                slot.slotBackgroundImage.sprite = slot.fullSlotSprite;
        }
    }

    void RecalculateSlotData(InventorySlot slot, List<InventorySlot> invSlots)
    {
        // Set the parent and child slots
        inv.SetParentAndChildSlots(slot.item, slot, invSlots);

        // Find the slot's icon
        slot.iconImage = slot.GetBottomRightChildSlot(slot.item, slot).transform.GetChild(2).GetComponent<Image>();

        // Find the slot item's ItemData
        slot.itemData = slot.iconImage.GetComponent<ItemData>();

        // Setup the icon's sprite
        slot.iconImage.sprite = slot.item.inventoryIcon;
        slot.iconImage.preserveAspect = true;

        // Reposition/resize the icon
        RectTransform iconRectTransform = slot.iconImage.GetComponent<RectTransform>();
        iconRectTransform.localPosition = inv.GetItemInvPosition(slot.item);
        iconRectTransform.localScale = new Vector3(67.5f, 67.5f, 67.5f);
    }

    IEnumerator SetArmAnims()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Arms arm in FindObjectsOfType<Arms>())
        {
            StartCoroutine(arm.SetArmAnims(0));
        }
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
}
