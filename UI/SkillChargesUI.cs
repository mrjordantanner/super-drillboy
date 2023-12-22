using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class SkillChargesUI : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Container;

    [HideInInspector] public List<Image> iconsList = new();

    public void CreateIcons()
    {
        ClearIcons();

        for (int i = 0; i < SkillController.Instance.CurrentSkill.Data.maxSkillCharges; i++)
        {
            var newIcon = Instantiate(Prefab, Container.transform.position, Quaternion.identity, Container.transform);
            var image = newIcon.GetComponent<Image>();
            iconsList.Add(image);
        }

        UpdateColors();
    }

    public void RefreshWithPop(int valueBeforeChange)
    {
        GameObject iconToPop = iconsList[0].gameObject;
        for (int i = 0; i < iconsList.Count; i++)
        {
            if (i == valueBeforeChange - 1)
            {
                iconToPop = iconsList[i].gameObject;
            }
        }

        StartCoroutine(PopAndRefresh(iconToPop));
    }

    IEnumerator PopAndRefresh(GameObject iconToPop)
    {
        var normalScale = new Vector3(1, 1, 1);
        iconToPop.transform.localScale = normalScale;

        var scale = new Vector3(1.35f, 1.35f, 1);
        iconToPop.transform.DOScale(scale, 0.02f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.02f);

        UpdateColors();

        iconToPop.transform.DOScale(normalScale, 0.2f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.2f);

    }

    public void UpdateColors()
    {
        for (int i = 0; i < iconsList.Count; i++)
        {
            if (i < SkillController.Instance.skillCharges)
            {
                iconsList[i].color = Color.white;
            }
            else
            {
                iconsList[i].color = Color.gray;
            }
        }
    }

    void ClearIcons()
    {
        foreach (var icon in iconsList)
        {
            Destroy(icon.gameObject);
        }

        iconsList.Clear();
    }
}
