using UnityEngine;


public class HitDetector : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Destructible") || 
            collision.gameObject.CompareTag("Solid") ||
            collision.gameObject.CompareTag("Damage"))
            && PlayerManager.Instance.isSuperDashing)
        {
            player.PlayerDestroyObject(collision);
        }

        if (collision.gameObject.CompareTag("Destructible") && PlayerManager.Instance.isDashing)
        {
            player.PlayerDestroyObject(collision);
        }

        if (collision.gameObject.CompareTag("Damage") && !PlayerManager.Instance.invulnerable)
        {
            PlayerManager.Instance.TakeDamage(PlayerManager.Instance.velocityY);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Destructible") && PlayerManager.Instance.isDashing)
        {
            player.PlayerDestroyObject(collision);
        }

        if (collision.gameObject.CompareTag("Damage") && !PlayerManager.Instance.invulnerable)
        {
            PlayerManager.Instance.TakeDamage(PlayerManager.Instance.velocityY);
        }
    }
}
