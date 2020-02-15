using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
    public Vector2 slotCoordinate = Vector2.zero;
    public GameObject iconPrefab;

    public Image iconImage;
    public Item item;

    bool isMovingItem;

    public void AddItem(Item newItem)
    {
        Instantiate(iconPrefab, transform.GetChild(0).transform);
        iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        item = newItem;
        
        iconImage.name = item.name;
        iconImage.sprite = item.icon;
        
        //icon.enabled = true;
    }

    public void ClearSlot()
    {
        Destroy(iconImage.gameObject);

        iconImage = null;
        item = null;

        //icon.sprite = null;
        //icon.enabled = false;
    }

    public void UpdateSlot(Item newItem)
    {
        item = newItem;

        iconImage.name = item.name;
        iconImage.sprite = item.icon;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            ClearSlot();
        }
    }

    public void MoveItem()
    {
        isMovingItem = true;
    }

    public void PlaceItemInSlot()
    {

    }
}
