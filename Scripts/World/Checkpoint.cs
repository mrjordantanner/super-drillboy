using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    public bool hasBeenReached;
    SpriteRenderer spriteRenderer;

    public int segmentID;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = GameManager.Instance.checkpointInactive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (hasBeenReached) return;

            hasBeenReached = true;
            spriteRenderer.color = GameManager.Instance.checkpointActive;
            GameManager.Instance.lastCheckpointReached = this;

            PlayerManager.Instance.OnCheckpointReached();
            MapController.Instance.OnCheckpointReached(segmentID);


        }
    }
}
