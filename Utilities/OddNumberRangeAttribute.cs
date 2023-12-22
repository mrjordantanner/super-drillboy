using UnityEngine;


public class OddNumberRangeAttribute : PropertyAttribute
{
    public readonly int min;
    public readonly int max;

    public OddNumberRangeAttribute(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}