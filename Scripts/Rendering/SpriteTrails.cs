using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class SpriteTrails : MonoBehaviour
{
    public bool on;

    public bool changeScale = false;
  //  public float scaleFactor = 1.25f;

    public Vector2 scale = new(1, 1);

    float spawnTimer = 0f;

    public Material spriteMaterial;

    List<GameObject> trailParts = new();

    public float duration = 0.3f;
    public float repeatRate = 0.01f;

    [Range(0.0f, 1.0f)]
    public float trailOpacity = 0.8f;

    [Range(0.0f, 1.0f)]
    public float trailColorRed;

    [Range(0.0f, 1.0f)]
    public float trailColorGreen;

    [Range(0.0f, 1.0f)]
    public float trailColorBlue;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (on)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= repeatRate)
            {
                SpawnTrailPart();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnTrailPart()
    {
        GameObject trailPart = new();
        trailPart.name = "TrailPart";

        // trailPart.transform.SetParent(VFXContainer.Instance.transform);

        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.enabled = false;
        trailPartRenderer.material = spriteMaterial;

        //trailPart.layer = 10;
        trailPart.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        trailPartRenderer.sortingOrder = 1;
        trailPartRenderer.sortingLayerName = "Level";

        trailPartRenderer.sprite = spriteRenderer.sprite;
        trailPart.transform.Rotate(transform.rotation.eulerAngles);

        //trailPart.transform.DOScaleY(1f, 0f);

        if (changeScale)
        {
            trailPart.transform.localScale *= scale;
        }
        else
        {

            trailPart.transform.localScale = transform.parent.localScale;
        }

        trailPart.transform.position = transform.position;
        trailPartRenderer.enabled = true;
        trailParts.Add(trailPart);

        StartCoroutine(FadeTrailPart(trailPartRenderer));

        Destroy(trailPart, duration);
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color trailColor;
        trailColor = new(trailColorRed, trailColorGreen, trailColorBlue, trailOpacity);

        trailPartRenderer.color = trailColor;
        trailPartRenderer.material.DOFade(0, duration).SetEase(Ease.Linear);
        yield return new WaitForEndOfFrame();
    }

}

