using UnityEngine;

public class DrawBowString : MonoBehaviour
{
    [HideInInspector] public Vector3 middleOfStringOriginalPosition;

    Transform leftStringEnd, middleOfString, rightStringEnd;
    LineRenderer leftLine, rightLine;
    Arms arms;
        
    void Start()
    {
        leftStringEnd = transform.Find("Left String End");
        middleOfString = transform.Find("Middle of String");
        rightStringEnd = transform.Find("Right String End");
        leftLine = leftStringEnd.GetComponent<LineRenderer>();
        rightLine = rightStringEnd.GetComponent<LineRenderer>();

        middleOfStringOriginalPosition = middleOfString.localPosition;
    }
    
    void FixedUpdate()
    {
        DrawString();
    }
    
    public void DrawString()
    {
        leftLine.SetPosition(0, leftStringEnd.position);
        leftLine.SetPosition(1, middleOfString.position);
        rightLine.SetPosition(0, rightStringEnd.position);
        rightLine.SetPosition(1, middleOfString.position);
    }
}
