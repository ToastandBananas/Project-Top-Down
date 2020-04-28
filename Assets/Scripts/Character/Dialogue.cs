using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Dialogue : MonoBehaviour
{
    PixelCrushers.DialogueSystem.DialogueSystemTrigger dialogue;
    GameManager gm;

    bool playerInRange;

    void Start()
    {
        dialogue = GetComponent<DialogueSystemTrigger>();
        dialogue.conversationActor = PlayerMovement.instance.transform;
        gm = GameManager.instance;
    }

    void Update()
    {
        if (playerInRange && GameControls.gamePlayActions.playerInteract.WasPressed && gm.menuOpen == false && gm.currentlySelectedInteractable == null)
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
