using UnityEngine.UI;
using UnityEngine;

public enum StatType { HEALTH, MANA, STAMINA, EXPERIENCE }

public class PlayerStatBar : MonoBehaviour
{
    public StatType thisStatType;

    RectTransform backgroundRect;
    Transform bar;
    Image barSprite;
    RectTransform barSpriteRect;
    BasicStats playerStats;

    void Awake()
    {
        backgroundRect = transform.Find("Background").GetComponent<RectTransform>();
        bar = transform.Find("Bar");
        barSprite = bar.Find("Bar Sprite").GetComponent<Image>();
        barSpriteRect = barSprite.GetComponent<RectTransform>();
    }

    void Start()
    {
        playerStats = FindObjectOfType<PlayerMovement>().GetComponent<BasicStats>();

        ChangeBar();
    }

    public void ChangeBar()
    {
        switch (thisStatType)
        {
            case StatType.HEALTH:
                SetBarSize(playerStats.maxHealth);
                SetBarFill(playerStats.health / playerStats.maxHealth);
                break;
            case StatType.MANA:
                SetBarSize(playerStats.maxMana);
                SetBarFill(playerStats.mana / playerStats.maxMana);
                break;
            case StatType.STAMINA:
                SetBarSize(playerStats.maxStamina);
                SetBarFill(playerStats.stamina / playerStats.maxStamina);
                break;
            case StatType.EXPERIENCE:
                Debug.Log("Experience bar system still needs created");
                break;
        }
    }

    public void SetBarFill(float sizeNormalized)
    {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }

    public void SetColor(Color color)
    {
        barSprite.color = color;
    } 

    public void SetBarSize(float maxStatValue)
    {
        backgroundRect.sizeDelta = new Vector2(maxStatValue * 2, 20);
        barSpriteRect.sizeDelta  = new Vector2(maxStatValue * 2, 20);
        barSpriteRect.anchoredPosition = new Vector2(maxStatValue * 0.2f, 0);
    }
}
