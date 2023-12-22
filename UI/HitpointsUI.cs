using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class HitpointsUI : MonoBehaviour
{
    [Header("Health")]
    public GameObject HealthIconPrefab;
    public GameObject HealthContainer;

    [Header("Armor")]
    public GameObject ArmorIconPrefab;
    public GameObject ArmorContainer;

    [HideInInspector] public List<Image> healthIcons = new();
    [HideInInspector] public List<Image> armorIcons = new();

    public void CreateAllIcons()
    {
        ClearAllIcons();

        CreateIcons((int)Stats.Instance.MaxHealth.Value, HealthIconPrefab, HealthContainer, healthIcons);
        CreateIcons((int)Stats.Instance.MaxArmor.Value, ArmorIconPrefab, ArmorContainer, armorIcons);
    }

    void CreateIcons(int number, GameObject Prefab, GameObject Container, List<Image> iconsList)
    {
        for (int i = 0; i < number; i++)
        {
            var newHitpointIcon = Instantiate(Prefab, Container.transform.position, Quaternion.identity, Container.transform);
            var image = newHitpointIcon.GetComponent<Image>();
            iconsList.Add(image);
        }
    }

    public void RefreshWithPop(List<Image> iconsList, int healthBeforeChange)
    {
        GameObject hitpointToPop = null;
        for (int i = 0; i < iconsList.Count; i++)
        {
            if (i == healthBeforeChange - 1)
            {
                hitpointToPop = iconsList[i].gameObject;
            }
        }

        StartCoroutine(PopAndRefresh(healthBeforeChange - 1, iconsList, hitpointToPop));
    }

    IEnumerator PopAndRefresh(int currentValue, List<Image> iconsList, GameObject hitpointToPop)
    {
        var normalScale = new Vector3(1, 1, 1);
        hitpointToPop.transform.localScale = normalScale;

        var scale = new Vector3(1.35f, 1.35f, 1);
        hitpointToPop.transform.DOScale(scale, 0.02f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.02f);

        UpdateColors(currentValue, iconsList);

        hitpointToPop.transform.DOScale(normalScale, 0.2f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.2f);

    }

    public void UpdateColors(int currentValue, List<Image> iconsList)
    {
        for (int i = 0; i < iconsList.Count; i++)
        {
            iconsList[i].color = i < currentValue ? Color.white : Color.gray;
        }
    }

    void ClearAllIcons()
    {
        ClearIcons(healthIcons);
        ClearIcons(armorIcons);
    }

    void ClearIcons(List<Image> iconsList)
    {
        foreach (var icon in iconsList)
        {
            Destroy(icon.gameObject);
        }
        iconsList.Clear();
    }

}
