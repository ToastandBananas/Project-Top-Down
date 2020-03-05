using UnityEngine;

public class Highlight : MonoBehaviour
{
    SpriteRenderer sr;
    Material originalMaterial;
    Material highlightMaterial;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
        highlightMaterial = Resources.Load<Material>("Materials/Sprite-Default");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = highlightMaterial;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = originalMaterial;
        }
    }
}
