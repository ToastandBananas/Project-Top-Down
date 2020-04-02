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

    LayerMask obstacleMask;

    void Start()
    {
        itemData = GetComponent<ItemData>();
        itemDrop = GetComponent<ItemDrop>();
        itemPickup = GetComponent<ItemPickup>();
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<BoxCollider2D>();

        obstacleMask = LayerMask.GetMask("Walls", "Doors");
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
            if (collision.tag == "NPC Body" || collision.tag == "Player Body")
            {
                BasicStats basicStats = collision.GetComponentInParent<BasicStats>();
                basicStats.TakeDamage(bowShotFrom.damage);

                float percentDamage = bowShotFrom.damage / basicStats.maxHealth;
                basicStats.SpawnBlood(collision.transform, transform, percentDamage, obstacleMask);

                if (collision.transform.Find("Arrows") != null)
                    transform.SetParent(collision.transform.Find("Arrows"));
                else
                {
                    GameObject Arrows = new GameObject();
                    Arrows.name = "Arrows";
                    Arrows.transform.SetParent(collision.transform);
                    transform.SetParent(collision.transform.Find("Arrows"));
                }

                StopArrow();
            }
            else if (collision.tag != "Weapon")
            {
                if (collision.GetComponent<ItemData>() != null)
                    collision.GetComponent<ItemData>().durability -= bowShotFrom.damage;

                if (collision.transform.Find("Arrows") != null)
                    transform.SetParent(collision.transform.Find("Arrows"));
                else
                {
                    GameObject Arrows = new GameObject();
                    Arrows.name = "Arrows";
                    Arrows.transform.SetParent(collision.transform);
                    transform.SetParent(collision.transform.Find("Arrows"));
                }

                StopArrow();
            }
        }
    }
}
