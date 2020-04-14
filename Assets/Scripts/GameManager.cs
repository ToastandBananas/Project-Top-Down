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
    UIControllerNavigation UIControllerNav;

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
        UIControllerNav = UIControllerNavigation.instance;

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
    }
}
