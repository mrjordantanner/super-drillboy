using System.Collections;
using UnityEngine;


public class Block : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    public string audioClip = "Destroy-Block";
    public GameObject DestroyBlockVFX;

    public bool isDestroyed;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        boxCollider = GetComponent<BoxCollider2D>();    
    }

    public void DestroyBlock()
    {
        StartCoroutine(DestroyAndRegenerate());
    }

    IEnumerator DestroyAndRegenerate()
    {
        if (isDestroyed) yield break;

        isDestroyed = true;
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;

        AudioManager.Instance.Play(audioClip);
        Instantiate(DestroyBlockVFX, transform.position, Quaternion.identity, transform);

        // "Drop" coin
        var val = Random.value;
        if (val <= MapController.Instance.blockCoinDropChance)
        {
            Instantiate(MapController.Instance.CoinPrefab, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(MapController.Instance.blockRegenerationTime);

        isDestroyed = false;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}
