using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Collections;

public class Door : Interactable
{
    public float doorOpenTime = 0.35f;
    public bool isVerticalDoorway;
    public bool isOpen;
    public bool NPCInRange;

    Quaternion newRotation;

    AudioManager audioManager;
    PlayerAttack playerAttack;

    List<GameObject> NPCGameObjectsInRange;

    LayerMask openDoorLayer, closedDoorLayer;

    Bounds bounds;
    GraphUpdateObject graphUpdateObject;

    public override void Start()
    {
        base.Start();

        audioManager = AudioManager.instance;
        NPCGameObjectsInRange = new List<GameObject>();
        playerAttack = FindObjectOfType<PlayerAttack>();

        openDoorLayer   = LayerMask.NameToLayer("OpenDoors");
        closedDoorLayer = LayerMask.NameToLayer("ClosedDoors");

        // Set up variables for recalculating pathfinding around open doors
        bounds = GetComponent<CircleCollider2D>().bounds;
        bounds.Expand(Vector3.forward * 1000);
        graphUpdateObject = new GraphUpdateObject(bounds);
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

        if (isOpen == false)
        {
            isOpen = true;
            transform.parent.gameObject.layer = openDoorLayer;
            audioManager.PlayRandomSound(audioManager.openDoorSounds, transform.position);
        }
        else
        {
            isOpen = false;
            transform.parent.gameObject.layer = closedDoorLayer;
            audioManager.PlayRandomSound(audioManager.closeDoorSounds, transform.position);
        }

        StartCoroutine(UpdateGraph());
    }

    void OpenDoor()
    {
        if (isVerticalDoorway == false && Mathf.Abs(transform.parent.rotation.z) != 90)
        {
            newRotation = Quaternion.AngleAxis(90, Vector3.forward);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, newRotation, 10f * Time.deltaTime);
        }
        else if (isVerticalDoorway && Mathf.Abs(transform.parent.rotation.z) != 180)
        {
            newRotation = Quaternion.AngleAxis(-180, Vector3.forward);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, newRotation, 10f * Time.deltaTime);
        }
    }

    void CloseDoor()
    {
        if (isVerticalDoorway == false && transform.parent.rotation.z != 0)
        {
            newRotation = Quaternion.AngleAxis(0, Vector3.forward);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, newRotation, 10f * Time.deltaTime);
        }
        else if (isVerticalDoorway && Mathf.Abs(transform.parent.rotation.z) != 90)
        {
            newRotation = Quaternion.AngleAxis(-90, Vector3.forward);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, newRotation, 10f * Time.deltaTime);
        }
    }

    IEnumerator UpdateGraph()
    {
        yield return new WaitForSeconds(doorOpenTime);
        AstarPath.active.UpdateGraphs(graphUpdateObject);
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
