using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardController : MonoBehaviour
{
    #region Singleton
    public static LeaderboardController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        #endregion

        Init();
    }

    public GameObject LeaderboardRowPrefab;
    public VerticalLayoutGroup table;
    public Vector3 offset = new(0, 0, 0);

    public float topYpos = 0f;

    public ScrollRect scrollRect;
    public RectTransform rectTransform;
    public LeaderboardRow rowOfCurrentUser = new();

    public void Init()
    {
        //rectTransform = table.gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // lock scrollbar to top of entries
        if (rectTransform.hasChanged)
        {
            if (rectTransform.anchoredPosition.y < topYpos)
                rectTransform.anchoredPosition = new Vector2(0, topYpos);
        }
    }

    public void FocusOnTop()
    {
        rectTransform.anchoredPosition = new Vector2(0, topYpos);
    }

    public void FocusOnRow()
    {
        Canvas.ForceUpdateCanvases();
        float posY = rectTransform.position.y - rowOfCurrentUser.transform.position.y;

        if (rowOfCurrentUser.transform.position.y <= 0)
        {
            FocusOnTop();
        }
        else
            rectTransform.anchoredPosition = new Vector2(0, posY + topYpos);
    }

    public void DestroyRows()
    {
        var allRows = GetComponentsInChildren<LeaderboardRow>();
        foreach (var row in allRows)
        {
            Destroy(row.gameObject);
        }
    }

    public void CreateRows(bool downloadDataOnRefresh = false)
    {
        DestroyRows();
        StartCoroutine(CreateLeaderboardRows(downloadDataOnRefresh));
    }

    public IEnumerator CreateLeaderboardRows(bool downloadDataOnRefresh = false)
    {
        if (downloadDataOnRefresh)
        {
            yield return new WaitForSecondsRealtime(1f);
            Dreamlo.Instance.DownloadAll();

            yield return new WaitForSecondsRealtime(1f);
        }

        yield return new WaitForSecondsRealtime(0f);

        var userDataList = Dreamlo.Instance.AllUserData;

        for (int i = 0; i < userDataList.Count; i++)
        {
            var userData = userDataList[i];

            var NewRowObject = Instantiate(LeaderboardRowPrefab, table.transform.position + offset, Quaternion.identity, table.transform);
            var newRow = NewRowObject.GetComponent<LeaderboardRow>();

            newRow.userNameText.text = userData.PlayerName;
            newRow.scoreText.text = Utils.FormatNumberWithCommas(userData.Score);
            newRow.rankText.text = (i + 1).ToString();

            if (userDataList[i].PlayerName == Config.Instance.currentUserDto.PlayerName)
            {
                rowOfCurrentUser = newRow;
                rowOfCurrentUser.blinkingText.blink = true;
            }
        }
    }

}
