using UnityEngine;

public class Interactable : MonoBehaviour
{
    // This component is for all objects that the player can interact with.
    // It is meant to act as a base class.

    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Material originalMaterial;

    [Header("Interactable")]
    public Interactable thisInteractable;
    public Material highlightMaterial;

    [HideInInspector] public ItemPickup thisItemPickup;
    [HideInInspector] public Container thisContainer;
    [HideInInspector] public Door thisDoor;

    public float interactRadius = 0.5f; // How close we need to be to interact
    public float distanceToPlayer;    // How close the object is to the player
    public bool playerInRange;
    Transform interactionTransform;   // The transform from where we interact
    
    Transform player;
    PlayerMovement playerMovement;
    [HideInInspector] public GameManager gm;

    //bool hasInteracted = false; // Have we already interacted with this object?

    public virtual void Start()
    {
        thisInteractable = this;
        interactionTransform = transform;
        playerMovement = PlayerMovement.instance;
        player = playerMovement.gameObject.transform;
        gm = GameManager.instance;

        TryGetComponent(out thisItemPickup);
        TryGetComponent(out thisContainer);
        TryGetComponent(out thisDoor);

        // For Highlighting
        if (tag == "Dead Body")
            sr = transform.Find("Body").GetComponent<SpriteRenderer>();
        else if (tag == "Container" || tag == "Door")
            sr = GetComponentInParent<SpriteRenderer>();
        else
            sr = GetComponent<SpriteRenderer>();

        if (sr != null) originalMaterial = sr.material;
    }

    public virtual void Update()
    {
        if (GameControls.gamePlayActions.playerInteract.WasPressed && playerInRange && gm.currentlySelectedInteractable == this)
            Interact();
    }

    void OnMouseExit()
    {
        if (sr != null && gm.currentlySelectedInteractable == this)
        {
            gm.currentlySelectedInteractable = null;
            sr.material = originalMaterial;
        }
    }

    public virtual void Interact()
    {
        // This method is meant to be overwritten
        // Debug.Log("Interacting with " + transform.name);
        
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInRange = true;
            if (sr != null && gm.currentlySelectedInteractable == null)
            {
                gm.currentlySelectedInteractable = this;
                sr.material = highlightMaterial;
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInRange = false;
            if (sr != null && gm.currentlySelectedInteractable == this)
            {
                gm.currentlySelectedInteractable = null;
                sr.material = originalMaterial;
            }
        }
    }
}
