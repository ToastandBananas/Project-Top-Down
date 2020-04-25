using UnityEngine;

public class UIDontRotate : MonoBehaviour
{
    private Quaternion rotation;

    void Awake()
    {
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = rotation;
    }
}
