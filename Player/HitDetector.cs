using UnityEngine;


public class HitDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageCheck(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DamageCheck(collision);
    }

    void DamageCheck(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Damage") && !PlayerManager.Instance.invulnerable && !PlayerManager.Instance.isBoosting)
        {
            PlayerManager.Instance.TakeDamage();
        }
    }
}
