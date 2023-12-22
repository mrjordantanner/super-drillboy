using UnityEngine;


public class DisableExistingObject : MonoBehaviour
{
    public MapSection parentMapSection;
    [ReadOnly]
    public int segmentId;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsPickup(collision))
        {
            collision.gameObject.GetComponent<Pickup>().DisablePickup();
        }
        else if (IsBlock(collision))
        {
            collision.gameObject.GetComponent<Block>().DisableBlock();
        }
    }

    bool IsPickup(Collider2D collision)
    {
        return collision.CompareTag("Gem-Small") ||
             collision.CompareTag("Gem-Large") ||
             collision.CompareTag("Armor");
    }

    bool IsBlock(Collider2D collision)
    {
        return collision.CompareTag("Solid") ||
            collision.CompareTag("Damage") ||
            collision.CompareTag("Destructible") ||
            collision.CompareTag("Nitro");
    }
}
