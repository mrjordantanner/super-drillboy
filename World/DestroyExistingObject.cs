using UnityEngine;


public class DestroyExistingObject : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid") || 
            collision.CompareTag("Damage") ||
            collision.CompareTag("Destructible") || 
            collision.CompareTag("Gem-Small") || 
            collision.CompareTag("Gem-Large") ||
            collision.CompareTag("Health") ||
            collision.CompareTag("Extra Life"))
        {
            if (collision.gameObject == gameObject || collision.gameObject == transform.parent.gameObject) return;

            Destroy(collision.gameObject);
            //print($"DestroyExistingObject destroyed object: {collision.gameObject.name}");
        }
    }
}
