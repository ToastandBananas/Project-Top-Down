using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextFade[] floatingTexts;
    public int floatingTextIndex = 0;

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
    }
}
