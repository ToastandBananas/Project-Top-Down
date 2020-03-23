using UnityEngine.UI;
using UnityEngine;

public enum StatType { HEALTH, MANA, STAMINA, EXPERIENCE }

public class PlayerStatBar : MonoBehaviour
{
    public StatType thisStatType;

    Slider slider;
    RectTransform rectTransform;
    Transform bar;
    Image barSprite;
    RectTransform barSpriteRect;
    BasicStats playerStats;

    void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        playerStats = PlayerMovement.instance.GetComponent<BasicStats>();

        ChangeBar();
    }

    public void ChangeBar()
    {
        switch (thisStatType)
        {
            case StatType.HEALTH:
                SetBarSize(playerStats.maxHealth);
                SetBarFill(playerStats.health);
                break;
            case StatType.MANA:
                SetBarSize(playerStats.maxMana);
                SetBarFill(playerStats.mana);
                break;
            case StatType.STAMINA:
                SetBarSize(playerStats.maxStamina);
                SetBarFill(playerStats.stamina);
                break;
            case StatType.EXPERIENCE:
                Debug.Log("Experience bar system still needs created");
                break;
        }
    }

    public void SetBarFill(float statValue)
    {
        slider.value = statValue;
    }

    public void SetColor(Color color)
    {
        barSprite.color = color;
    } 

    public void SetBarSize(float maxStatValue)
    {
        slider.maxValue = maxStatValue;
        rectTransform.sizeDelta = new Vector2(maxStatValue * 2, 28);
    }
}
