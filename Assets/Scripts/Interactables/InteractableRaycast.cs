using UnityEngine;

public class InteractableRaycast : MonoBehaviour
{
    GameManager gm;
    PlayerMovement playerMovement;

    Vector2 worldPoint;
    Vector2 dir;
    RaycastHit2D hit;
    RaycastHit2D hit2;
    LayerMask wallMask;
    LayerMask obstacleMask;
    LayerMask interactableMask;

    void Start()
    {
        gm = GameManager.instance;
        playerMovement = PlayerMovement.instance;
        wallMask = LayerMask.GetMask("Walls");
        obstacleMask = LayerMask.GetMask("Walls", "Doors");
        interactableMask = LayerMask.GetMask("Interactables", "Doors");
    }
    
    void Update()
    {
        if (gm.isUsingController == false && GameControls.gamePlayActions.leftCtrl.IsPressed)
        {
            worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(worldPoint, Vector2.zero, 1f, interactableMask);
            if (hit.collider != null)
            {
                hit.collider.TryGetComponent(out Interactable interactable);
                if (interactable != null && gm.currentlySelectedInteractable != interactable)
                {
                    // Debug.Log(interactable.name);
                    if (gm.currentlySelectedInteractable != null)
                        gm.currentlySelectedInteractable.sr.material = gm.currentlySelectedInteractable.originalMaterial;

                    gm.currentlySelectedInteractable = interactable;
                    interactable.sr.material = interactable.highlightMaterial;
                }

                if (GameControls.gamePlayActions.playerLeftAttack.WasPressed && interactable != null)
                {
                    if (interactable.playerInRange == false)
                    {
                        // Walk to the object and use it if the player can see it directly
                        dir = (interactable.transform.position - playerMovement.transform.position).normalized;

                        if (hit.collider.GetComponent<Door>() != false)
                            hit2 = Physics2D.Raycast(playerMovement.transform.position, dir, Vector2.Distance(interactable.transform.position, playerMovement.transform.position), wallMask);
                        else
                            hit2 = Physics2D.Raycast(playerMovement.transform.position, dir, Vector2.Distance(interactable.transform.position, playerMovement.transform.position), obstacleMask);
                        
                        if (hit2.collider == null)
                            StartCoroutine(playerMovement.MoveToInteractable(gm.currentlySelectedInteractable.transform, playerMovement.runSpeed));
                        //else
                            //Debug.Log(hit2.collider);
                    }
                    else if (interactable == gm.currentlySelectedInteractable)
                        InteractWithObject();
                }
            }
        }
        else if (gm.currentlySelectedInteractable != null)
        {
            gm.currentlySelectedInteractable.sr.material = gm.currentlySelectedInteractable.originalMaterial;
            gm.currentlySelectedInteractable = null;
        }
    }

    void InteractWithObject()
    {
        if (hit.collider.TryGetComponent(out ItemPickup itemPickup) != false)
            itemPickup.Interact();
        else if (hit.collider.TryGetComponent(out Container container) != false)
            container.Interact();
        else if (hit.collider.TryGetComponent(out Door door) != false)
            door.Interact();
    }
}
