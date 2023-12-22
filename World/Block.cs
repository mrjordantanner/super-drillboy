using System.Collections;
using UnityEngine;


public class Block : MonoBehaviour
{
    public enum BlockType { Solid, Destructible, Damage, Nitro }
    public BlockType blockType;

    [HideInInspector]
    public MapSection parentMapSection;

    public BlockSet blockSet;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer frame;
    public SpriteRenderer spriteExtensionRenderer;
    public Animator anim;
    Collider2D coll;

    public bool isDestroyed;
    public bool applyLevelTheme = true;
    public PlayerDetector playerDetector;

    [HideInInspector]
    public Material startingMaterial;
    Sprite currentSprite, invertedSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        startingMaterial = spriteRenderer.material;
    }

    private void OnEnable()
    {
        StartCoroutine(EnableBlock());
    }

    private void OnDisable()
    {
        if (isDestroyed)
        {
            CancelInvoke(nameof(TryToRegenerate));
        }
    }

    public void ApplyBlockSet(BlockSet set)
    {
        if (!spriteRenderer) return;

        blockSet = set;

        // Apply block variant
        var spriteVariant = set.GetVariant();
        spriteRenderer.sprite = spriteVariant;

        //currentSprite = spriteVariant;
        //spriteRenderer.sprite = currentSprite;
        //invertedSprite = Utils.CreateInvertedColorSprite(currentSprite);

        // Possibly apply a sprite extension
        var spriteExtension = set.RollForSpriteExtension();
        if (spriteExtension)
        {
            spriteExtensionRenderer.sprite = spriteExtension;

            // 50% chance to flip sprite X
            if (Utils.CoinFlip())
            {
                spriteExtensionRenderer.flipX = !spriteExtensionRenderer.flipX;
            }
        }
        else
        {
            spriteExtensionRenderer.sprite = null;
        }
    }

    public void ApplyBlockSetByType()
    {
        if (!applyLevelTheme) return;

        switch (blockType)
        {
            case BlockType.Solid:
                ApplyBlockSet(LevelController.Instance.selectedLevel.block_solid);
                break;

            case BlockType.Damage:
                ApplyBlockSet(LevelController.Instance.selectedLevel.block_damage);
                break;

            case BlockType.Destructible:
                ApplyBlockSet(LevelController.Instance.selectedLevel.block_destructible);
                break;

            default:
                break;
        }
    }

    public void DestroyBlock()
    {
        StartCoroutine(DestroyAndRegenerate());
    }

    IEnumerator DestroyAndRegenerate()
    {
        if (isDestroyed) yield return null;

        if (coll)
        {
            coll.enabled = false;
        }

        SoundEffect sound = null;
        if (blockSet) sound = blockSet.destroySound != null ? blockSet.destroySound : AudioManager.Instance.soundBank.DestroyBlock;
        sound.Play();

        // TODO Experiment with inverted sprites or other sprite effects as HitFlash
        //currentSprite = spriteRenderer.sprite;
        //spriteRenderer.sprite = invertedSprite;
        spriteRenderer.material = MapGenerator.Instance.blockHitFlashMaterial;
        yield return new WaitForSeconds(MapGenerator.Instance.blockHitFlashDuration / 2);
        spriteRenderer.material = MapGenerator.Instance.blockHitFlashMaterial2;
        yield return new WaitForSeconds(MapGenerator.Instance.blockHitFlashDuration / 2);

        DisableBlock();

        if (anim)
        {
            anim.SetTrigger("Destroy");
            //yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Block-Destroy-1"));
            yield return new WaitForSeconds(0.1f);
        }

        Instantiate(blockSet.DestroyVFX, transform.position, Quaternion.identity, transform);

        Drops.Instance.RollForDrops(this);
        yield return new WaitForSeconds(MapGenerator.Instance.blockRegenerationTime);

        InvokeRepeating(nameof(TryToRegenerate), 0, 1);
    }

    void TryToRegenerate()
    {
        if (playerDetector == null)
        {
            Debug.LogError($"Block playerDetector was null: {gameObject.name}");
            return;
        }

        if (playerDetector.playerDetected) return;

        CancelInvoke(nameof(TryToRegenerate));
        StartCoroutine(EnableBlock());
    }

    public IEnumerator EnableBlock()
    {
        if (!isDestroyed) yield break;

        if (anim)
        {
            anim.SetTrigger("Regenerate");
            //yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Block-Regen-1"));
            yield return new WaitForSeconds(0.3f);
        }

        isDestroyed = false;

        if (coll)
        {
            coll.enabled = true;
        }
        else
        {
            Debug.LogError($"{name} in {parentMapSection} has no Collider");
        }

        if (frame)
        {
            frame.enabled = true;
        }
        else
        {
            Debug.LogError($"{name} in {parentMapSection} has no Frame");
        }

        if (spriteRenderer)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.material = startingMaterial;
            //spriteRenderer.sprite = currentSprite;
        }
        else
        {
            Debug.LogError($"{name} in {parentMapSection} has no spriteRenderer");
        }

        if (spriteExtensionRenderer)
        {
            spriteExtensionRenderer.enabled = true;
        }
        else
        {
            Debug.LogError($"{name} in {parentMapSection} has no spriteExtensionRenderer");
        }
    }

    public void DisableBlock()
    {
        isDestroyed = true;

        if (coll)
        {
            coll.enabled = false;
        }

        if (frame)
        {
            frame.enabled = false;
        }

        if (spriteRenderer)
        {
            spriteRenderer.enabled = false;
        }

        if (spriteExtensionRenderer)
        {
            spriteExtensionRenderer.enabled = false;
        }
    }
}
