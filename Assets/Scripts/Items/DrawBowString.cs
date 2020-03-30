using UnityEngine;

public class DrawBowString : MonoBehaviour
{
    Transform leftStringEnd, rightStringEnd;
    LineRenderer leftLine, rightLine;
    Arms arms;
        
    void Start()
    {
        leftStringEnd = transform.Find("Left String End");
        rightStringEnd = transform.Find("Right String End");
        leftLine = leftStringEnd.GetComponent<LineRenderer>();
        rightLine = rightStringEnd.GetComponent<LineRenderer>();
    }
    
    void Update()
    {
        DrawString(rightStringEnd.position, leftStringEnd.position);
    }
    
    public void DrawString(Vector3 endPosForLeftString, Vector3 endPosForRightString)
    {
        leftLine.SetPosition(0, leftStringEnd.position);
        leftLine.SetPosition(1, endPosForLeftString);
        rightLine.SetPosition(0, rightStringEnd.position);
        rightLine.SetPosition(1, endPosForRightString);
    }
}
