using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public TextFade[] floatingTexts;
    [HideInInspector] public int floatingTextIndex = 0;
    
    public bool menuOpen;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button loadButton;
    public Button saveButton;
    public Button saveAndQuitButton;

    [Header("Controller")]
    public bool isUsingController;

    [Header("Interactable")]
    public Interactable currentlySelectedInteractable;

    [HideInInspector] public StandardUIQuestLogWindow questLog;
    [HideInInspector] public StandardDialogueUI dialogueUI;

    InventoryUI invUI;
    Inventory inv;
    UIControllerNavigation UIControllerNav;
    ButtonHints buttonHints;
    Transform dialogueManager;

    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("FIXME: More than one instance of Inventory found!");
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        floatingTexts = GameObject.Find("Floating Texts").GetComponentsInChildren<TextFade>();
        invUI = InventoryUI.instance;
        inv = Inventory.instance;
        UIControllerNav = UIControllerNavigation.instance;
        buttonHints = ButtonHints.instance;

        dialogueManager = GameObject.Find("Dialogue Manager").transform;
        dialogueUI = dialogueManager.GetChild(0).GetComponentInChildren<StandardDialogueUI>();
        questLog = dialogueManager.GetChild(0).GetComponentInChildren<StandardUIQuestLogWindow>();

        if (invUI.inventoryMenu.activeSelf == true)
            invUI.ToggleInventory();

        if (invUI.playerEquipmentMenu.activeSelf == true)
            invUI.ToggleEquipmentMenu();

        if (invUI.containerMenu.activeSelf == true)
            invUI.ToggleContainerMenu();
    }

    void Update()
    {
        if (GameControls.gamePlayActions.menuPause.WasPressed)
            TogglePauseMenu();

        if (GameControls.gamePlayActions.playerJournal.WasPressed && invUI.currentlySelectedItem == null && pauseMenu.activeSelf == false && dialogueUI.isOpen == false)
            ToggleQuestLog();

        if (Input.GetKeyDown(KeyCode.P))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Detect if using controller or mouse/keyboard
        DetectController();
    }

    public void TogglePauseMenu()
    {
        StartCoroutine(TogglePauseMenuWithDelay());
    }

    public IEnumerator TogglePauseMenuWithDelay()
    {
        if (invUI.currentlySelectedItem != null && pauseMenu.activeSelf == false)
        {
            if (invUI.invSlotMovingFrom != null)
            {
                invUI.invSlotMovingFrom.contextMenu.DropItem();
                invUI.invSlotMovingFrom.contextMenu.DisableContextMenu();
            }
            else if (invUI.equipSlotMovingFrom != null)
            {
                invUI.equipSlotMovingFrom.contextMenu.DropItem();
                invUI.equipSlotMovingFrom.contextMenu.DisableContextMenu();
            }

            invUI.StopDraggingInvItem();
            yield return new WaitForSeconds(0.15f);
        }

        TurnOffMenus();

        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf)
        {
            UIControllerNav.ClearCurrentlySelected();
            UIControllerNav.currentlySelectedObject = resumeButton.gameObject;
            UIControllerNav.currentlySelectedButton = resumeButton;
            resumeButton.OnSelect(null);
            resumeButton.Select();
        }
        else
            UIControllerNav.ClearCurrentlySelected();

        invUI.DetermineIfMenuOpen();
    }

    public void TurnOffMenus()
    {
        if (questLog.isOpen)
            questLog.Close();
        if (invUI.containerMenu.activeSelf)
            invUI.ToggleContainerMenu();
        if (invUI.inventoryMenu.activeSelf)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenu.activeSelf)
            invUI.ToggleEquipmentMenu();

        invUI.TurnOffHighlighting();
        invUI.ClearAllTooltips();
        UIControllerNav.DisableContextMenu();
        UIControllerNav.ClearCurrentlySelected();
    }

    public void ToggleQuestLog()
    {
        if (invUI.inventoryMenu.activeSelf)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenu.activeSelf)
            invUI.ToggleEquipmentMenu();
        if (invUI.containerMenu.activeSelf)
            invUI.ToggleContainerMenu();

        questLog.Toggle();
        invUI.DetermineIfMenuOpen();
    }

    void DetectController()
    {
        if (isUsingController
            && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || GameControls.gamePlayActions.ActiveDevice.Name == "None"))
        {
            isUsingController = false;

            Cursor.visible = true;

            if (invUI.currentlySelectedItem != null)
                UIControllerNav.RemoveHighlightFromItem();

            UIControllerNav.ClearCurrentlySelected();

            buttonHints.SetButtonHints();
        }
        else if (isUsingController == false
            && GameControls.gamePlayActions.ActiveDevice.Name != "None" && (GameControls.gamePlayActions.ActiveDevice.AnyButton.IsPressed
            || GameControls.gamePlayActions.ActiveDevice.Direction.HasChanged || GameControls.gamePlayActions.ActiveDevice.LeftTrigger.IsPressed
            || GameControls.gamePlayActions.ActiveDevice.RightTrigger.IsPressed || GameControls.gamePlayActions.ActiveDevice.LeftBumper.IsPressed
            || GameControls.gamePlayActions.ActiveDevice.RightBumper.IsPressed || GameControls.gamePlayActions.ActiveDevice.CommandIsPressed
            || GameControls.gamePlayActions.ActiveDevice.LeftStickButton.IsPressed || GameControls.gamePlayActions.ActiveDevice.RightStickButton.IsPressed))
        {
            isUsingController = true;

            Cursor.visible = false;

            if (invUI.currentlySelectedItem != null)
                invUI.TurnOffHighlighting();

            if (invUI.contextMenu.transform.childCount > 0)
                StartCoroutine(UIControllerNav.SelectButton(invUI.contextMenu.GetChild(0).GetComponent<Button>()));
            else if (invUI.inventoryMenu.activeSelf)
                UIControllerNav.FocusOnInvSlot(inv.GetSlotByCoordinates(Vector2.one, invUI.pocketsSlots), 0, 0);
            else if (invUI.playerEquipmentMenu.activeSelf)
                UIControllerNav.FocusOnEquipSlot(invUI.weaponSlots[(int)WeaponSlot.WeaponRight]);
            else if (pauseMenu.activeSelf)
                StartCoroutine(UIControllerNav.SelectButton(resumeButton));

            if (invUI.currentlySelectedItem != null)
            {
                UIControllerNav.SetIconPosition();
                UIControllerNav.HighlightItem();
            }

            buttonHints.SetButtonHints();
        }
    }
}
