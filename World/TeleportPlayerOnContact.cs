using UnityEngine;


public class TeleportPlayerOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCheck(collision);
    }

    void PlayerCheck(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Player left play area boundary.  Teleporting to nearest checkpoint");
           PlayerManager.Instance.Teleport();
        }
    }
}
