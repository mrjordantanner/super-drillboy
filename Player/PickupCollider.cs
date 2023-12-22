using UnityEngine;


public class PickupCollider : MonoBehaviour
{
    public CapsuleCollider2D capsuleCollider;
    GameObject previousSmallGemCollected, previousLargeGemCollected, previousArmorCollected;

    private void Start()
    {
        UpdateColliderSize();
    }

    public void UpdateColliderSize()
    {
        capsuleCollider.size = new Vector2(Stats.Instance.PickupDistance.Value, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerManager.Instance.isTeleporting) return;

        if (collision.gameObject.CompareTag("Gem-Small") && !PlayerManager.Instance.dead)
        {
            if (previousSmallGemCollected == collision.gameObject) return;

            previousSmallGemCollected = collision.gameObject;
            GemController.Instance.CollectGem(GemType.Small);
            collision.GetComponent<Pickup>().DestroyPickup();
        }

        if (collision.gameObject.CompareTag("Gem-Large") && !PlayerManager.Instance.dead)
        {
            if (previousLargeGemCollected == collision.gameObject) return;

            previousLargeGemCollected = collision.gameObject;
            GemController.Instance.CollectGem(GemType.Large);
            collision.GetComponent<Pickup>().DestroyPickup();
        }

        if (collision.gameObject.CompareTag("Armor") && !PlayerManager.Instance.dead)
        {
            if (previousArmorCollected == collision.gameObject) return;

            previousArmorCollected = collision.gameObject;
            PlayerManager.Instance.CollectArmor();
            collision.GetComponent<Pickup>().DestroyPickup();
        }
    }
}
