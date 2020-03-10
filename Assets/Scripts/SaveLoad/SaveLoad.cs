using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    ES3AutoSaveMgr autoSaveManager;
    GameManager gm;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        autoSaveManager = FindObjectOfType<ES3AutoSaveMgr>();
        gm = GameManager.instance;
    }

    public void Save()
    {
        autoSaveManager.Save();
        gm.TogglePauseMenu();
    }

    public void Load()
    {
        autoSaveManager.Load();
        gm.TogglePauseMenu();

        foreach(Arms arm in FindObjectsOfType<Arms>())
        {
            arm.SetArmAnims();
        }
    }
}
