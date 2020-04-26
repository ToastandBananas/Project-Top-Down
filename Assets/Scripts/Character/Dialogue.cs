using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Dialogue : MonoBehaviour
{
    PixelCrushers.DialogueSystem.DialogueSystemTrigger dialogue;

    bool playerInRange;

    void Start()
    {
        dialogue = GetComponent<DialogueSystemTrigger>();
        dialogue.conversationActor = PlayerMovement.instance.transform;
    }

    void Update()
    {
        if (playerInRange && GameControls.gamePlayActions.playerInteract.WasPressed && GameManager.instance.currentlySelectedInteractable == null)
            DialogueManager.StartConversation(dialogue.conversation, PlayerMovement.instance.transform, transform);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playerInRange = false;
    }
}
