using UnityEngine;


public class Checkpoint : MonoBehaviour, ISegmentIdentifier
{
    public Color activeColor, inactiveColor;

    public bool hasBeenReached;
    SpriteRenderer spriteRenderer;
    public bool isFirstCheckpointInLevel;
    public GameObject Entity { get { return gameObject; } }
    public int SegmentId { get; set; }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = inactiveColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (hasBeenReached) return;

            hasBeenReached = true;
            spriteRenderer.color = activeColor;
            PlayerManager.Instance.lastCheckpointReached = this;

            PlayerManager.Instance.OnCheckpointReached();
            MapGenerator.Instance.OnCheckpointReached(this);

        }
    }
}
