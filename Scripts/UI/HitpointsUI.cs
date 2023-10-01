using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class HitpointsUI : MonoBehaviour
{
    public Image[] childIcons;
    public int hitpoints, maxHitpoints;

    public Color full, empty;

    public void RefreshHitpoints(int hitpoints)
    {
        for (int i = 0; i < maxHitpoints; i++)
        {
            if (i < hitpoints)
            {
                childIcons[i].color = full;
            }
            else
            {
                childIcons[i].color = empty;
            }

        }

    }
}
