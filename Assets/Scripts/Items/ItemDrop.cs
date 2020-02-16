using System.Collections;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item item;
    public bool isDropped;
    
    Rigidbody2D rb;
    ItemPickup itemPickupScript;
    WeaponDamage weaponDamageScript;
    Transform looseItemsContainer;

    LayerMask obstacleMask;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemPickupScript = GetComponent<ItemPickup>();
        weaponDamageScript = GetComponent<WeaponDamage>();
        looseItemsContainer = GameObject.Find("Loose Items").transform;
        obstacleMask = LayerMask.GetMask("Walls", "Floors");
    }

    public void DropItem()
    {
        isDropped = true;
        transform.SetParent(looseItemsContainer, true);
        itemPickupScript.enabled = true;
        weaponDamageScript.enabled = false;
        AddForce();
    }

    // Call this when you want to apply force in a random direction
    void AddForce()
    {
        StartCoroutine(FakeAddForceMotion(false));
    }

    IEnumerator FakeAddForceMotion(bool tossInAir)
    {
        Vector3 originalScale = transform.localScale;

        float i = 0.01f;
        float forceAmount = 0.5f;
        Vector2 randomForceModifier = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        float randomRotationAmount = Random.Range(-10, 10);

        Vector3 dir;
        float raycastDistance;
        RaycastHit2D hit;

        bool dirFound = false;
        while (dirFound == false)
        {
            // Make sure the item isn't going to go through an obstacle
            dir = (new Vector3((forceAmount / i) * randomForceModifier.x, (forceAmount / i) * randomForceModifier.y, 0) - transform.position).normalized;
            raycastDistance = Vector3.Distance(transform.position, transform.position + dir * 3f);
            hit = Physics2D.Raycast(transform.position, dir, raycastDistance, obstacleMask);

            if (hit)
                randomForceModifier = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            else
                dirFound = true;
        }

        while (forceAmount > i)
        {
            if (tossInAir)
            {
                // Simulate to look like the item was tossed into the air by increasing and then decreasing its scale
                if (i <= forceAmount / 2)
                    transform.localScale *= 1.05f;
                else if (transform.localScale.x > originalScale.x)
                    transform.localScale *= 0.95f;
            }

            // Move in a random direction towards a random distance
            rb.velocity = new Vector2((forceAmount / i) * randomForceModifier.x, (forceAmount / i) * randomForceModifier.y);

            // Rotate a random amount/direction
            transform.Rotate(new Vector3(0, 0, randomRotationAmount));

            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = originalScale;
        rb.velocity = Vector2.zero;
        yield return null;
    }
}
