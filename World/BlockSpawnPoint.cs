using UnityEngine;


public class BlockSpawnPoint : MonoBehaviour
{
    public BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid") || collision.CompareTag("Damage") || 
            collision.CompareTag("Destructible") || collision.CompareTag("Gem-Small") || collision.CompareTag("Gem-Large"))
        {
            Destroy(collision.gameObject);
            //print($"BlockSpawnPoint destroyed block: {collision.gameObject.name}");
        }
    }
}
