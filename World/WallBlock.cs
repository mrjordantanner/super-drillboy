using UnityEngine;


public class WallBlock : Block, ISegmentIdentifier
{
    public GameObject Entity { get { return gameObject; } }
    public int SegmentId { get; set; }

}
