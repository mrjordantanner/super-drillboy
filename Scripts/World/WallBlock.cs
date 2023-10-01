using UnityEngine;


public class WallBlock : MonoBehaviour
{
    public int segmentID;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid") || collision.CompareTag("Damage") || collision.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
        }
    }
}
