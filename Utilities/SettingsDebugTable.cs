using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class SettingsDebugTable : MonoBehaviour
{
    public GameObject SettingsDebugRowPrefab;
    public VerticalLayoutGroup table;
    public ScrollRect scrollRect;
    public RectTransform rectTransform;

    int rowCount;
    float rowHeight = 36f;

    List<SettingsDebugRow> rows = new();

    private void Update()
    {
        ClampScrollRect();
    }

    private void ClampScrollRect()
    {
        float contentHeight = rectTransform.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        float minY = contentHeight - viewportHeight;
        float maxY = rowHeight * (rowCount - 1);

        if (scrollRect.verticalNormalizedPosition < 0)
        {
            scrollRect.verticalNormalizedPosition = 0;
        }
        else if (scrollRect.verticalNormalizedPosition > 1)
        {
            scrollRect.verticalNormalizedPosition = 1;
        }

        float clampedY = Mathf.Clamp(rectTransform.anchoredPosition.y, minY, maxY);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, clampedY);
    }

    public void PopulateTable()
    {
        if (Config.Instance.allSettings.Length == 0)
        {
            Debug.LogError("Config settings not loaded!");
            return;
        }

        rows = new();

        foreach (var setting in Config.Instance.allSettings)
        {
            var NewRowObject = Instantiate(SettingsDebugRowPrefab, table.transform.position, Quaternion.identity, table.transform);
            var newRow = NewRowObject.GetComponent<SettingsDebugRow>();
            newRow.Setting = setting;
            newRow.UpdateRow();

            rows.Add(newRow);
        }

        rowCount = rows.Count;
    }

    public void UpdateAllRows()
    {
        foreach (var row in rows)
        {
            row.UpdateRow();
        }
    }
}
