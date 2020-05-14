using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Dialogue : MonoBehaviour
{
    DialogueSystemTrigger dialogueSystemTrigger;
    StandardBarkUI barkUI;
    GameManager gm;

    bool playerInRange;
    Transform playerTransform;

    void Start()
    {
        dialogueSystemTrigger = GetComponent<DialogueSystemTrigger>();
        dialogueSystemTrigger.conversationActor = PlayerMovement.instance.transform;
        barkUI = GetComponentInChildren<StandardBarkUI>();

        playerTransform = PlayerMovement.instance.transform;
        gm = GameManager.instance;
    }

    void Update()
    {
        if (GameControls.gamePlayActions.playerInteract.WasPressed && playerInRange && gm.menuOpen == false && gm.dialogueUIPanel.isOpen == false && gm.currentlySelectedInteractable == null)
            DialogueManager.StartConversation(dialogueSystemTrigger.conversation, playerTransform, transform);
    }

    void FixedUpdate()
    {
        if ((gm.menuOpen || gm.dialogueUIPanel.isOpen) && barkUI.barkText.text != "" && barkUI.isPlaying)
            DisableBark();
    }

    public void DisableBark()
    {
        barkUI.barkText.text = "";
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
