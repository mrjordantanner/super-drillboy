using System.Collections;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    [HideInInspector]
    public MapSection parentMapSection;

    public bool isDestroyed;
    public bool wasDropped;
    float lifespanTimer;

    //private void Awake()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    boxCollider = GetComponent<BoxCollider2D>();
    //}

    private void Update()
    {
        if (wasDropped)
        {
            lifespanTimer += Time.deltaTime;
            if (lifespanTimer >= MapGenerator.Instance.blockRegenerationTime)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DestroyPickup()
    {
        if (!wasDropped)
        {
            //StartCoroutine(DestroyAndRegenerate());
            DisablePickup();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //IEnumerator DestroyAndRegenerate()
    //{
    //    if (isDestroyed) yield return null;

    //    DisablePickup();
    //    yield return new WaitForSeconds(MapController.Instance.pickupRegenerationTime);
    //    EnablePickup();
    //}

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (isDestroyed)
        {
            EnablePickup();
        }
    }

    public void EnablePickup()
    {
        isDestroyed = false;
        if (spriteRenderer)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
           // Debug.LogError($"SpriteRenderer was null when enabling Pickup: {name}");
        }

        if (boxCollider)
        {
            boxCollider.enabled = true;
        }
        else
        {
           // Debug.LogError($"BoxCollider was null when enabling Pickup: {name}");
        }
    }

    public void DisablePickup()
    {
        isDestroyed = true;
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

}
