using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isVerticalDoorway;
    public bool isOpen;
    public bool playerInRange;
    public bool NPCInRange;

    Quaternion newRotation;

    List<GameObject> NPCGameObjectsInRange;

    void Start()
    {
        NPCGameObjectsInRange = new List<GameObject>();
    }
    
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonUp("Interact"))
            {
                if (isOpen == false)
                    isOpen = true;
                else
                    isOpen = false;
            }
        }

        if (NPCInRange && isOpen == false)
            isOpen = true;

        if (isOpen)
            OpenDoor();
        else
            CloseDoor();
    }

    void OpenDoor()
    {
        if (isVerticalDoorway == false && transform.rotation.z != 90)
        {
            newRotation = Quaternion.AngleAxis(90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
        else if (isVerticalDoorway && transform.rotation.z != -180)
        {
            newRotation = Quaternion.AngleAxis(-180, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
    }

    void CloseDoor()
    {
        if (isVerticalDoorway == false && transform.rotation.z != 0)
        {
            newRotation = Quaternion.AngleAxis(0, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
        else if (isVerticalDoorway && transform.rotation.z != -90)
        {
            newRotation = Quaternion.AngleAxis(-90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInRange = true;
        else if (collision.tag == "NPC" /* TODO: && canOpenDoors */)
        {
            if (NPCGameObjectsInRange.Contains(collision.gameObject) == false)
                NPCGameObjectsInRange.Add(collision.gameObject);

            NPCInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInRange = false;
        else if (collision.tag == "NPC" /* TODO: && canOpenDoors */)
        {
            if (NPCGameObjectsInRange.Contains(collision.gameObject))
                NPCGameObjectsInRange.Remove(collision.gameObject);

            if (NPCGameObjectsInRange.Count == 0)
                NPCInRange = false;
        }
    }
}
