
using UnityEngine;

public interface ISegmentIdentifier
{
    public GameObject Entity { get; }
    public int SegmentId { get; set; }
}
