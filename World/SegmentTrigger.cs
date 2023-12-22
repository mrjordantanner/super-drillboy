using UnityEngine;


public class SegmentTrigger : MonoBehaviour, ISegmentIdentifier
{
    public bool renderSegmentTrigger;
    public bool hasBeenReached;
    SpriteRenderer spriteRenderer;
    public GameObject Entity { get { return gameObject; } }
    public int SegmentId { get; set; }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        if (renderSegmentTrigger)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.red;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (hasBeenReached) return;

            hasBeenReached = true;
            if (renderSegmentTrigger)
            {

                spriteRenderer.color = Color.green;
            }


            MapGenerator.Instance.OnSegmentTriggerReached(this);

        }
    }
}
