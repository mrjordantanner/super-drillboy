using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRoom : MapSection
{
    public ScrollingBackground[] scrollingBackgrounds;
    bool hasBeenTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasBeenTriggered)
            {
                StartCoroutine(TriggerTransition());
            }

        }

        // Prevent blockGroups from appearing inside TransitionRooms
        if (collision.gameObject.GetComponent<BlockGroup>() != null)
        {
            Destroy(collision.gameObject);
        }
    }
    // TODO Maybe have child triggers that trigger the music/backround transition based on player position in the room?
    IEnumerator TriggerTransition()
    {
        hasBeenTriggered = true;
        StartCoroutine(AudioManager.Instance.MusicTransition(LevelController.Instance.selectedLevel.music, 1.5f));

        StartCoroutine(MapGenerator.Instance.BackgroundTransition());
        MapGenerator.Instance.isTransitionPending = false;

        yield return new WaitForSeconds(5f);
    }
}
