using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool arrowShouldStop = false;
    public ItemData bowShotFrom;

    ItemData itemData;
    ItemDrop itemDrop;
    ItemPickup itemPickup;
    Rigidbody2D rigidBody;
    BoxCollider2D circleCollider;

    void Start()
    {
        itemData = GetComponent<ItemData>();
        itemDrop = GetComponent<ItemDrop>();
        itemPickup = GetComponent<ItemPickup>();
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<BoxCollider2D>();
    }

    public void StopArrow()
    {
        rigidBody.bodyType = RigidbodyType2D.Static;
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        arrowShouldStop = true;

        itemData.currentStackSize = 1;
        itemDrop.isDropped = true;
        itemPickup.enabled = true;

        circleCollider.enabled = false;
        this.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false)
        {
            if (collision.tag == "NPC")
            {
                collision.GetComponent<BasicStats>().TakeDamage(itemData.damage);
                transform.SetParent(collision.transform);
                StopArrow();
            }
            else if (collision.tag != "Weapon")
            {
                if (collision.GetComponent<ItemData>() != null)
                    collision.GetComponent<ItemData>().durability -= itemData.damage;
                transform.SetParent(collision.transform);
                StopArrow();
            }
        }
    }
}
