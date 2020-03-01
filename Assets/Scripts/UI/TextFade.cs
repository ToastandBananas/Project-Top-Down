using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{
    public Text textComponent;
    Color originalColor = Color.white;

    float timer;
    float fadeOutSpeed = 0.005f;
    float moveSpeed = 0.8f;
    float fadeTime = 2f;

    int activeTexts = 0;

    GameManager GM;

    void Start()
    {
        GM = GameManager.instance;
    }

    public void DisplayUseItemFloatingText(Item item, Transform target, bool usedSuccessfully)
    {
        if (usedSuccessfully)
            textComponent.text = item.onUseItemText;
        else
            textComponent.text = item.onFailedToUseItemText;

        transform.position = target.position + (Vector3.up * 0.75f);

        SetFloatingTextIndex();

        StartCoroutine(MoveAndFade());
    }

    public void DisplayFloatingText(Transform target, string textToDisplay)
    {
        textComponent.text = textToDisplay;
        transform.position = target.position + (Vector3.up * 0.75f);

        SetFloatingTextIndex();

        StartCoroutine(SlowFade());
    }

    void SetFloatingTextIndex()
    {
        GM.floatingTextIndex++;
        if (GM.floatingTextIndex >= 5)
            GM.floatingTextIndex = 0;
    }

    public IEnumerator SlowFade()
    {
        timer = 0;
        textComponent.color = originalColor;

        while (true)
        {
            textComponent.color -= new Color(0, 0, 0, fadeOutSpeed / 2);

            if (timer >= fadeTime)
                break;
            else
                timer += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator MoveAndFade()
    {
        timer = 0;
        textComponent.color = originalColor;

        while (true)
        {
            transform.position += Vector3.up * Time.deltaTime * moveSpeed;
            textComponent.color -= new Color(0, 0, 0, fadeOutSpeed);

            if (timer >= fadeTime)
                break;
            else
                timer += Time.deltaTime;

            yield return null;
        }
    }
}
