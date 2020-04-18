using UnityEngine;
using UnityEngine.UI;

public class ButtonHints : MonoBehaviour
{
    [Header("PC")]
    public Sprite leftClick;
    public Sprite rightClick, middleClick, c, i, g, enter, backspace, pcLeftArrow, pcRightArrow;

    [Header("PS4")]
    public Sprite PS4Arrows;
    public Sprite r1, r2, PS4LeftThumbstick, cross, triangle, square, circle, PS4LeftArrow, PS4RightArrow;

    [Header("Xbox One")]
    public Sprite xboxArrows;
    public Sprite rb, rt, xboxLeftThumbstick, y, b, a, x, xboxLeftArrow, xboxRightArrow;

    [Header("Inventory Hint Images")]
    public Image moveItem;
    public Image use, context, drop, closeMenus;

    [Header("Container Hint Images")]
    public Image takeItem;
    public Image takeAll, takeGold;

    [Header("Equipment Menu Hint Images")]
    public Image closeEquipmentMenu;

    [Header("Quantity Menu Hint Images")]
    public Image decrease;
    public Image increase, submit, cancel;

    GameManager gm;

    #region Singleton
    public static ButtonHints instance;

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
        gm = GameManager.instance;
    }

    public void SetButtonHints()
    {
        if (gm.isUsingController)
        {
            // Inventory
            moveItem.sprite = cross;
            use.sprite = r1;
            context.sprite = square;
            drop.transform.parent.gameObject.SetActive(true);
            drop.sprite = triangle;
            closeMenus.sprite = circle;

            // Container
            takeItem.transform.parent.gameObject.SetActive(true);
            takeItem.sprite = triangle;
            takeAll.sprite = PS4LeftThumbstick;
            takeGold.sprite = r2;

            // Equipment Menu
            closeEquipmentMenu.transform.parent.gameObject.SetActive(false);
        }
        else // PC
        {
            // Inventory
            moveItem.sprite = leftClick;
            use.sprite = middleClick;
            context.sprite = rightClick;
            drop.transform.parent.gameObject.SetActive(false);
            closeMenus.sprite = i;

            // Container
            takeItem.transform.parent.gameObject.SetActive(false);
            takeAll.sprite = enter;
            takeGold.sprite = g;

            // Equipment Menu
            closeEquipmentMenu.transform.parent.gameObject.SetActive(true);
            closeEquipmentMenu.sprite = c;
        }
    }
}
