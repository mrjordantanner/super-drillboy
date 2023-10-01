using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSectionNode : MonoBehaviour
{
    public float scale = 5f;
    public Color color = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * -scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * -scale);
    }



}
