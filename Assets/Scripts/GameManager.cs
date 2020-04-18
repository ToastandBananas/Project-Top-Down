using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public bool isUsingController;

    InventoryUI invUI;
    Inventory inv;
    UIControllerNavigation UIControllerNav;
    ButtonHints buttonHints;

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
        {
            TurnOffMenus();
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.P))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Detect if using controller or mouse/keyboard
        DetectController();
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf)
        {
            menuOpen = true;
            UIControllerNav.ClearCurrentlySelected();
            UIControllerNav.currentlySelectedObject = resumeButton.gameObject;
            UIControllerNav.currentlySelectedButton = resumeButton;
            resumeButton.OnSelect(null);
            resumeButton.Select();
        }
        else
        {
            menuOpen = false;
            UIControllerNav.ClearCurrentlySelected();
        }
    }

    public void TurnOffMenus()
    {
        if (invUI.containerMenu.activeSelf == true)
            invUI.ToggleContainerMenu();
        if (invUI.inventoryMenu.activeSelf == true)
            invUI.ToggleInventory();
        if (invUI.playerEquipmentMenu.activeSelf == true)
            invUI.ToggleEquipmentMenu();

        invUI.TurnOffHighlighting();
        invUI.ClearAllTooltips();
        UIControllerNav.DisableContextMenu();
        UIControllerNav.ClearCurrentlySelected();
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
