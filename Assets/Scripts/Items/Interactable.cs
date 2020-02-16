using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // This component is for all objects that the player can interact with.
    // It is meant to act as a base class.

    public float radius = 1f; // How close we need to be to interact
    Transform interactionTransform; // The transform from where we interact

    bool isFocus = false; // Is this interactable currently being focused?
    Transform player;
    PlayerMovement playerMovement;
    
    //bool hasInteracted = false; // Have we already interacted with this object?

    public virtual void Interact()
    {
        // This method is meant to be overwritten
        // Debug.Log("Interacting with " + transform.name);
    }

    void Start()
    {
        interactionTransform = transform;
        playerMovement = PlayerMovement.instance;
        player = playerMovement.gameObject.transform;
    }
    
    void Update()
    {
        if (GameControls.gamePlayActions.playerInteract.WasPressed)
        {
            // If we're close enough
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            if (distance <= radius && playerMovement.itemsToBePickedUpCount == 0)
            {
                playerMovement.itemsToBePickedUpCount++;

                // Interact with the object
                Interact();
                //hasInteracted = true;

                playerMovement.StartPickUpCooldown();
            }
        }
    }

    // Called when the object starts being focused
    /*public void OnFocused(Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        //hasInteracted = false;
    }

    // Called when the object is no longer focused
    public void OnUnfocused()
    {
        isFocus = false;
        player = null;
        //hasInteracted = false;
    }*/

    // Draw our radius in the editor
    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
