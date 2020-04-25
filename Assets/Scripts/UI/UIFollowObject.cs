using UnityEngine;

public class UIFollowObject : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRectTransform;
    RectTransform UI;
    Transform follow;
    public Vector2 offset;

    Vector3 screenPos;
    Vector2 canvasPos;

    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        UI = canvas.transform.GetChild(0).GetComponent<RectTransform>();
        follow = transform.parent;
    }

    void LateUpdate ()
    {
        screenPos = Camera.main.WorldToScreenPoint(follow.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, null, out canvasPos);
        UI.anchoredPosition = canvasPos + offset;
    }
}
