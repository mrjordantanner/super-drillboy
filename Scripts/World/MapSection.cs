using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSection : MonoBehaviour
{
    public bool hasBeenVisited;
    public bool playerPresent;

    public Transform topNode, bottomNode;

    public int segmentID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasBeenVisited = true;
            playerPresent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerPresent = false;
        }
    }
}
