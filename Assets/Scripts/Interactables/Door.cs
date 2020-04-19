using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool isVerticalDoorway;
    public bool isOpen;
    public bool NPCInRange;

    Quaternion newRotation;

    AudioManager audioManager;
    PlayerAttack playerAttack;

    List<GameObject> NPCGameObjectsInRange;

    public override void Start()
    {
        base.Start();

        audioManager = AudioManager.instance;
        NPCGameObjectsInRange = new List<GameObject>();
        playerAttack = FindObjectOfType<PlayerAttack>();
    }
    
    public override void Update()
    {
        base.Update();

        if (NPCInRange && isOpen == false)
            isOpen = true;

        if (isOpen)
            OpenDoor();
        else
            CloseDoor();
    }

    public override void Interact()
    {
        base.Interact();
        
        if (playerInRange)
        {
            if (isOpen == false)
            {
                isOpen = true;
                audioManager.PlayRandomSound(audioManager.openDoorSounds, transform.position);
            }
            else
            {
                isOpen = false;
                audioManager.PlayRandomSound(audioManager.closeDoorSounds, transform.position);
            }
        }
    }

    void OpenDoor()
    {
        if (isVerticalDoorway == false && Mathf.Abs(transform.rotation.z) != 90)
        {
            newRotation = Quaternion.AngleAxis(90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
        else if (isVerticalDoorway && Mathf.Abs(transform.rotation.z) != 180)
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
        else if (isVerticalDoorway && Mathf.Abs(transform.rotation.z) != 90)
        {
            newRotation = Quaternion.AngleAxis(-90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (collision.tag == "NPC" /* TODO: && canOpenDoors */)
        {
            if (NPCGameObjectsInRange.Contains(collision.gameObject) == false)
                NPCGameObjectsInRange.Add(collision.gameObject);

            NPCInRange = true;
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.tag == "NPC" /* TODO: && canOpenDoors */)
        {
            if (NPCGameObjectsInRange.Contains(collision.gameObject))
                NPCGameObjectsInRange.Remove(collision.gameObject);

            if (NPCGameObjectsInRange.Count == 0)
                NPCInRange = false;
        }
    }
}
