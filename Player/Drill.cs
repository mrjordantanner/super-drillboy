using UnityEngine;


public class Drill : MonoBehaviour
{
    private void Update()
    {
        if (PlayerManager.Instance.isDrilling || PlayerManager.Instance.isBoosting)
        {
            if (PlayerManager.Instance.player.controller.verticalRaycastHit)
            {
                CheckCollision(PlayerManager.Instance.player.controller.verticalRaycastHit.collider, false);
            }

            if (PlayerManager.Instance.player.controller.horizontalRaycastHit && PlayerManager.Instance.allowHorizontalDrilling)
            {
                CheckCollision(PlayerManager.Instance.player.controller.horizontalRaycastHit.collider, true);
            }
        }
    }

    void CheckCollision(Collider2D collision, bool wasHorizontalRaycast = false)
    {
        if (NitroBlockCheck(collision)) return;
        if (BoostDestroyCheck(collision)) return;
        if (DrillDestroyCheck(collision, wasHorizontalRaycast)) return;
    }

    GameObject PreviousNitroBlock = null;
    bool NitroBlockCheck(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Nitro")
            && (PlayerManager.Instance.isDashing || PlayerManager.Instance.isBoosting))
        {
            if (collision.gameObject == PreviousNitroBlock) return false;
            PreviousNitroBlock = collision.gameObject;

            PlayerManager.Instance.PlayerDestroyBlock(collision);
            SkillController.Instance.SetResourceToMax();

            return true;
        }
        return false;
    }

    GameObject PreviousBoostDestroyBlock = null;
    bool BoostDestroyCheck(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Destructible") ||
            collision.gameObject.CompareTag("Solid") ||
            collision.gameObject.CompareTag("Damage"))
            && PlayerManager.Instance.isBoosting)
        {
            if (collision.gameObject == PreviousBoostDestroyBlock) return false;
            PreviousBoostDestroyBlock = collision.gameObject;

            PlayerManager.Instance.PlayerDestroyBlock(collision);
            return true;
        }
        return false;
    }

    GameObject PreviousHorizontalDrillDestroyBlock = null;
    GameObject PreviousVerticalDrillDestroyBlock = null;
    bool DrillDestroyCheck(Collider2D collision, bool wasHorizontalRaycast = false)
    {
        if (collision.gameObject.CompareTag("Destructible")
            && PlayerManager.Instance.isDashing)
        {
            if (wasHorizontalRaycast && PlayerManager.Instance.canDrillHoriz && PlayerManager.Instance.allowHorizontalDrilling)
            {
                if (collision.gameObject == PreviousHorizontalDrillDestroyBlock) return false;
                PreviousHorizontalDrillDestroyBlock = collision.gameObject;

                PlayerManager.Instance.PlayerDestroyBlock(collision);
                StartCoroutine(PlayerManager.Instance.DrillCooldown(wasHorizontalRaycast));

                return true;
            }

            if (!wasHorizontalRaycast && PlayerManager.Instance.canDrillVert)
            {
                if (collision.gameObject == PreviousVerticalDrillDestroyBlock) return false;
                PreviousVerticalDrillDestroyBlock = collision.gameObject;

                PlayerManager.Instance.PlayerDestroyBlock(collision);
                StartCoroutine(PlayerManager.Instance.DrillCooldown(wasHorizontalRaycast));

                return true;
            }
        }
        return false;
    }
}
