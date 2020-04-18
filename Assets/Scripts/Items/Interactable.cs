using UnityEngine;

public class Interactable : MonoBehaviour
{
    // This component is for all objects that the player can interact with.
    // It is meant to act as a base class.

    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Material originalMaterial;

    [Header("Interactable")]
    public Material highlightMaterial;

    public float radius = 1f; // How close we need to be to interact
    Transform interactionTransform; // The transform from where we interact
    
    Transform player;
    PlayerMovement playerMovement;
    GameManager gm;

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
        gm = GameManager.instance;

        // For Highlighting
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
    }
    
    void Update()
    {
        if (GameControls.gamePlayActions.playerInteract.WasPressed && GameManager.instance.menuOpen == false)
        {
            if (player == null)
            {
                interactionTransform = transform;
                playerMovement = PlayerMovement.instance;
                player = playerMovement.gameObject.transform;
            }

            // If we're close enough
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            if (distance <= radius && playerMovement.itemsToBePickedUpCount == 0)
            {
                playerMovement.itemsToBePickedUpCount++;

                // Interact with the object
                Interact();

                playerMovement.StartPickUpCooldown();
            }
        }
    }

    // Draw our radius in the editor
    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && sr != null)
        {
            sr.material = highlightMaterial;
        }

        if (GameControls.gamePlayActions.playerInteract.WasPressed && GameManager.instance.menuOpen == false)
        {
            if (player == null)
            {
                interactionTransform = transform;
                playerMovement = PlayerMovement.instance;
                player = playerMovement.gameObject.transform;
            }
            
            if (playerMovement.itemsToBePickedUpCount == 0)
            {
                playerMovement.itemsToBePickedUpCount++;

                // Interact with the object
                Interact();

                playerMovement.StartPickUpCooldown();
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && sr != null)
        {
            sr.material = originalMaterial;
        }
    }
}
